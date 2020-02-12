using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PipelineR
{
    public static class PipelineRAutoInject
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static void SetupPipelineR(this IServiceCollection services)
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

            var searchingCondition = new []{ "StepHandler", "ICondition", "IWorkflow" };

            var contexts = types
                        .Where(a => a.IsClass && a.BaseType == typeof(BaseContext));

            foreach (var context in contexts)
                services.AddScoped(p => Activator.CreateInstance(context));

            var pipes = types
                        .Where(a => a.IsClass && 
                                       !searchingCondition.Any(exclude => a.Name.Contains(exclude)) && 
                                       a.ImplementedInterfaces.Any(i =>
                                                             searchingCondition.Any(include => i.Name.Contains(include))));

            foreach (var pipe in pipes)
            {
                var interfaces = pipe.GetInterfaces()
                                .Where(a => !searchingCondition.Any(e => a.Name.Contains(e)));

                foreach (var i in interfaces)
                    services.AddScoped(i, pipe.AsType());
            }

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}