using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineR
{
    public class CacheSettings
    {
        public int TTLInMinutes { get; set; } = 1;
        public string ConnectionString { get; set; }
    }
}
