using System;
using System.Collections.Generic;
using System.Reflection;

namespace PipelineR
{
    public interface IStepHandler<TPipelineContext> where TPipelineContext : class
    {
        Func<TPipelineContext, bool> Condition { get; set; }

        TPipelineContext Context { get; set; }

        IStepHandler<TPipelineContext> NextStep { get; set; }

        void AddVariable(PropertyInfo propertyInfo, object value);

        void LoadVariables();

        StepHandlerResult HandleStep();
    }
}