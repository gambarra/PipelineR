using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineR
{
    public interface IPipelineBuilder<TContext, in TRequest> where TContext : BaseContext
    {
        IPipeline<TContext, TRequest> Create();
    }
}
