using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Visual
{
    /*
     * The tile class contains both visual and game information. Its ID states its position in the map
     * */
    public class Tile
    {

        public static int TILE_SIZE = 50;

        private int ID;
        private float x1;
        private float y1;
        private float x2;
        private float y2;
        private bool is_wall;

        // Testing might need to remove it - Phil 27/1/2015
        private Room room;
        private Door door;

        public Tile(int ID, float x1, float y1, float x2, float y2)
        {
            this.ID = ID;
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;

            this.is_wall = false;

            this.room = null;
            this.door = null;
        }

        public int getID()
        {
            return this.ID;
        }

        public float[] getCoords()
        {
            float[] coords = {x1, y1, x2, y2};
            return coords;
        }
        
        public bool isWall()
        {
            return is_wall;
        }

        public Room getRoom()
        {
            return room;
        }

        public Door getDoor()
        {
            return door;
        }

        public void setRoom(Room room)
        {
            this.room = room;
        }

        public void setDoor(Door door)
        {
            this.door = door;
        }

        public static List<Tile> createTileMap(int height, int width)
        {
            List<Tile> tile_map = new List<Tile>();
            int id_counter = 0;

            //Given the size of the Tile Map, the system will try and adapt to the closest size if not perfect.
            height = height - (height % TILE_SIZE);
            width = width - (width % TILE_SIZE);

            // How many tiles can fit the height and width of pixels. This is important for tile navigation especially.
            Map.HEIGHT_TILE_SIZE = height / TILE_SIZE;
            Map.WIDTH_TILE_SIZE = width / TILE_SIZE;

            //Lets create the tiles
            for (int height_count = 0; height_count < height; height_count += TILE_SIZE)
            {
                for (int width_count = 0; width_count < width; width_count += TILE_SIZE)
                {
                    tile_map.Add(new Tile(id_counter, height_count, width_count, height_count + TILE_SIZE, width_count + TILE_SIZE));
                    id_counter++;
                }
            }
            return tile_map;
        }

        // Resets the room and door objects of this tile
        public void resetTile()
        {
            this.room = null;
            this.door = null;
        }

        public override bool Equals(object obj)
        {
            Tile t2 = (Tile)obj;
            return this.ID == t2.getID();
        }

    }
}
