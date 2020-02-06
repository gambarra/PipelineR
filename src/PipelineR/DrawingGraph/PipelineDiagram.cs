using System;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace PipelineR.DrawingGraph
{
    public class PipelineDiagram
    {
        private static string _projectPath = Environment.CurrentDirectory;
        private static string _applicationName = _projectPath.Split('\\').LastOrDefault();
        private static string _scriptsPath = Path.Combine(_projectPath, "wwwroot/scripts");
        private static string _viewsPath = Path.Combine(_projectPath, "Views/DocsDiagrams");
        private static string _controllersPath = Path.Combine(_projectPath, "Controllers");

        public PipelineDiagram()
        {
            
        }

        public static void Setup()
        {
            if (!Directory.Exists(_scriptsPath))
                Directory.CreateDirectory(_scriptsPath);

            if (!Directory.Exists(_viewsPath))
                Directory.CreateDirectory(_viewsPath);

            if (!Directory.Exists(_controllersPath))
                Directory.CreateDirectory(_controllersPath);
        }

        public void Build(string graph, IDictionary<string, string> descriptions)
        {
            ProccessController();
            CopyStream(LoadResource("PipelineR.DrawingGraph.Data.mermaid.min.js"), $"{_scriptsPath}/mermaid.min.js");
            CopyStream(LoadResource("PipelineR.DrawingGraph.Data.popper.min.js"), $"{_scriptsPath}/popper.min.js");
            CopyStream(LoadResource("PipelineR.DrawingGraph.Data.tippy.min.js"), $"{_scriptsPath}/tippy.min.js");
            ProccessTemplate(graph, descriptions);
        }

        public void ProccessTemplate(string graph, IDictionary<string, string> descriptions)
        {
            var viewHtml = $"{_viewsPath}/index.cshtml";

            var template = LoadResourceString("PipelineR.DrawingGraph.Data.template.html");

            var descriptionsJson = JsonConvert.SerializeObject(descriptions, Formatting.None);

            var resultHtml = template.Replace("##GRAPH##", graph).Replace("##DESCRIPTIONS##", descriptionsJson);

            var bytes = Encoding.ASCII.GetBytes(resultHtml);
            var stream = new MemoryStream(bytes);
            CopyStream(stream, viewHtml);
        }

        public void ProccessController()
        {
            var docsDiagramsController = $"{_controllersPath}/DocsDiagramsController.cs";

            if (File.Exists(docsDiagramsController))
                return;

            var template = LoadResourceString("PipelineR.DrawingGraph.Data.DocsDiagramsController.txt");

            var controller = template.Replace("##ApplicationName##", _applicationName);

            var bytes = Encoding.ASCII.GetBytes(controller);
            var stream = new MemoryStream(bytes);
            CopyStream(stream, docsDiagramsController);
        }

        private void CopyStream(Stream stream, string destPath)
        {
            if (File.Exists(destPath) && !destPath.Contains("index.cshtml"))
                return;

            using (var fileStream = new FileStream(destPath, FileMode.OpenOrCreate, FileAccess.Write))
                stream.CopyTo(fileStream);
        }

        private Stream LoadResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(resourceName);
        }

        private string LoadResourceString(string resourceName)
        {
            using (Stream stream = LoadResource(resourceName))
            using (StreamReader reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
    }
}