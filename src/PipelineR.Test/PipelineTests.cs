using PipelineR.Test.Request;
using PipelineR.Test.RequestHandlers;

using Xunit;

namespace PipelineR.Test
{
    public class PipelineTests
    {
        [Fact]
        public void Should_finish_pipeline_successfully()
        {
            var context = new ContextSample();
            var request = new SampleRequest();

            var pipeline = new Pipeline<ContextSample, SampleRequest>()
                .AddNext(new InitializeCreateUserHandler(context))
                .AddNext(new CreateUserApiOneHandler(context))
                .AddNext(new CreateUserApiTwoHandler(context))
                .AddNext(new UpdateAccountHandler(context))
                .AddNext(new CreateUserApiThreeHandler(context))
                .AddNext(new CreateUserApiFourHandler(context))
                .Execute(request);

            Assert.Equal("Success", pipeline.ResultObject.ToString());
            Assert.Equal(201, pipeline.StatusCode);
            Assert.True(context.InitializeCreateUserHandlerWasExecuted);
            Assert.True(context.CreateUserApiOneHandlerWasExecuted);
            Assert.True(context.CreateUserApiTwoHandlerWasExecuted);
            Assert.True(context.UpdateAccountHandlerWasExecuted);
            Assert.True(context.CreateUserApiThreeHandlerWasExecuted);
            Assert.True(context.CreateUserApiFourHandlerWasExecuted);
        }

        [Fact]
        public void Should_not_execute_step_because_condition_is_false()
        {
            var context = new ContextSample();
            var request = new SampleRequest();

            var firstStepToAvoid = new CreateUserApiTwoHandler(context)
            {
                Condition = (c, r) => false
            };
            var secondStepToAvoid = new CreateUserApiThreeHandler(context)
            {
                Condition = (c, r) => false
            };

            var pipeline = new Pipeline<ContextSample, SampleRequest>()
                .AddNext(new InitializeCreateUserHandler(context))
                .AddNext(new CreateUserApiOneHandler(context))
                .AddNext(firstStepToAvoid)
                .AddNext(new UpdateAccountHandler(context))
                .AddNext(secondStepToAvoid)
                .AddNext(new CreateUserApiFourHandler(context))
                .Execute(request);

            Assert.Equal("Success", pipeline.ResultObject.ToString());
            Assert.Equal(201, pipeline.StatusCode);
            Assert.True(context.InitializeCreateUserHandlerWasExecuted);
            Assert.True(context.CreateUserApiOneHandlerWasExecuted);
            Assert.False(context.CreateUserApiTwoHandlerWasExecuted);
            Assert.True(context.UpdateAccountHandlerWasExecuted);
            Assert.False(context.CreateUserApiThreeHandlerWasExecuted);
            Assert.True(context.CreateUserApiFourHandlerWasExecuted);
        }

        [Fact]
        public void Should_recover_pipeline_from_specific_step()
        {
            var recoverPipelineFromStep = "UpdateAccountHandler";
            var context = new ContextSample();
            var request = new SampleRequest();

            var pipeline = new Pipeline<ContextSample, SampleRequest>()
                .AddNext(new InitializeCreateUserHandler(context))
                    .WithRecovery(new InitializeCreateUserRecoveryHandler(context))
                .AddNext(new CreateUserApiOneHandler(context))
                .AddNext(new CreateUserApiTwoHandler(context))
                .AddNext(new UpdateAccountHandler(context))
                    .WithRecovery(new UpdateAccountRecoveryHandler(context))
                .AddNext(new CreateUserApiThreeHandler(context))
                .AddNext(new CreateUserApiFourHandler(context))
                    .WithRecovery(new CreateUserApiFourRecoveryHandler(context))
                .Execute(request, recoverPipelineFromStep);

            Assert.Equal("Success", pipeline.ResultObject.ToString());
            Assert.Equal(201, pipeline.StatusCode);
            Assert.False(context.InitializeCreateUserHandlerWasExecuted);
            Assert.False(context.CreateUserApiOneHandlerWasExecuted);
            Assert.False(context.CreateUserApiTwoHandlerWasExecuted);
            Assert.True(context.UpdateAccountHandlerWasExecuted);
            Assert.True(context.CreateUserApiThreeHandlerWasExecuted);
            Assert.True(context.CreateUserApiFourHandlerWasExecuted);

            Assert.True(context.InitializeCreateUserRecoveryHandlerWasExecuted);
            Assert.False(context.UpdateAccountRecoveryHandlerWasExecuted);
            Assert.False(context.CreateUserApiFourRecoveryHandlerWasExecuted);
        }

        [Fact]
        public void Should_recover_step_abort_pipeline()
        {
            var recoverPipelineFromStep = "UpdateAccountHandler";
            var context = new ContextSample()
            {
                InitializeCreateUserRecoveryHandlerShouldAbort = true
            };
            var request = new SampleRequest();

            var pipeline = new Pipeline<ContextSample, SampleRequest>()
                .AddNext(new InitializeCreateUserHandler(context))
                    .WithRecovery(new InitializeCreateUserRecoveryHandler(context))
                .AddNext(new CreateUserApiOneHandler(context))
                .AddNext(new CreateUserApiTwoHandler(context))
                .AddNext(new UpdateAccountHandler(context))
                    .WithRecovery(new UpdateAccountRecoveryHandler(context))
                .AddNext(new CreateUserApiThreeHandler(context))
                .AddNext(new CreateUserApiFourHandler(context))
                    .WithRecovery(new CreateUserApiFourRecoveryHandler(context))
                .Execute(request, recoverPipelineFromStep);

            Assert.Equal("Aborted", pipeline.ResultObject.ToString());
            Assert.Equal(400, pipeline.StatusCode);
            Assert.False(context.InitializeCreateUserHandlerWasExecuted);
            Assert.False(context.CreateUserApiOneHandlerWasExecuted);
            Assert.False(context.CreateUserApiTwoHandlerWasExecuted);
            Assert.False(context.UpdateAccountHandlerWasExecuted);
            Assert.False(context.CreateUserApiThreeHandlerWasExecuted);
            Assert.False(context.CreateUserApiFourHandlerWasExecuted);

            Assert.True(context.InitializeCreateUserRecoveryHandlerWasExecuted);
            Assert.False(context.UpdateAccountRecoveryHandlerWasExecuted);
            Assert.False(context.CreateUserApiFourRecoveryHandlerWasExecuted);
        }
    }
}
