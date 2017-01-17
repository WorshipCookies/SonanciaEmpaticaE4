using QuickGraph.Concepts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.GeneticAlgorithm.GraphRepresentation
{
    public class Vertex : IVertex
    {

        private int id; // This ID is extremely important, as it maps to the ID of the Tile Map. Do NOT Lose
        private int vertex_type; // In the furture it might be important to have items, or such on vertices hence the "type". For now 0 = Normal.
        private IVertex vertex;

        public Vertex(int i, IVertex vertex, int type)
            : base()
        {
            this.id = i;
            this.vertex_type = type;
            this.vertex = vertex;
        }

        public Vertex(int i, IVertex vertex) 
            : base()
        {
            this.id = i;
            this.vertex_type = 0;
            this.vertex = vertex;
        }

        public int getID()
        {
            return this.id;
        }

        public int getType()
        {
            return this.vertex_type;
        }

        public IVertex getIVertex()
        {
            return this.vertex;
        }

        public void setType(int type)
        {
            this.vertex_type = type;
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
