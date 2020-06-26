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
        public string Name { get; private set; }

        public void UpdateName(string name)
        {
            this.Name = name;
        }

    }
}
