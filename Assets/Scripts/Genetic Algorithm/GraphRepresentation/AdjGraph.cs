using QuickGraph.Concepts;
using QuickGraph.Representations;
using QuickGraph.Providers;
using QuickGraph.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.GeneticAlgorithm.GraphRepresentation
{
    public class AdjGraph : AdjacencyGraph
    {

        private Dictionary<int, Vertex> vertexes;
        private Dictionary<IEdge, Edge> edges;
        private EdgeDoubleDictionary dictionary_value; // REMOVED -- Might Cause Errors! 
        private Dictionary<IVertex, int> graph_traversal_helper;

        public AdjGraph() 
            : base(new VertexAndEdgeProvider(), true)
        {
            vertexes = new Dictionary<int, Vertex>();
            edges = new Dictionary<IEdge, Edge>();
            dictionary_value = new EdgeDoubleDictionary();
            graph_traversal_helper = new Dictionary<IVertex, int>();
            //Console.WriteLine("Is Directed? - " + this.IsDirected);
        }

        private AdjGraph(AdjGraph to_clone)
            : base(new VertexAndEdgeProvider(), true)
        {
            this.vertexes = new Dictionary<int, Vertex>(to_clone.vertexes);
            this.edges = new Dictionary<IEdge, Edge>(to_clone.edges);
            this.graph_traversal_helper = new Dictionary<IVertex, int>(to_clone.graph_traversal_helper);
            this.dictionary_value = to_clone.dictionary_value;
        }

        public void addVertex(int i)
        {
            if (!vertexes.ContainsKey(i))
            {
                IVertex vert = AddVertex();
                vertexes.Add(i, new Vertex(i, vert));
                graph_traversal_helper.Add(vert, i);
            } 
        }

        public void addEdge(Edge edge, int type)
        {
            //if (!ContainsEdge(edge))
            //{
                IEdge iedge = AddEdge(edge.Source, edge.Target);
                edges.Add(iedge, new Edge(type, edge.getVertexSource(), edge.getVertexTarget()));
                dictionary_value.Add(iedge, 1.0);
                
            //}
        }

        public void addEdge(Vertex source, Vertex target, int type)
        {
            //if ( !ContainsEdge(source.getIVertex(), target.getIVertex()) && !ContainsEdge(target.getIVertex(), source.getIVertex()) ) 
            //{
                IEdge iedge = AddEdge(source.getIVertex(), target.getIVertex());
                edges.Add(iedge, new Edge(type, source, target));
                dictionary_value.Add(iedge, 1.0);
            //}   
        }

        public EdgeDoubleDictionary getDictionaryValue()
        {
            return this.dictionary_value;
        }

        public bool isDoor(Vertex source, Vertex target)
        {
            return edges[this.Provider.ProvideEdge(source, target)].getType() == 0;
        }

        public bool isDoor(IEdge edge)
        {
            return edges[edge].getType() == 1;
        }

        public int getType(IEdge edge)
        {
            return edges[edge].getType();
        }

        public Vertex getVertex(int ID)
        {
            return vertexes[ID];
        }

        public int getID(IVertex vert)
        {
            return graph_traversal_helper[vert];
        }

        public Dictionary<int, Vertex> getVertices()
        {
            return this.vertexes;
        }

        public Edge getEdge(IEdge edge)
        {
            return edges[edge];
        }

        public Dictionary<IVertex, int> getTraversalHelper()
        {
            return this.graph_traversal_helper;
        }

        public string printConnections()
        {
            string str = "";
            str += " ---------------------- Printing All Connections of Graph -------------------- \n";
            foreach (KeyValuePair<IEdge, Edge> edge in edges)
            {
                str += edge.Value.getVertexSource().getID() + " is Connected to " + edge.Value.getVertexTarget().getID() + " and is Type " + edge.Value.getType() + "\n";
            }

            str += " --------------------------- END -------------------- \n";
            return str;
        }

        public AdjGraph CloneGraph()
        {
            return new AdjGraph(this);
        }

    }
}
