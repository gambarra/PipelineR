using AutoBogus;

namespace PipelineR.Faker
{
    public class StringGeneratorOverride : AutoGeneratorOverride
    {
        public override bool CanOverride(AutoGenerateContext context)
        {
            return context.GenerateType.IsSimpleType();
        }

        public override void Generate(AutoGenerateOverrideContext context)
        {
            var type = context.Instance.GetType();

            if (type.IsNumericType())
                context.Instance = 0;
            else if (type == typeof(string))
                context.Instance = "string";
        }
    }
}