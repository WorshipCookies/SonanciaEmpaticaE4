using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.Util;
using ProjectMaze.GeneticAlgorithm;
using ProjectMaze.Visual.Spawn;

namespace ProjectMaze.Visual
{
    /* This class contains the full information of the map, number of tiles, doors and rooms. 
     * This class encompases the visual and data structure elements that constitute a map */
    public class Map
    {
        
        private int ID;
        private List<Room> rooms;
        private List<Tile> tiles;
        private List<Door> doors;

        private List<SpawnPoint> spawns; // Item and Enemy Placement can be fixed or not!

        private int height;
        private int width;

        public static int HEIGHT_TILE_SIZE = 0;
        public static int WIDTH_TILE_SIZE = 0;
        public static int MIN_ROOM_SIZE = 5;

        public static int MAX_SPAWN_NUM = 5;
        public static int MIN_SPAWN_NUM = 2; // THIS NEEDS TO BE ALWAYS ABOVE 2! DO NOT CHANGE!!

        public static double CHANGE_ROOM_VAR = 0.01;

        private Map(int height, int width, bool is_static)
        {
            this.height = height;
            this.width = width;
            tiles = Tile.createTileMap(height, width);

            if (is_static)
            {
                initializeStaticMap();
                //initializeStaticMap2();
                //initializeStaticMap3();
            }
            else
            {
                initializeMap();
            }
            
        }

        // Usable for cloning ONLY!!! Empty Map.
        private Map(Map map)
        {
            this.height = map.height;
            this.width = map.width;
            this.rooms = new List<Room>(map.getRooms());
            this.tiles = new List<Tile>(map.getTiles());
            this.doors = new List<Door>(map.getDoors());
            this.spawns = new List<SpawnPoint>(map.getSpawnPoints());
        }

        public static Map mapFactory(int height, int width)
        {
            return new Map(height, width, false);
        }

        public static Map mapFactoryStatic(int height, int width)
        {
            return new Map(height, width, true);
        }

        public Map(int height, int width, List<Room> rooms, List<Tile> tiles, List<Door> doors, List<SpawnPoint> spawns)
        {
            this.height = height;
            this.width = width;
            this.rooms = rooms;
            this.tiles = tiles;
            this.doors = doors;
            this.spawns = spawns;
        }

        // Creates a simple boring static map 
        private void initializeStaticMap()
        {
            // Initialize the room list.
            this.rooms = new List<Room>();

            for (int i = 0; i < 4; i++)
            {
                this.rooms.Add(new Room(i));
            }

            // Create the Rooms
            int id_counter = 0;
            for (int i = 0; i < HEIGHT_TILE_SIZE; i++)
            {
                for (int j = 0; j < WIDTH_TILE_SIZE; j++)
                {
                    if (i < HEIGHT_TILE_SIZE / 2 && j < WIDTH_TILE_SIZE / 2)
                    {
                        tiles[id_counter].setRoom(rooms[0]);
                        rooms[0].addTile(tiles[id_counter]);
                    }
                    else if (i < HEIGHT_TILE_SIZE / 2 && j >= WIDTH_TILE_SIZE / 2)
                    {
                        tiles[id_counter].setRoom(rooms[1]);
                        rooms[1].addTile(tiles[id_counter]);
                    }
                    else if (i >= HEIGHT_TILE_SIZE / 2 && j < WIDTH_TILE_SIZE / 2)
                    {
                        tiles[id_counter].setRoom(rooms[2]);
                        rooms[2].addTile(tiles[id_counter]);
                    }
                    else if (i >= HEIGHT_TILE_SIZE / 2 && j >= WIDTH_TILE_SIZE / 2)
                    {
                        tiles[id_counter].setRoom(rooms[3]);
                        rooms[3].addTile(tiles[id_counter]);
                    }
                    id_counter++;
                }
            }

            // Create Doors
            this.doors = new List<Door>();

            // Door 0 to 1
            List<int[]> temp = getAdjacentTileFromRoomToRoom(this.rooms[0], this.rooms[1]);
            int[] door = temp[temp.Count/2];
            Tile[] tile_door1 = {getTileByID(door[0]), getTileByID(door[1])};
            this.doors.Add(new Door(0, tile_door1));

            // Door 0 to 2
            temp = getAdjacentTileFromRoomToRoom(this.rooms[0], this.rooms[2]);
            door = temp[temp.Count/2];
            Tile[] tile_door2 = { getTileByID(door[0]), getTileByID(door[1]) };
            this.doors.Add(new Door(0, tile_door2));

            // Door 1 to 3
            temp = getAdjacentTileFromRoomToRoom(this.rooms[1], this.rooms[3]);
            door = temp[temp.Count / 2];
            Tile[] tile_door3 = { getTileByID(door[0]), getTileByID(door[1]) };
            this.doors.Add(new Door(0, tile_door3));

            // Door 2 to 3
            temp = getAdjacentTileFromRoomToRoom(this.rooms[2], this.rooms[3]);
            door = temp[temp.Count/2];
            Tile[] tile_door4 = { getTileByID(door[0]), getTileByID(door[1]) };
            this.doors.Add(new Door(0, tile_door4));


            // Spawn Point Initialization -- Lets create 4 spawn points -- 2 monsters and 2 items
            this.spawns = new List<SpawnPoint>();

            this.spawns.Add(SpawnPoint.SpawnPointFactory(0, this.rooms[3].getID()));
            int tiles_count = getRoomByID(spawns[0].getRoom()).getTiles().Count;
            spawns[0].setTile(getRoomByID(spawns[0].getRoom()).getTiles()[(int)tiles_count / 2]);

            this.spawns.Add(SpawnPoint.SpawnPointFactory(0, this.rooms[1].getID()));
            tiles_count = getRoomByID(spawns[1].getRoom()).getTiles().Count;
            spawns[1].setTile(getRoomByID(spawns[1].getRoom()).getTiles()[(int)tiles_count / 2]);

            //this.spawns.Add(SpawnPoint.SpawnPointFactory(1, this.rooms[2].getID()));
            //this.spawns.Add(SpawnPoint.SpawnPointFactory(1, this.rooms[3].getID()));
        }

        private void initializeStaticMap2()
        {
            // Initialize the room list.
            this.rooms = new List<Room>();

            for (int i = 0; i < 2; i++)
            {
                this.rooms.Add(new Room(i));
            }

            // Create the Rooms
            int id_counter = 0;
            for (int i = 0; i < HEIGHT_TILE_SIZE; i++)
            {
                for (int j = 0; j < WIDTH_TILE_SIZE; j++)
                {
                    if (j < WIDTH_TILE_SIZE / 2)
                    {
                        tiles[id_counter].setRoom(rooms[0]);
                        rooms[0].addTile(tiles[id_counter]);
                    }
                    else
                    {
                        tiles[id_counter].setRoom(rooms[1]);
                        rooms[1].addTile(tiles[id_counter]);
                    }
                    id_counter++;
                }
            }

            // Create Doors
            this.doors = new List<Door>();

            // Door 0 to 1
            List<int[]> temp = getAdjacentTileFromRoomToRoom(this.rooms[0], this.rooms[1]);
            int[] door = temp[temp.Count / 2];
            Tile[] tile_door1 = { getTileByID(door[0]), getTileByID(door[1]) };
            this.doors.Add(new Door(0, tile_door1));

            // Spawn Point Initialization -- Lets create 4 spawn points -- 2 monsters and 2 items
            this.spawns = new List<SpawnPoint>();

            this.spawns.Add(SpawnPoint.SpawnPointFactory(0, this.rooms[0].getID()));
            int tiles_count = getRoomByID(spawns[0].getRoom()).getTiles().Count;
            spawns[0].setTile(getRoomByID(spawns[0].getRoom()).getTiles()[(int)tiles_count / 2]);

            this.spawns.Add(SpawnPoint.SpawnPointFactory(0, this.rooms[1].getID()));
            tiles_count = getRoomByID(spawns[1].getRoom()).getTiles().Count;
            spawns[1].setTile(getRoomByID(spawns[1].getRoom()).getTiles()[(int)tiles_count / 2]);

        }

        private void initializeStaticMap3()
        {
            // Initialize the room list.
            this.rooms = new List<Room>();

            for (int i = 0; i < 5; i++)
            {
                this.rooms.Add(new Room(i));
            }

            for (int i = 0; i < HEIGHT_TILE_SIZE * WIDTH_TILE_SIZE; i++)
            {
                int mod = i % WIDTH_TILE_SIZE;
                if (i > WIDTH_TILE_SIZE * 4 && i <= WIDTH_TILE_SIZE * 12)
                {
                    if (mod < 4 && i <= WIDTH_TILE_SIZE * 9)
                    {
                        tiles[i].setRoom(rooms[0]);
                        rooms[0].addTile(tiles[i]);
                    }
                    else if (mod > 9 && i <= WIDTH_TILE_SIZE * 9)
                    {
                        tiles[i].setRoom(rooms[1]);
                        rooms[1].addTile(tiles[i]);
                    }
                    else if (mod < 4 && i > WIDTH_TILE_SIZE * 9)
                    {
                        tiles[i].setRoom(rooms[2]);
                        rooms[2].addTile(tiles[i]);
                    }
                    else if (mod > 9 && i > WIDTH_TILE_SIZE * 9)
                    {
                        tiles[i].setRoom(rooms[3]);
                        rooms[3].addTile(tiles[i]);
                    }
                    else
                    {
                        tiles[i].setRoom(rooms[4]);
                        rooms[4].addTile(tiles[i]);
                    }
                }
                else if (i <= WIDTH_TILE_SIZE * 4)
                {
                    if (mod < 7)
                    {
                        tiles[i].setRoom(rooms[0]);
                        rooms[0].addTile(tiles[i]);
                    }
                    else
                    {
                        tiles[i].setRoom(rooms[1]);
                        rooms[1].addTile(tiles[i]);
                    }
                }
                else if (i > WIDTH_TILE_SIZE * 9)
                {
                    if (mod < 7)
                    {
                        tiles[i].setRoom(rooms[2]);
                        rooms[2].addTile(tiles[i]);
                    }
                    else
                    {
                        tiles[i].setRoom(rooms[3]);
                        rooms[3].addTile(tiles[i]);
                    }
                }
            }
            // Create Doors
            // Create Doors
            this.doors = new List<Door>();

            // Door 0 to 1
            List<int[]> temp = getAdjacentTileFromRoomToRoom(this.rooms[0], this.rooms[1]);
            int[] door = temp[temp.Count / 2];
            Tile[] tile_door1 = { getTileByID(door[0]), getTileByID(door[1]) };
            this.doors.Add(new Door(0, tile_door1));

            // Door 0 to 2
            temp = getAdjacentTileFromRoomToRoom(this.rooms[0], this.rooms[2]);
            door = temp[temp.Count / 2];
            Tile[] tile_door2 = { getTileByID(door[0]), getTileByID(door[1]) };
            this.doors.Add(new Door(0, tile_door2));

            // Door 1 to 3
            temp = getAdjacentTileFromRoomToRoom(this.rooms[1], this.rooms[3]);
            door = temp[temp.Count / 2];
            Tile[] tile_door3 = { getTileByID(door[0]), getTileByID(door[1]) };
            this.doors.Add(new Door(0, tile_door3));

            // Door 2 to 3
            temp = getAdjacentTileFromRoomToRoom(this.rooms[2], this.rooms[3]);
            door = temp[temp.Count / 2];
            Tile[] tile_door4 = { getTileByID(door[0]), getTileByID(door[1]) };
            this.doors.Add(new Door(0, tile_door4));

            // Door 0 to 4
            temp = getAdjacentTileFromRoomToRoom(this.rooms[0], this.rooms[4]);
            door = temp[temp.Count / 2];
            Tile[] tile_door5 = { getTileByID(door[0]), getTileByID(door[1]) };
            this.doors.Add(new Door(0, tile_door5));

            // Door 1 to 4
            temp = getAdjacentTileFromRoomToRoom(this.rooms[1], this.rooms[4]);
            door = temp[temp.Count / 2];
            Tile[] tile_door6 = { getTileByID(door[0]), getTileByID(door[1]) };
            this.doors.Add(new Door(0, tile_door6));

            // Door 2 to 4
            temp = getAdjacentTileFromRoomToRoom(this.rooms[2], this.rooms[4]);
            door = temp[temp.Count / 2];
            Tile[] tile_door7 = { getTileByID(door[0]), getTileByID(door[1]) };
            this.doors.Add(new Door(0, tile_door7));

            // Door 3 to 4
            temp = getAdjacentTileFromRoomToRoom(this.rooms[3], this.rooms[4]);
            door = temp[temp.Count / 2];
            Tile[] tile_door8 = { getTileByID(door[0]), getTileByID(door[1]) };
            this.doors.Add(new Door(0, tile_door8));


            // Spawn Point Initialization -- Lets create 2 spawn Item Points
            this.spawns = new List<SpawnPoint>();

            this.spawns.Add(SpawnPoint.SpawnPointFactory(0, this.rooms[3].getID()));
            int tiles_count = getRoomByID(spawns[0].getRoom()).getTiles().Count;
            spawns[0].setTile(getRoomByID(spawns[0].getRoom()).getTiles()[(int)tiles_count / 2]);

            this.spawns.Add(SpawnPoint.SpawnPointFactory(0, this.rooms[1].getID()));
            tiles_count = getRoomByID(spawns[1].getRoom()).getTiles().Count;
            spawns[1].setTile(getRoomByID(spawns[1].getRoom()).getTiles()[(int)tiles_count / 2]);

        }

        private void initializeMap()
        {
            // Initialize the room list! Don't Forget!!!!!!
            this.rooms = new List<Room>();
            
            // Given a "Blank Tile Map", lets create rooms for it. This process is totally random, using our Util.MyRandom function. 
            Random rand = Util.MyRandom.getRandom().random();
            

            // Lets first create a List of ID's of all available tiles, so we can distribute them to rooms.
            List<int> tile_ids = getAllTileID();

            // Now we create a list of aggregated neighbours that are available for selection in order to extend the length of a room.
            List<int> neighbour_tiles = new List<int>(); 

            int pick_tile = tile_ids[0];
            int room_id_counter = 0;
            Room current_room = new Room(room_id_counter);

            // While there is still ID's in the list, keep distributing
            while (tile_ids.Count > 0)
            {
                // Get the tile with ID
                Tile t = getTileByID(pick_tile);
                tile_ids.Remove(pick_tile);

                //Add it to the current room
                current_room.addTile(t);
                t.setRoom(current_room);

                // Get Neighbours of tile
                List<Tile> neighbours = obtainAvailableNeighbours(t);
                foreach (Tile ti in neighbours)
                    neighbour_tiles.Add(ti.getID());

                // If there are neighboring tiles in the list do the "Room Test", if not just create a brand new room
                if (neighbour_tiles.Count > 0 && tile_ids.Count > 0)
                {
                    // if the random number is above the Change Room Variable, continue to extend the same room, else create a new room.
                    if (rand.NextDouble() > CHANGE_ROOM_VAR)
                    {
                        pick_tile = neighbour_tiles[rand.Next(neighbour_tiles.Count)];
                    }
                    else
                    {
                        room_id_counter++;
                        this.rooms.Add(current_room);
                        current_room = new Room(room_id_counter);
                        neighbour_tiles.Clear();
                        pick_tile = tile_ids[rand.Next(tile_ids.Count)];
                    }
                }
                else
                {
                    // Make sure that there are still tiles available to distribute...
                    if (tile_ids.Count > 0) 
                    {
                        room_id_counter++;
                        this.rooms.Add(current_room);
                        current_room = new Room(room_id_counter);
                        neighbour_tiles.Clear();
                        pick_tile = tile_ids[rand.Next(tile_ids.Count)];
                    }
                }
            }

            // Now that rooms are formed! Time to add the doors
            initializeDoors();
        }

        private void initializeDoors()
        {
            int door_counter = 0;
            this.doors = new List<Door>();

            // For the first iteration of the door initialization function we will randomly distribute one door for each room + one additional random door (for the outside).
            // NOT THE MOST EFFICIENT....
            foreach (Room r in this.rooms)
            {
                List<int[]> adj_tiles = getAllRoomAdjacentTiles(r);
                int[] rand_pick = adj_tiles[MyRandom.getRandom().random().Next(adj_tiles.Count)];
                Tile[] tiles = {getTileByID(rand_pick[0]), getTileByID(rand_pick[1])};
                this.doors.Add(new Door(door_counter, tiles));
                door_counter++;
            }

            // Pick all the tiles that are on the outside and randomly choose one for the outside door. NOTE -----> IGNORING THIS FOR NOW!!!
            //List<int> ext = getAllExtremityTiles();
            //Tile[] pick = {getTileByID( ext[MyRandom.getRandom().random().Next(ext.Count)] ), null};
            //this.doors.Add(new Door(door_counter, pick));
        }

        // Random spawn placement code
        private void initializeStaticSpawnPoints()
        {
            int num_additional_spawns = Util.MyRandom.getRandom().random().Next(MAX_SPAWN_NUM - MIN_SPAWN_NUM);
            spawns = new List<SpawnPoint>();

            // Add at least one item to a random room.
            spawns.Add(SpawnPoint.SpawnPointFactory(0, this.rooms[Util.MyRandom.getRandom().random().Next(this.rooms.Count)].getID())); // Get random room and place item.

            // Add at least one monsters to a random room.
            spawns.Add(SpawnPoint.SpawnPointFactory(1, this.rooms[Util.MyRandom.getRandom().random().Next(this.rooms.Count)].getID()));

            // Fill the rest of the rooms with additional spawn points, chosen at random.
            for (int i = 0; i < num_additional_spawns; i++)
            {
                int rand_spawn_type = Util.MyRandom.getRandom().random().Next(1);
                spawns.Add(SpawnPoint.SpawnPointFactory(rand_spawn_type, this.rooms[Util.MyRandom.getRandom().random().Next(this.rooms.Count)].getID()));
            }
        }

        public int[] getHeightWidth()
        {
            int[] coord = { height, width };
            return coord;
        }

        public int getID()
        {
            return ID;
        }

        public List<Room> getRooms()
        {
            return this.rooms;
        }

        public List<Tile> getTiles()
        {
            return this.tiles;
        }

        public List<Door> getDoors()
        {
            return this.doors;
        }

        public List<SpawnPoint> getSpawnPoints()
        {
            return this.spawns;
        }

        // Get neighbours of a specific tile. Neighbours returned in a fashion [UP, DOWN, LEFT, RIGHT], if a neighbour doesn't exist, returns null in its place.
        public Tile[] obtainNeighbours(Tile tile)
        {
            // if no tiles error.
            if (tiles.Count <= 0)
                return null;

            Tile[] neighbours = new Tile[4];
            
            // First check and see if the tile is in a extremity of the map
            // LEFT Extremity Check
            if ((tile.getID() % Map.WIDTH_TILE_SIZE) == 0)
            {
                neighbours[2] = null;
            }
            else
            {
                neighbours[2] = tiles[tile.getID() - 1];
            }
            
            // RIGHT Extremity Check
            if ((tile.getID() % Map.WIDTH_TILE_SIZE) == (Map.WIDTH_TILE_SIZE - 1))
            {
                neighbours[3] = null;
            }
            else
            {
                neighbours[3] = tiles[tile.getID() + 1];
            }

            // UP Extremity Check
            if (tile.getID() < Map.WIDTH_TILE_SIZE)//Map.HEIGHT_TILE_SIZE)
            {
                neighbours[0] = null;
            }
            else
            {
                neighbours[0] = tiles[tile.getID() - Map.WIDTH_TILE_SIZE];
            }

            //DOWN Extremity Check
            if (tile.getID() >= ((Map.HEIGHT_TILE_SIZE - 1) * Map.WIDTH_TILE_SIZE))
            {
                neighbours[1] = null;
            }
            else
            {
                neighbours[1] = tiles[tile.getID() + Map.WIDTH_TILE_SIZE];
            }

            return neighbours;
        }

        // Returns a list of only available tiles with no rooms associated.
        public List<Tile> obtainAvailableNeighbours(Tile tile)
        {
            Tile[] tiles = obtainNeighbours(tile);
            List<Tile> available_only = new List<Tile>();
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i] != null && tiles[i].getRoom() == null)
                    available_only.Add(tiles[i]);
            }
            return available_only;
        }

        // Returns a list with all the available tile ids.
        public List<int> getAllTileID()
        {
            List<int> id_list = new List<int>();
            foreach(Tile t in this.tiles)
            {
                id_list.Add(t.getID());
            }
            return id_list;
        }

        public Tile getTileByID(int ID)
        {
            foreach (Tile t in tiles)
            {
                if (t.getID() == ID)
                {
                    return t;
                }
            }
            return null;
        }

        public Room getRoomByID(int ID)
        {
            foreach (Room r in rooms)
            {
                if (r.getID() == ID)
                    return r;
            }
            return null;
        }

        // Returns a list of all the tiles in a room that are neighboring a different room.
        public List<int[]> getAllRoomAdjacentTiles(Room room)
        {
            List<int[]> adjacent_tiles = new List<int[]>();
            foreach (Tile t in room.getTiles())
            {
                foreach (Tile adj_t in obtainNeighbours(t))
                {
                    if (adj_t != null)
                    {
                        // If different rooms, add both tiles in list
                        if (t.getRoom().getID() != adj_t.getRoom().getID())
                        {
                            int[] tuple = { t.getID(), adj_t.getID() };
                            adjacent_tiles.Add(tuple);
                        }
                    }
                }
            }
            return adjacent_tiles;
        }

        // Returns a list (if any) of all adjacent tiles of room1 to room2.
        public List<int[]> getAdjacentTileFromRoomToRoom(Room room1, Room room2)
        {
            List<int[]> adj_tiles = new List<int[]>();
            foreach (Tile t in room1.getTiles())
            {
                foreach (Tile adj_t in obtainNeighbours(t))
                {
                    // If adj tile is equal to room2 ID.
                    if (adj_t != null && adj_t.getRoom().getID() == room2.getID())
                    {
                        int[] tuple = { t.getID(), adj_t.getID() };
                        adj_tiles.Add(tuple);
                    }
                }
            }
            if (adj_tiles.Count < 1)
            {
                //Console.WriteLine("Adjacent Tile Problem!");
            }

            return adj_tiles;
        }

        // Returns a list of all the tiles in the extremities of the map.
        public List<int> getAllExtremityTiles()
        {
            List<int> ext_tiles = new List<int>();
            foreach (Tile t in tiles)
            {
                foreach (Tile t_adj in obtainNeighbours(t))
                {
                    if (t_adj == null)
                    {
                        ext_tiles.Add(t.getID());
                        break;
                    }
                }
            }
            return ext_tiles;
        }

        public bool isDoor(Tile t1, Tile t2)
        {
            foreach (Door d in doors)
            {
                if (d.getConnectingTiles()[0].getID() == t1.getID() && t2 == null)
                {
                    if(d.getConnectingTiles()[1] == null)
                        return true;
                    else 
                        return false;
                } 
                else if (d.getConnectingTiles()[0].getID() == t1.getID() && d.getConnectingTiles()[1].getID() == t2.getID())
                {
                    return true;
                }
            }
            return false;
        }

        public void resetValues()
        {
            foreach (Tile t in tiles)
                t.resetTile();
            
            doors.Clear();
            rooms.Clear();
            spawns.Clear();
        }

        public void createRoom(int ID)
        {
            if (getRoomByID(ID) == null)
                rooms.Add(new Room(ID));
        }

        public List<int> getAllRoomIDs()
        {
            List<int> room_ids = new List<int>();
            foreach (Room r in rooms)
            {
                room_ids.Add(r.getID());
            }
            return room_ids;
        }

        public List<SpawnPoint> getItemOfRoomID(int roomID)
        {
            List<SpawnPoint> items = new List<SpawnPoint>();
            foreach (SpawnPoint sp in spawns)
            {
                if (sp.getType() == 0 && sp.getRoom() == roomID)
                {
                    items.Add(sp);
                }
            }
            return items;
        }

        // Creates an Identical Map of this.
        public Map cloneMap()
        {
            return new Map(this);
        }


        public bool itemExists(int roomID)
        {
            foreach (SpawnPoint sp in spawns)
            {
                if ((sp.getType() == 0 || sp.getType() == 2) && sp.getRoom() == roomID)
                    return true;
            }
            return false;
        }



        // NOT YET IMPLEMENTED!!! ----- TO BE IMPLEMENTED!
        public string toUnity()
        {
            string map = "";

            for (int i = 0; i < HEIGHT_TILE_SIZE; i++)
            {
                for (int j = 0; j < WIDTH_TILE_SIZE; j++)
                {

                }
            }

            return map;
        }

    }
}
