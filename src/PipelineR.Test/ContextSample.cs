using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineR.Test
{
    public class ContextSample : BaseContext
    {
        public IEnumerable<string> Values { get; set; }

        public bool InitializeCreateUserHandlerWasExecuted { get; set; }
        public bool CreateUserApiOneHandlerWasExecuted { get; set; }
        public bool CreateUserApiTwoHandlerWasExecuted { get; set; }
        public bool UpdateAccountHandlerWasExecuted { get; set; }
        public bool CreateUserApiThreeHandlerWasExecuted { get; set; }
        public bool CreateUserApiFourHandlerWasExecuted { get; set; }
        public bool InitializeCreateUserRecoveryHandlerWasExecuted { get; set; }
        public bool UpdateAccountRecoveryHandlerWasExecuted { get; set; }
        public bool CreateUserApiFourRecoveryHandlerWasExecuted { get; set; }

        public bool InitializeCreateUserRecoveryHandlerShouldAbort { get; set; }
    }
}