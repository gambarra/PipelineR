using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PipelineR
{
    public static class PipelineRService
    {
        public static void Setup(this IServiceCollection services)
        {

            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries.Where(p =>
                p.Type.Equals("Project", StringComparison.CurrentCultureIgnoreCase));

            foreach (var library in dependencies)
            {
                var name = new AssemblyName(library.Name);
                var assembly = Assembly.Load(name);
                assemblies.Add(assembly);
            }

            var types = assemblies
                        .SelectMany(a => a.GetTypes())
                        .Select(a => a.GetTypeInfo());

            var steps = types
                        .Where(a => a.IsClass && !a.Name.Contains("StepHandler") && a.ImplementedInterfaces.Any(i => i.Name.Contains("StepHandler")));

            foreach (var step in steps)
            {
                var @interface = step.GetInterfaces().Where(i => !i.Name.Contains("IStep") && !i.Name.Contains("StepHandler"));
                services.AddScoped(@interface.FirstOrDefault(), step.AsType());
            }
        }
    }
}