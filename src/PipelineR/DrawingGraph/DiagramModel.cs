using MermaidNet;

namespace PipelineR.DrawingGraph
{
    public class DiagramModel
    {
        public DiagramModel()
        {
            Graph = new Graph();
        }

        public string Description { get; set; }
        public Graph Graph { get; set; }
        public string Title { get; set; }
    }
}