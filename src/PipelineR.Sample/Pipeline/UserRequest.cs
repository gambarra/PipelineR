using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineR.Sample.Pipeline
{
    public class UserRequest : PipelineRequest
    {
        public string Name { get; set; }
        public string DocumentNumber { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
