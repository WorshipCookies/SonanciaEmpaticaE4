using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Visual
{
    public class Room
    {

        private int ID;
        private List<Tile> tiles;
        private List<Door> doors;


        public Room(int ID)
        {
            this.ID = ID;
            this.tiles = new List<Tile>();
            this.doors = new List<Door>();
        }

        public Room(int ID, List<Tile> tiles)
        {
            this.ID = ID;
            this.tiles = tiles;
            this.doors = new List<Door>();
        }

        public Room(int ID, List<Tile> tiles, List<Door> doors)
        {
            this.ID = ID;
            this.tiles = tiles;
            this.doors = doors;
        }

        public void addTile(Tile tile)
        {
            this.tiles.Add(tile);
        }

        public void removeTile(Tile tile)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].getID() == tile.getID())
                {
                    tiles.RemoveAt(i);
                    return;
                }
            }
        }

        public void addDoor(Door door)
        {
            this.doors.Add(door);
        }

        public void removeDoor(Door door)
        {
            for (int i = 0; i < doors.Count; i++)
            {
                if (this.doors[i].getID() == door.getID())
                {
                    this.doors.RemoveAt(i);
                    return;
                }
            }
        }

        public int getID()
        {
            return this.ID;
        }

        public void setID(int new_id)
        {
            this.ID = new_id;
        }

        public List<Door> getDoors()
        {
            return this.doors;
        }

        public List<Tile> getTiles()
        {
            return this.tiles;
        }

        public List<Room> getRoomNeighbours()
        {
            List<Room> neighbourRoomID = new List<Room>();

            foreach(Door d in doors)
            {
                Tile[] tiles = d.getConnectingTiles();
                if(tiles[0].getRoom().getID() == this.ID)
                {
                    neighbourRoomID.Add(tiles[1].getRoom());
                }
                else
                {
                    neighbourRoomID.Add(tiles[0].getRoom());
                }
            }
            return neighbourRoomID;
        }

        public override bool Equals(object obj)
        {
            Room t2 = (Room)obj;
            return this.ID == t2.getID();
        }

    }
}
