using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineR
{

    public abstract class BasePipelineBuilder<TContext, TRequest> : IPipelineBuilder<TContext, TRequest> where TContext : BaseContext
    {
        protected readonly IServiceProvider ServiceProvider;

        public BasePipelineBuilder(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        public abstract IPipeline<TContext, TRequest> Create();
    }
}
