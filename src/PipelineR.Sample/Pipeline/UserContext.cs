using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineR.Sample.Pipeline
{
    public class UserContext : BaseContext
    {
        public UserContext():base()
        {
            
        }
    
        public bool CreateUserRequestHandlerSuccess { get; set; }

    }
}
