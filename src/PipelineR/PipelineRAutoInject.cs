using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using PipelineR.DrawingGraph;
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
            var assemblies = GetAssemblies();

            var types = assemblies
                        .SelectMany(a => a.GetTypes())
                        .Select(a => a.GetTypeInfo());

            InjectContexts(services, types);
            InjectPipes(services, types);

            //services.AddScoped<IPipelineStarting<BaseContext>, PipelineStarting<BaseContext>>();

            Type baseType = typeof(IPipelineStarting<>);

            var contexts = types
                                .Where(a => a.IsClass && a.BaseType == typeof(BaseContext));

            foreach (var context in contexts)
            {
                Type[] typeArgs = { context.AsType() };
                var typeWithGeneric = baseType.MakeGenericType(typeArgs);

                Type impleBaseType = typeof(PipelineStartingDiagram<>);
                var impleTypeWithGeneric = impleBaseType.MakeGenericType(typeArgs);

                services.AddScoped(typeWithGeneric, impleTypeWithGeneric);
            }
            

            ServiceProvider = services.BuildServiceProvider();

            LoadingDiagrams(services, types);
            //BUILD DIAGRAM            
        }

        private static void LoadingDiagrams(IServiceCollection services, IEnumerable<TypeInfo> types)
        {
            var searchingCondition = new[] { "IWorkflow" };

            var pipes = types
                        .Where(a => a.IsClass &&
                                       !searchingCondition.Any(exclude => a.Name.Contains(exclude)) &&
                                       a.ImplementedInterfaces.Any(i =>
                                                             searchingCondition.Any(include => i.Name.Contains(include))));

            foreach (var pipe in pipes)
            {
                var interfaces = pipe.GetInterfaces()
                                .Where(a => !searchingCondition.Any(e => a.Name.Contains(e)));

                foreach (var @interface in interfaces)
                {
                    var inst = ServiceProvider.GetService(@interface);
                    var methods = inst.GetType().GetMethods();

                    foreach (var method in methods.Where(m => m.ReturnType.Name.Contains("StepHandlerResult")))
                    {
                        var parameter = method.GetParameters().FirstOrDefault();
                        var paramsCtor = parameter.ParameterType.GetConstructors().FirstOrDefault()?.GetParameters();

                        var parameters = new List<object>();

                        foreach (var paramCtor in paramsCtor)
                            parameters.Add(Activator.CreateInstance(paramCtor.ParameterType));
                        
                        var paramInstance = Activator.CreateInstance(parameter.ParameterType, parameters.ToArray());

                        method.Invoke(inst, new object[] { paramInstance });
                    }
                }
            }
        }


        private static void InjectPipes(IServiceCollection services, IEnumerable<TypeInfo> types)
        {
            var searchingCondition = new[] { "StepHandler", "ICondition", "IWorkflow" };

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
        }

        private static void InjectContexts(IServiceCollection services, IEnumerable<TypeInfo> typeInfos)
        {
            var contexts = typeInfos
                                .Where(a => a.IsClass && a.BaseType == typeof(BaseContext));

            foreach (var context in contexts)
                services.AddScoped(p => Activator.CreateInstance(context));
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            var dependencies = DependencyContext.Default.RuntimeLibraries.Where(p =>
                p.Type.Equals("Project", StringComparison.CurrentCultureIgnoreCase));

            foreach (var library in dependencies)
            {
                var name = new AssemblyName(library.Name);
                var assembly = Assembly.Load(name);
                yield return assembly;
            }
        }
    }
}