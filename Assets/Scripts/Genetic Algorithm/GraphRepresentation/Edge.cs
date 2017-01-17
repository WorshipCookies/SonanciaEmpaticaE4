using QuickGraph.Concepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.GeneticAlgorithm.GraphRepresentation
{
    public class Edge: IEdge
    {
        private int type_connection; // For now we only have two types of connections: 
                                     // 0 = Room Connection and 1 = Door Connection. In the future we might want different types of connections... who knows?

        private Vertex vert_source;
        private Vertex vert_target;

        public Edge(int type, Vertex source, Vertex target) 
            : base()
        {
            this.type_connection = type;
            this.vert_source = source;
            this.vert_target = target;
        }

        public Edge(Vertex source, Vertex target)
            : base()
        {
            this.type_connection = 0;
            this.vert_source = source;
            this.vert_target = target;
        }

        public void setType(int type)
        {
            this.type_connection = type;
        }

        public IVertex Source
        {
            get { return this.Source; }
        }

        public IVertex Target
        {
            get { return this.Target; }
        }

        public Vertex getVertexSource()
        {
            return this.vert_source;
        }

        public Vertex getVertexTarget()
        {
            return this.vert_target;
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public int getType()
        {
            return type_connection;
        }
    }
}
