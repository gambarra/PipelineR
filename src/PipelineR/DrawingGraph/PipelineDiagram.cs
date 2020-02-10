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
    public class PipelineDiagram
    {
        private static string _projectPath = Environment.CurrentDirectory;
        private static string _applicationName = _projectPath.Split('\\').LastOrDefault();
        private static string _scriptsPath = Path.Combine(_projectPath, "wwwroot/scripts");
        private static string _viewsPath = Path.Combine(_projectPath, "Views/DocsDiagrams");
        private static string _controllersPath = Path.Combine(_projectPath, "Controllers");
        //private List<(Graph graph, IDictionary<string, string> descriptions)> _graphs;
        private Dictionary<object, Graph> _diagrams;

        public PipelineDiagram()
        {
            Setup();
            //_graphs = new List<(Graph graph, IDictionary<string, string> descriptions)>();
            _diagrams = new Dictionary<object, Graph>();
        }

        private Graph GetGraph(object key)
        {
            if (!_diagrams.TryGetValue(key, out var graph))
            {
                graph = new Graph();
                _diagrams.Add(key, graph);
            }
            return graph;
        }

        //private Node LastNode(object key)
        //{
        //    var graph = GetGraph(key);
        //    var node = graph.Nodes.LastOrDefault();

        //    if (node == null)


        //}

        public void BuildDiagram(object key)
        {
            var diagram = new DiagramBuilder();
            var graph = GetGraph(key);
            var descriptions = diagram.GetDescriptions(graph);

            var result = diagram.Build(graph);

            Build(result, descriptions);
        }


        public void AddStep(object key, string nodeName)
        {
            var graph = GetGraph(key);

            var lastNode = graph.Nodes.LastOrDefault();

            var node = graph.AddNode(nodeName);
            node.Style = NodeStyle.Normal;

            if (lastNode != null)
            {
                graph.Connect(lastNode, node);
            }
        }

        //public void AddCondition()
        //{

        //}

        //public void AddGraph(Graph graph, IDictionary<string, string> descriptions)
        //{
        //    //_graphs.Add((graph, descriptions));
        //}

        private void Setup()
        {
            if (!Directory.Exists(_scriptsPath))
                Directory.CreateDirectory(_scriptsPath);

            if (!Directory.Exists(_viewsPath))
                Directory.CreateDirectory(_viewsPath);

            if (!Directory.Exists(_controllersPath))
                Directory.CreateDirectory(_controllersPath);
        }

        private void Build(string graph, IDictionary<string, string> descriptions)
        {
            ProccessController();
            CopyStream(LoadResource("PipelineR.DrawingGraph.Data.mermaid.min.js"), $"{_scriptsPath}/mermaid.min.js");
            CopyStream(LoadResource("PipelineR.DrawingGraph.Data.popper.min.js"), $"{_scriptsPath}/popper.min.js");
            CopyStream(LoadResource("PipelineR.DrawingGraph.Data.tippy.min.js"), $"{_scriptsPath}/tippy.min.js");
            ProccessTemplate(graph, descriptions);
        }

        private void ProccessTemplate(string graph, IDictionary<string, string> descriptions)
        {
            var viewHtml = $"{_viewsPath}/index.cshtml";

            var template = LoadResourceString("PipelineR.DrawingGraph.Data.template.html");

            var descriptionsJson = JsonConvert.SerializeObject(descriptions, Formatting.None);

            var resultHtml = template.Replace("##GRAPH##", graph).Replace("##DESCRIPTIONS##", descriptionsJson);

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