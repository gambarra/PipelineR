using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Polly;
using Serilog;
using Serilog.Context;

namespace PipelineR
{
    public class Pipeline<TContext, TRequest> : IPipeline<TContext, TRequest> where TContext : BaseContext
    {
        private IRequestHandler<TContext, TRequest> _requestHandler;
        private readonly ICacheProvider _cacheProvider;
        private RequestHandler<TContext, TRequest> _lastRequestHandlerAdd;
        private IRequestHandler<TContext, TRequest> _finallyRequestHandler;
        private IValidator<TRequest> _validator;
        private readonly IServiceProvider _serviceProvider;
        private readonly Stack<RollbackHandler<TContext, TRequest>> _rollbacks;
        private IHandler<TContext, TRequest> _lastHandlerAdd;
        private bool _useReuseRequisitionHash;
        private string _requestKey;

        #region Constructores

        private Pipeline(IServiceProvider serviceProvider, string requestKey = null) : this()
        {
            this._serviceProvider = serviceProvider;
            this._requestKey = requestKey;
            _cacheProvider = serviceProvider.GetService<ICacheProvider>();
        }

        public Pipeline()
        {
            _rollbacks = new Stack<RollbackHandler<TContext, TRequest>>();
        }

        #endregion

        #region Configure
        public static Pipeline<TContext, TRequest> Configure()
        {
            return new Pipeline<TContext, TRequest>();
        }

        public static Pipeline<TContext, TRequest> Configure(IServiceProvider serviceProvider, string requestKey=null)
        {
            return new Pipeline<TContext, TRequest>(serviceProvider, requestKey);
        }

        public Pipeline<TContext, TRequest> UseRecoveryRequestByHash()
        {
            _useReuseRequisitionHash = true;
            return this;
        }
        #endregion

        #region AddNext

        public Pipeline<TContext, TRequest> AddNext(RequestHandler<TContext, TRequest> requestHandler)
        {
            if (this._requestHandler == null)
            {
                this._requestHandler = requestHandler;
            }
            else
            {
                GetLastRequestHandler(this._requestHandler).NextRequestHandler = requestHandler;
            }

            _lastRequestHandlerAdd = requestHandler;
            _lastHandlerAdd = requestHandler;
            return this;
        }

        public Pipeline<TContext, TRequest> AddNext<TRequestHandler>(
            Expression<Func<TContext, TRequest, bool>> condition)
            => this.AddNext<TRequestHandler>(condition, null);


        public Pipeline<TContext, TRequest> AddNext<TRequestHandler>(
            Expression<Func<TContext, TRequest, bool>> condition, Policy policy)
        {
            var requestHandler = ((RequestHandler<TContext, TRequest>)(IRequestHandler<TContext, TRequest>)_serviceProvider.GetService<TRequestHandler>());

            requestHandler.Condition = condition;
            requestHandler.Policy = policy;
            requestHandler.AddPipeline(this);

            return this.AddNext((RequestHandler<TContext, TRequest>)requestHandler);
        }

        public Pipeline<TContext, TRequest> AddNext<TRequestHandler>() => AddNext<TRequestHandler>(null);


        #endregion

        #region  AddCondition

        public Pipeline<TContext, TRequest> When(Expression<Func<TContext, TRequest, bool>> condition)
        {
            if (condition != null && this._lastHandlerAdd != null)
            {
                this._lastHandlerAdd.Condition = condition;
            }

            return this;
        }

        #endregion

        #region AddPolicy

        public Pipeline<TContext, TRequest> WithPolicy(Policy policy)
        {
            if (policy != null && this._lastHandlerAdd != null)
            {
                this._lastRequestHandlerAdd.Policy = policy;
            }

            return this;
        }
        public Pipeline<TContext, TRequest> WithPolicy(Policy<RequestHandlerResult> policy)
        {
            if (policy != null && this._lastHandlerAdd != null)
            {
                this._lastRequestHandlerAdd.PolicyRequestHandler = policy;
            }

            return this;
        }
        #endregion

        #region AddRollback

        public Pipeline<TContext, TRequest> Rollback(IRollbackHandler<TContext, TRequest> rollbackHandler)
        {

            var rollbackHandlerAux = (RollbackHandler<TContext, TRequest>)rollbackHandler;

            _lastHandlerAdd = rollbackHandler;

            _rollbacks.Push(rollbackHandlerAux);

            var rollbackIndex = _rollbacks.Count;

            rollbackHandlerAux.AddRollbackIndex(rollbackIndex);
            rollbackHandlerAux.RequestCondition = this._lastRequestHandlerAdd.Condition;

            this._lastRequestHandlerAdd.AddRollbackIndex(rollbackIndex);

            return this;
        }

        public Pipeline<TContext, TRequest> Rollback<TRollbackHandler>() where TRollbackHandler : IRollbackHandler<TContext, TRequest>
        {
            var rollbackHandler = (IRollbackHandler<TContext, TRequest>)_serviceProvider.GetService<TRollbackHandler>();
            this.Rollback(rollbackHandler);
            return this;
        }
        #endregion

        #region AddFinally
        public Pipeline<TContext, TRequest> AddFinally(IRequestHandler<TContext, TRequest> requestHandler)
        {
            _finallyRequestHandler = requestHandler;
            _lastRequestHandlerAdd = (RequestHandler<TContext, TRequest>)requestHandler;
            return this;
        }

        public Pipeline<TContext, TRequest> AddFinally<TRequestHandler>() => AddFinally<TRequestHandler>(null);


        public Pipeline<TContext, TRequest> AddFinally<TRequestHandler>(Policy policy)
        {
            var requestHandler = (IRequestHandler<TContext, TRequest>)_serviceProvider.GetService<TRequestHandler>();
            requestHandler.Policy = policy;
            return this.AddFinally(requestHandler);
        }

        #endregion

        #region AddValidator

        public Pipeline<TContext, TRequest> AddValidator(IValidator<TRequest> validator)
        {
            _validator = validator;
            return this;
        }

        public Pipeline<TContext, TRequest> AddValidator<TValidator>()
        {
            var validator = (IValidator<TRequest>)_serviceProvider.GetService<TValidator>();
            return this.AddValidator(validator);
        }

        #endregion

        #region AddRecovery
        public Pipeline<TContext, TRequest> WithRecovery(IRecoveryHandler<TContext, TRequest> requestHandler)
        {
            if (this._requestHandler != null)
            {
                _lastRequestHandlerAdd.RecoveryRequestHandler = requestHandler;
            }

            return this;
        }

        public Pipeline<TContext, TRequest> WithRecovery<TRequestHandler>(
            Expression<Func<TContext, TRequest, bool>> condition = null, Policy policy = null)
        {
            var requestHandler = ((RecoveryHandler<TContext, TRequest>)(IRecoveryHandler<TContext, TRequest>)_serviceProvider.GetService<TRequestHandler>());

            requestHandler.Condition = condition ?? _lastRequestHandlerAdd.Condition;
            requestHandler.Policy = policy ?? _lastRequestHandlerAdd.Policy;

            return this.WithRecovery(requestHandler);
        }
        #endregion


        public RequestHandlerResult Execute(TRequest request) => Execute(request, string.Empty);
        public RequestHandlerResult Execute(TRequest request, string recoverFromStep) => Execute(request, string.Empty, recoverFromStep);
        public RequestHandlerResult Execute(TRequest request, string idempotencyKey, string recoverFromStep)
        {
            if (this._validator != null)
            {
                var validateResult = this._validator.Validate(request);

                if (validateResult.IsValid == false)
                {
                    var errors = (validateResult.Errors.Select(p =>
                        new ErrorResult(null, p.ErrorMessage, p.PropertyName))).ToList();
                    return new RequestHandlerResult(errors, 400);
                }
            }

            if (this._requestHandler == null)
            {
                throw new ArgumentNullException("No started handlers");
            }

            RequestHandlerResult result = null;

            var lastRequestHandlerId = string.Empty;
            var nextRequestHandlerId = 
                !string.IsNullOrEmpty(recoverFromStep) ? 
                recoverFromStep :
                string.Empty;

            TContext context = null;

            var hash = idempotencyKey == string.Empty ? request.GenerateHash() : idempotencyKey;

            if (this._useReuseRequisitionHash)
            {
                var snapshot = this._cacheProvider.Get<PipelineSnapshot>(hash).Result;
                if (snapshot != null)
                {
                    if (snapshot.Success)
                    {
                        result = snapshot.Context.Response;
                        result.SetStatusCode(200);
                        return result;
                    }
                    else
                    {
                        context = (TContext)snapshot.Context;
                        context.Request = request;
                        nextRequestHandlerId = snapshot.LastRequestHandlerId;
                        this._requestHandler.UpdateContext(context);
                    }
                }

            }

            lastRequestHandlerId = Execute(request, nextRequestHandlerId, ref result);

            if (this._useReuseRequisitionHash)
            {

                var sucess = result?.IsSuccess() ?? false;
                var snapshot = new PipelineSnapshot(sucess,
                    lastRequestHandlerId,
                    this._requestHandler.Context);

                this._cacheProvider.Add<PipelineSnapshot>(snapshot, hash);
            }
            return result;
        }

        private string Execute(TRequest request, string nextRequestHandlerId, ref RequestHandlerResult result)
        {
            string lastRequestHandlerId;
            try
            {
                this._requestHandler.Context.Request = request;

                result = RequestHandlerOrchestrator
                    .ExecuteHandler(request, (RequestHandler<TContext, TRequest>)this._requestHandler, nextRequestHandlerId);
            }
            catch (PipelinePolicyException px)
            {
                result = px.Result;
            }
            catch (Exception ex)
            {
                if (Log.Logger != null)
                {

                    using (LogContext.PushProperty("RequestKey", this._requestKey))
                    {
                        Log.Logger.Error(ex, string.Concat("Error - ", this._requestHandler.Context.CurrentRequestHandleId));
                    }
                }
                result = new RequestHandlerResult(new ErrorResult(ex.Source, ex.GetBaseException().Message, ex, null, null), 500);
            }
            finally
            {
                lastRequestHandlerId = this._requestHandler.Context.CurrentRequestHandleId;
                result = ExecuteFinallyHandler(request) ?? result;
            }

            return lastRequestHandlerId;
        }

        private RequestHandlerResult ExecuteFinallyHandler(TRequest request)
        {
            RequestHandlerResult result = null;

            if (this._finallyRequestHandler != null)
            {
                result = ((RequestHandler<TContext, TRequest>)this._finallyRequestHandler).Execute(request);
            }

            return result;
        }

        private static IRequestHandler<TContext, TRequest> GetLastRequestHandler(
            IRequestHandler<TContext, TRequest> requestHandler)
        {
            if (requestHandler.NextRequestHandler != null)
            {
                return GetLastRequestHandler(requestHandler.NextRequestHandler);
            }


            return requestHandler;
        }

        internal void ExecuteRollback(int rollbackIndex, TRequest request)
        {
            foreach (var rollbackHandler in this._rollbacks.Where(rollbackHandler => rollbackHandler.Index <= rollbackIndex))
            {
                rollbackHandler.Execute(request);
            }
        }


    }

    public interface IPipeline<TContext, in TRequest> where TContext : BaseContext
    {
        RequestHandlerResult Execute(TRequest request, string idempotencyKey, string recoverFromStep);
        RequestHandlerResult Execute(TRequest request, string recoverFromStep);
        RequestHandlerResult Execute(TRequest request);
    }
}