using QuickGraph.Collections;
using QuickGraph.Concepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.GeneticAlgorithm.GraphRepresentation
{
    public class PredecessorRecorder
    {
        private Dictionary<IVertex,IEdge> m_Predecessors;
        private AdjGraph graph;
        private List<int> destination;
        private int start;

        private List<List<IEdge>> path;

        public PredecessorRecorder(AdjGraph graph, int start, List<int> destination)
        {
            m_Predecessors = new Dictionary<IVertex, IEdge>();
            this.graph = graph;
            this.path = new List<List<IEdge>>();
            this.destination = destination;
            this.start = start;

            if (destination.Contains(start))
            {
                destination.Remove(start);
            }
        }

        public PredecessorRecorder(AdjGraph graph)
        {
            m_Predecessors = new Dictionary<IVertex, IEdge>();
            this.graph = graph;
            this.path = new List<List<IEdge>>();
            this.start = 0;
            this.destination = new List<int>();
        }

        // the delegate
        public void RecordPredecessor(object sender, EdgeEventArgs args)
        {
            m_Predecessors[args.Edge.Target] = args.Edge;
        }

        public void findVertex(object sender, VertexEventArgs args)
        {

            //Console.WriteLine("Going to Vertex - " + graph.getTraversalHelper()[args.Vertex] );

            if (destination.Contains(graph.getID(args.Vertex)))
            {
                
                // Go through edge list and create the path
                //foreach (KeyValuePair<IVertex, IEdge> bee in m_Predecessors)
                //{
                //    edges.Add(graph.getEdge(bee.Value));
                //}
                
                // Start from last;
                IVertex v_start = graph.getVertex(start).getIVertex();
                IVertex v_dest = args.Vertex;
                List<IEdge> aux = new List<IEdge>();
                while(v_start != v_dest)
                {
                    //Backtrack the path and construct
                    IEdge aux_edge = m_Predecessors[v_dest];
                    aux.Insert(0, aux_edge);
                    v_dest = aux_edge.Source;
                }

               
                //Console.WriteLine("-------------- NEW --------------------");
                //Console.WriteLine("Start = " + start + " Destination = " + graph.getID(args.Vertex));
                //foreach (IEdge edge in aux)
                //{
                //    Console.WriteLine(graph.getEdge(edge).getVertexSource().getID() + " is Connected to " + graph.getEdge(edge).getVertexTarget().getID());
                //}
                path.Add(aux);

                // Remove that destination.
                destination.Remove(graph.getID(args.Vertex));
            }
        }

        public Dictionary<IVertex, IEdge> getDictionary()
        {
            return this.m_Predecessors;
        }

        public List<List<IEdge>> getPath()
        {
            return this.path;
        }

        public void setStartDestination(int start_id, List<int> destination_id, List<int> path_visited)
        {
            this.start = start_id;
            this.destination = new List<int>(destination_id);


            foreach (int i in path_visited)
            {
                destination.Remove(i);
            }

            if (destination.Contains(start))
            {
                destination.Remove(start);
            }

        }

        public void setStartDestination(int start_id, int destination_id)
        {
            this.start = start_id;
            this.destination = new List<int>();
            this.destination.Add(destination_id);
        }

        public int getStartID()
        {
            return this.start;
        }

        public List<int> getDestinationID()
        {
            return this.destination;
        }

    }
}
