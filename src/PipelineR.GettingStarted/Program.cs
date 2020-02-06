using AspNetScaffolding;
using AspNetScaffolding.Models;
using MermaidNet;
using Microsoft.Extensions.DependencyInjection;
using PipelineR.DrawingGraph;
using System.Reflection;

namespace PipelineR.GettingStarted
{
    public class Program
    {
        public static void Main(string[] _)
        {
            PipelineDiagram.Setup();

            var config = new ApiBasicConfiguration
            {
                ApiName = "PipelineR Getting Started",
                ApiPort = 8700,
                EnvironmentVariablesPrefix = "PipelineR_",
                ConfigureServices = ConfigureServices,
                AutoRegisterAssemblies = new Assembly[]
                { Assembly.GetExecutingAssembly() }
            };

            var graph = new Graph();

            var node = graph.AddNode("Testing");
            var node2 = graph.AddNode("Testing2");

            graph.Connect(node, node2);

            var diagram = new DiagramBuilder();
            var result = diagram.Build(graph);

            var draw = new PipelineDiagram();
            draw.Build(result, null);

            Api.Run(config);
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();            

            Startup.AddPipelines(services);
        }
    }
}