using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Moq;
using PipelineR.Test.Request;
using PipelineR.Test.RequestHandlers;
using Xunit;

namespace PipelineR.Test
{
  
    public class RequestHandlerOrchestratorTest
    {
        public static Expression<Func<ContextSample,SampleRequest, bool>> Condition()
        {
            return (context, request) => request.Name == "name_test";
        }
        [Fact]
        public void ExecuteHandler_RequestHandlerWithoutCondition_Execute()
        {
            var context = new ContextSample()
            {
                Values = new List<string>() { "primeiro" }
            };

            var resilt = new ContextSample();
             context.ConvertTo<ContextSample>(resilt);
       
        }

        [Fact]
        public void ExecuteHandler_RequestHandlerWithConditionTrue_Execute()
        {
            var request = new SampleRequest
            {
                Name = "name_test"
            };

            var requestHandler = new Mock<FirstRequestHandler>(new ContextSample(),Condition());
            
            RequestHandlerOrchestrator.ExecuteHandler(request, requestHandler.Object);


            requestHandler.Verify(p => p.HandleRequest(request));
         
        }

        [Fact]
        public void ExecuteHandler_RequestHandlerWithConditionFalse_NotExecute()
        {
            var request = new SampleRequest
            {
                Name = "name_test2"
            };

            var requestHandler = new Mock<FirstRequestHandler>(new ContextSample(), Condition());

            RequestHandlerOrchestrator.ExecuteHandler(request, requestHandler.Object);


            requestHandler.Verify(p => p.HandleRequest(request), Times.Never());
     
        }
    }
}
