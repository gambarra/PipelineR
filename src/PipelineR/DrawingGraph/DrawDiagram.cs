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
        private static string _modelValidator = "Model Validator";
        private static string _projectPath = Environment.CurrentDirectory;
        private static string _scriptsPath = Path.Combine(_projectPath, "wwwroot/scripts");
        private static string _cssPath = Path.Combine(_projectPath, "wwwroot/css");
        private static string _viewsPath = Path.Combine(_projectPath, "Views/DocsDiagrams");
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

        private string GetHTMLBodyBase()
        {
            return "<div class=\"row\"> " +
            "	<div class=\"col-sm-8\" style=\"padding-left:50px; \">  " +
            "		##TITLE##	" +
            "		##DESCRIPTION##	" +
            "		<div class=\"mermaid\">	" +
            "			##GRAPH##	" +
            "		</div>	" +
            "	</div>	" +
            "	<div class=\"col-sm-4\">	" +
            "   <p class=\"codeTitle\">Request Model</p> " +
            "		##SCRIPTREQUESTMODEL##	" +
            "	</div>	" +
            "	<div class=\"line\"></div>	" +
            "</div>	";
        }

        private string GetScriptToSerializeModel(Guid id, string value) => $"$('#{id.ToString()}').jsonViewer({value}, {{collapsed: true, rootCollapsable: false, withQuotes: true, withLinks: false}});";

        private string GetHTMLModelSerialized(Guid id) => $"<pre id=\"{id.ToString()}\" class=\"code\"></pre>";
        private string GetHTMLTitle(string title, Guid id) => $"<h2 id=\"{id.ToString()}\">{title}</h2>";
        private string GetHTMLDescription(string description) => $"<p>{description}</p>";
        private string GetHTMLDiagram(Graph graph) => $"{_diagramBuilder.Build(graph)}";
        private string GetHTMLMenu(string title, Guid id) => $"<li> <a href=\"#{id.ToString()}\">{title}</a> </li>";

        public void BuildDiagram()
        {
            var builderBody = new StringBuilder();
            var builderMenu = new StringBuilder();
            var builderScripts = new StringBuilder();
            var descriptions = new Dictionary<string, string>();
            
            foreach (var detail in _details)
            {
                var htmlBase = GetHTMLBodyBase();

                var diagramModel = detail.Value;
                var navigationId = Guid.NewGuid();
                var serializedModelId = Guid.NewGuid();

                htmlBase = htmlBase
                                .Replace("##TITLE##", GetHTMLTitle(diagramModel.Title, navigationId))
                                .Replace("##DESCRIPTION##", GetHTMLDescription(diagramModel.Description))
                                .Replace("##SCRIPTREQUESTMODEL##", GetHTMLModelSerialized(serializedModelId))
                                .Replace("##GRAPH##", GetHTMLDiagram(diagramModel.Graph));


                if (!string.IsNullOrEmpty(diagramModel.Title))
                    builderMenu.AppendLine(GetHTMLMenu(diagramModel.Title, navigationId));

                builderBody.AppendLine(htmlBase);
                builderScripts.AppendLine(GetScriptToSerializeModel(serializedModelId, JsonConvert.SerializeObject(diagramModel.Request)));
            }

            Build(builderBody.ToString(), builderMenu.ToString(), builderScripts.ToString(), descriptions);
        }

        public void AddRequest(object ctx)
        {
            _details.LastOrDefault().Value.Request = ctx;
        }

        public void AddStep(object key, string nodeName)
        {
            var graph = GetGraph(key);

            var node = new Node(Guid.NewGuid().ToString())
            {
                Name = nodeName.Replace("Step", "").SplitCamelCase(),
                Style = NodeStyle.Normal
            };

            Connect(graph, node);
        }

        public void Connect(Graph graph, Node node)
        {
            if (node.Style == NodeStyle.Normal || node.Style == NodeStyle.Rounded)
            {
                var last = graph.Nodes.LastOrDefault();

                if (last == null)
                {
                    graph.AddNode(node);
                    return;
                }

                if (last.Name == _modelValidator)
                {
                    var end = new Node(Guid.NewGuid().ToString())
                    {
                        Name = "End".SplitCamelCase(),
                        Style = NodeStyle.Circle
                    };

                    graph.Connect(last, end, text: "Invalid");
                    graph.AddNode(end);

                    graph.Connect(last, node, text: "Valid");
                    graph.AddNode(node);

                    return;
                }

                var penult = graph.Nodes.PenultOrDefault();

                if (penult != null && penult.Style == NodeStyle.Rhombus && penult.Name != _modelValidator)
                {
                    graph.Connect(penult, node, text: "False");
                }

                graph.Connect(last, node);
                graph.AddNode(node);
            }

            if (node.Style == NodeStyle.Rhombus)
            {
                var last = graph.Nodes.LastOrDefault();

                if (last == null)
                {
                    graph.AddNode(node);
                    return;
                }

                var relation = graph.Relations.FirstOrDefault(r => r.B == last);

                if (relation == null)
                {
                    graph.Connect(last, node);  
                    return;
                }
                else
                {
                    graph.Relations.Remove(relation);
                }

                var penult = graph.Nodes.PenultOrDefault();

                graph.Connect(penult, node, Direction.None);
                graph.Connect(node, last, Direction.AB, text: "True");

                graph.Nodes.Remove(last);
                graph.AddNode(node);
                graph.AddNode(last);
            }
        }

        public void AddFinnaly(object key, string nodeName)
        {
            var graph = GetGraph(key);

            var node = new Node(Guid.NewGuid().ToString())
            {
                Name = nodeName.Replace("Step", "").SplitCamelCase(),
                Style = NodeStyle.Rounded
            };

            Connect(graph, node);
        }

        public void AddCondition(object key, string nodeName)
        {
            var graph = GetGraph(key);

            var node = new Node(Guid.NewGuid().ToString())
            {
                Name = $"\"{nodeName.Replace("Condition", "")}\"",
                Style = NodeStyle.Rhombus
            };

            Connect(graph, node);
        }

        public void AddValidator(object key)
        {
            var graph = GetGraph(key);

            var node = graph.AddNode(_modelValidator);
            node.Style = NodeStyle.Rhombus;
        }

        public void AddEnd(object key)
        {
            var graph = GetGraph(key);

            var endNode = graph.Nodes.FirstOrDefault(n => n.Name == "End");

            if (endNode == null)
            {
                endNode = new Node(Guid.NewGuid().ToString())
                {
                    Name = "End".SplitCamelCase(),
                    Style = NodeStyle.Circle
                };
            }

            var last = graph.Nodes.LastOrDefault();

            graph.Connect(last, endNode);
            graph.AddNode(endNode);
        }

        private void Setup()
        {
            if (!Directory.Exists(_scriptsPath))
                Directory.CreateDirectory(_scriptsPath);

            if (!Directory.Exists(_cssPath))
                Directory.CreateDirectory(_cssPath);

            if (!Directory.Exists(_viewsPath))
                Directory.CreateDirectory(_viewsPath);

            CopyStream(LoadResource("PipelineR.DrawingGraph.Data.mermaid.min.js"), $"{_scriptsPath}/mermaid.min.js");
            CopyStream(LoadResource("PipelineR.DrawingGraph.Data.jquery.json-viewer.js"), $"{_scriptsPath}/jquery.json-viewer.js");
            CopyStream(LoadResource("PipelineR.DrawingGraph.Data.style.css"), $"{_cssPath}/style.css");
            CopyStream(LoadResource("PipelineR.DrawingGraph.Data.jquery.json-viewer.css"), $"{_cssPath}/jquery.json-viewer.css");
        }

        private void Build(string graph, string menu, string scripts, IDictionary<string, string> descriptions)
        {
            var viewHtmlPath = $"{_viewsPath}/index.cshtml";

            if (File.Exists(viewHtmlPath))
                File.Delete(viewHtmlPath);

            var template = LoadResourceString("PipelineR.DrawingGraph.Data.template.html");

            var descriptionsJson = JsonConvert.SerializeObject(descriptions, Formatting.None);

            var resultHtml = template
                                .Replace("##BODY##", graph)
                                .Replace("##DESCRIPTIONS##", descriptionsJson)
                                .Replace("##MENU##", menu)
                                .Replace("##SCRIPTS##", scripts);

            var bytes = Encoding.ASCII.GetBytes(resultHtml);
            var stream = new MemoryStream(bytes);
            CopyStream(stream, viewHtmlPath);
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