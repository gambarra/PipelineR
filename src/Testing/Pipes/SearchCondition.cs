using System;
using PipelineR.Interface;

namespace Testing.Pipes
{
    public interface ISearchCondition : ICondition<CarContext>
    {
    }

    public class SearchCondition : ISearchCondition
    {
        public Func<CarContext, bool> When()
        {
            return new Func<CarContext, bool>(p =>
            {
                var req = (CarCreate)p.Request;
                return req.Nome == "yuri";
            });
        }
    }
}