//using System;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Runtime.CompilerServices;
//using FluentValidation;
//using Microsoft.Extensions.DependencyInjection;

//namespace PipelineR
//{
//    public class Pipeline<TContext, TRequest> : IPipeline<TContext, TRequest> where TContext : BaseContext
//    {
//        private IRequestHandler<TContext, TRequest> _requestHandler;
//        private IRequestHandler<TContext, TRequest> _finallyRequestHandler;
//        private IValidator<TRequest> _validator;
//        private readonly IServiceProvider _serviceProvider;

//        private Pipeline(IServiceProvider serviceProvider)
//        {
//            this._serviceProvider = serviceProvider;
//        }

//        public Pipeline()
//        {

//        }
//        public static Pipeline<TContext, TRequest> Configure()
//        {
//            return new Pipeline<TContext, TRequest>();
//        }
//        public static Pipeline<TContext, TRequest> Configure(IServiceProvider serviceProvider)
//        {
//            return new Pipeline<TContext, TRequest>(serviceProvider);
//        }

//        public Pipeline<TContext, TRequest> AddNext(IRequestHandler<TContext, TRequest> requestHandler)
//        {
//            if (this._requestHandler == null)
//            {
//                this._requestHandler = requestHandler;
//            }
//            else
//            {
//                GetLastRequestHandler(this._requestHandler).NextRequestHandler = requestHandler;
//            }

//            return this;
//        }
//        public Pipeline<TContext, TRequest> AddNext<TRequestHandler>(Expression<Func<TContext, TRequest, bool>> condition)
//        {

//            var requestHandler = (IRequestHandler<TContext, TRequest>)_serviceProvider.GetService<TRequestHandler>();
//            requestHandler.Condition = condition;

//            return this.AddNext(requestHandler);
//        }

//        public Pipeline<TContext, TRequest> AddNext<TRequestHandler>() => AddNext<TRequestHandler>(null);
//        public Pipeline<TContext, TRequest> AddFinally(IRequestHandler<TContext, TRequest> requestHandler)
//        {
//            _finallyRequestHandler = requestHandler;
//            return this;
//        }
//        public Pipeline<TContext, TRequest> AddFinally<TRequestHandler>()
//        {
//            var requestHandler = (IRequestHandler<TContext, TRequest>)_serviceProvider.GetService<TRequestHandler>();
//            return this.AddFinally(requestHandler);
//        }

//        public Pipeline<TContext, TRequest> AddValidator(IValidator<TRequest> validator)
//        {
//            _validator = validator;
//            return this;
//        }

//        public Pipeline<TContext, TRequest> AddValidator<TValidator>()
//        {

//            var validator = (IValidator<TRequest>)_serviceProvider.GetService<TValidator>();
//            return this.AddValidator(validator);
//        }

//        public RequestHandlerResult Execute(TRequest request)
//        {
//            if (this._validator != null)
//            {
//                var validateResult = this._validator.Validate(request);

//                if (validateResult.IsValid == false)
//                {
//                    var errors = (validateResult.Errors.Select(p => new ErrorResult(p.ErrorMessage))).ToList();
//                    return new RequestHandlerResult(errors, 412);
//                }

//            }

//            if (this._requestHandler == null)
//            {
//                throw new ArgumentNullException("No started handlers");
//            }


//            this._requestHandler.Context.Request = request;

//            var result = RequestHandlerOrchestrator.ExecuteHandler(request, this._requestHandler);

//            result = ExecuteFinallyHandler(request) ?? result;

//            return result;
//        }

//        private RequestHandlerResult ExecuteFinallyHandler(TRequest request)
//        {

//            RequestHandlerResult result = null;

//            if (this._finallyRequestHandler != null)
//            {
//                result = this._finallyRequestHandler.HandleRequest(request);
//            }

//            return result;
//        }



//        private static IRequestHandler<TContext, TRequest> GetLastRequestHandler(
//            IRequestHandler<TContext, TRequest> requestHandler)
//        {
//            if (requestHandler.NextRequestHandler != null)
//            {
//                return GetLastRequestHandler(requestHandler.NextRequestHandler);
//            }


//            return requestHandler;
//        }
//    }

//    public interface IPipeline<TContext, in TRequest> where TContext : BaseContext
//    {
//        RequestHandlerResult Execute(TRequest request);
//    }
//}