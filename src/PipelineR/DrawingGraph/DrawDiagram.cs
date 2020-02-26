using System;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using MermaidNet;

namespace PipelineR.DrawingGraph
{
    public class DrawDiagram
    {
        private static string _projectPath = Environment.CurrentDirectory;
        private static string _applicationName = _projectPath.Split('\\').LastOrDefault();
        private static string _scriptsPath = Path.Combine(_projectPath, "wwwroot/scripts");
        private static string _cssPath = Path.Combine(_projectPath, "wwwroot/css");
        private static string _viewsPath = Path.Combine(_projectPath, "Views/DocsDiagrams");
        private static string _controllersPath = Path.Combine(_projectPath, "Controllers");
        private readonly Dictionary<object, DiagramModel> _details;
        private readonly DiagramBuilder _diagramBuilder;

        public DrawDiagram()
        {
            Setup();
            _details = new Dictionary<object, DiagramModel>();
            _diagramBuilder = new DiagramBuilder();
        }

        public void Configure(object key, string title, string description)
        {
            var model = GetModel(key);
            model.Title = title;
            model.Description = description;
        }

        private DiagramModel GetModel(object key)
        {
            if (!_details.TryGetValue(key, out var model))
            {
                model = new DiagramModel();
                _details.Add(key, model);
            }
            return model;
        }

        private Graph GetGraph(object key)
        {
            var model = GetModel(key);
            return model.Graph;
        }

        private string GetHTMLTitle(string title, Guid id) => $"<h2 id=\"{id.ToString()}\">{title}</h2>";
        private string GetHTMLDescription(string description) => $"<p>{description}</p>";
        private string GetHTMLDiagram(Graph graph) => $"<div class=\"mermaid\"> graph TD {_diagramBuilder.Build(graph).Replace("graph TD", "")} </div><div class=\"line\"></div>";
        private string GetHTMLMenu(string title, Guid id) => $"<li> <a href=\"#{id.ToString()}\">{title}</a> </li>";

        public void BuildDiagram()
        {
            var builderBody = new StringBuilder();
            var builderMenu = new StringBuilder();
            var descriptions = new Dictionary<string, string>();
            
            foreach (var detail in _details)
            {
                var diagramModel = detail.Value;

                if (!string.IsNullOrEmpty(diagramModel.Title))
                {
                    var id = Guid.NewGuid();
                    builderBody.Append(GetHTMLTitle(diagramModel.Title, id));
                    builderMenu.Append(GetHTMLMenu(diagramModel.Title, id));
                }

                if (!string.IsNullOrEmpty(diagramModel.Description))
                    builderBody.Append(GetHTMLDescription(diagramModel.Description));

                builderBody.Append(GetHTMLDiagram(diagramModel.Graph));
            }

            Build(builderBody.ToString(), builderMenu.ToString(), descriptions);
        }

        public void AddStep(object key, string nodeName)
        {
            var graph = GetGraph(key);

            var lastNode = graph.Nodes.LastOrDefault();

            var node = graph.AddNode(nodeName.Replace("Step", "").SplitCamelCase());
            node.Style = NodeStyle.Normal;
            node.Description = "Bla";

            if (lastNode != null)
                graph.Connect(lastNode, node);
        }

        public void AddFinnaly(object key, string nodeName)
        {
            var graph = GetGraph(key);

            var lastNode = graph.Nodes.LastOrDefault();

            var node = graph.AddNode(nodeName.Replace("Step", "").SplitCamelCase());
            node.Style = NodeStyle.Rounded;

            if (lastNode != null)
                graph.Connect(lastNode, node);
        }

        private void Setup()
        {
            if (!Directory.Exists(_scriptsPath))
                Directory.CreateDirectory(_scriptsPath);

            if (!Directory.Exists(_cssPath))
                Directory.CreateDirectory(_cssPath);

            if (!Directory.Exists(_viewsPath))
                Directory.CreateDirectory(_viewsPath);

            if (!Directory.Exists(_controllersPath))
                Directory.CreateDirectory(_controllersPath);

            ProccessController();
            CopyStream(LoadResource("PipelineR.DrawingGraph.Data.mermaid.min.js"), $"{_scriptsPath}/mermaid.min.js");
            CopyStream(LoadResource("PipelineR.DrawingGraph.Data.popper.min.js"), $"{_scriptsPath}/popper.min.js");
            CopyStream(LoadResource("PipelineR.DrawingGraph.Data.tippy.min.js"), $"{_scriptsPath}/tippy.min.js");
            CopyStream(LoadResource("PipelineR.DrawingGraph.Data.style.css"), $"{_cssPath}/style.css");
        }

        private void Build(string graph, string menu, IDictionary<string, string> descriptions)
        {
            var viewHtml = $"{_viewsPath}/index.cshtml";

            var template = LoadResourceString("PipelineR.DrawingGraph.Data.template.html");

            var descriptionsJson = JsonConvert.SerializeObject(descriptions, Formatting.None);

            var resultHtml = template.Replace("##GRAPH##", graph).Replace("##DESCRIPTIONS##", descriptionsJson).Replace("##MENU##", menu);

            var bytes = Encoding.ASCII.GetBytes(resultHtml);
            var stream = new MemoryStream(bytes);
            CopyStream(stream, viewHtml);
        }

        private void ProccessController()
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