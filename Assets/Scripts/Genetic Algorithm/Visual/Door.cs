using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Visual
{
    public class Door
    {

        private int ID;
        private Tile[] adjacent_tiles;

        public Door(int ID)
        {
            this.ID = ID;
            this.adjacent_tiles = new Tile[2];
        }

        public Door(int ID, Tile[] adjacent_tiles)
        {
            this.ID = ID;
            this.adjacent_tiles = adjacent_tiles;
        }

        public void setDoor(Tile tile1, Tile tile2)
        {
            adjacent_tiles[0] = tile1;
            adjacent_tiles[1] = tile2;
        }

        public void setAdj1(Tile tile1)
        {
            adjacent_tiles[0] = tile1;
        }

        public void setAdj2(Tile tile2)
        {
            adjacent_tiles[1] = tile2;
        }

        public bool areConnected(Tile tile1, Tile tile2)
        {
            return ( (tile1.getID() == adjacent_tiles[0].getID() && tile2.getID() == adjacent_tiles[1].getID() ) 
                || (tile1.getID() == adjacent_tiles[1].getID() && tile2.getID() == adjacent_tiles[0].getID()) );
        }

        // Returns the other half of the door. Null if there is no door associated to t.
        public Tile getOpposite(Tile t)
        {
            if (t.getDoor() == null)
            {
                return null;
            }
            else
            {
                if (this.getConnectingTiles()[0].getID() == t.getID())
                {
                    return getConnectingTiles()[1];
                }
                else if (this.getConnectingTiles()[1].getID() == t.getID())
                {
                    return getConnectingTiles()[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public Tile[] getConnectingTiles()
        {
            return adjacent_tiles;
        }

        public int getID()
        {
            return ID;
        }

        public void setID(int ID)
        {
            this.ID = ID;
        }
    }
}
