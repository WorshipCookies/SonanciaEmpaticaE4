using UnityEngine;
using System.Collections.Generic;
using System;
using ProjectMaze.GeneticAlgorithm;
using ProjectMaze.Util;
using ProjectMaze.Visual;
using ProjectMaze.Visual.Spawn;

public class LevelLoader {

    public static int TILE_SIZE = Tile.TILE_SIZE;

    public static int[] myStaticLevel()
    {
        int[] level = { 
                        
                        8, 8, 8, -8, -1, 1, 1, 1, 1, 1, 1, 2, 2, 2, // 0
                        
                        8, 8, 8, 3, 1, 1, -1, 1, 1, 1, 1, 2, 2, 2, // 14
                        
                        8, 8, 3, 3, 3, 3, -3, 3, 3, 3, 2, 2, 2, 2, // 28
                        
                        8, 8, 3, 3, 3, 3, 3, 3, 3, 3, 2, 2, 2, 2, // 42
                        
                        8, 8, 3, 3, 3, 3, 3, 3, 3, 3, 2, 2, 2, 2, // 56
                        
                        8, -8, 8, 4, 4, 4, 4, 4, 4, 4, 2, 2, 2, 2, // 70

                        5, -5, 5, 4, 4, 4, 4, 4, 4, -4, -2, 2, -2, 2, // 84

                        5, 5, 5, 4, 4, 4, 4, 6, 6, 6, 6, 7, -7, 7, // 98

                        5, 5, 5, 4, 4, 6, 6, 6, 6, 6, 6, 6, 7, 7, // 112

                        5, 5, 5, 4, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, // 126

                        5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, -6, -7, 7, // 140

                        5, 5, 5, 5, -5, -6, 6, 6, 6, 6, 6, 6, 7, 7, // 154

                        5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 7, 7, // 168

                        5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 7, 7  // 182

                      
                      };

        return level;
    }

    public static int[] myInverseStaticLevel()
    {
        int[] level = { 
                        
                        2, 2, 2, 1, 1, 1, 1, 1, 1, -1, -8, 8, 8, 8, // 0
                        
                        2, 2, 2, 1, 1, 1, 1, -1, 1, 1, 3, 8, 8, 8, // 14
                        
                        2, 2, 2, 2, 3, 3, 3, -3, 3, 3, 3, 3, 8, 8, // 28
                        
                        2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 8, 8, // 42 

                        2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 8, 8, // 56

                        2, 2, 2, 2, 4, 4, 4, 4, 4, 4, 4, 8, -8, 8, // 70

                        2, -2, 2, -2, -4, 4, 4, 4, 4, 4, 4, 5, -5, 5, // 84
 
                        7, -7, 7, 6, 6, 6, 6, 4, 4, 4, 4, 5, 5, 5,  // 98

                        7, 7, 6, 6, 6, 6, 6, 6, 6, 4, 4, 5, 5, 5, // 112

                        7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 4, 5, 5, 5, // 126

                        7, -7, -6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, // 140

                        7, 7, 6, 6, 6, 6, 6, 6, -6, -5, 5, 5, 5, 5, // 154 

                        7, 7, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, // 168

                        7, 7, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, // 182
                      
                      };

        return level;
    }

    public static LevelObject staticLevelLoader(string path)
    {
        string[] text = System.IO.File.ReadAllLines(@path);
        return levelLoader(text, readFromFile(path));
    }

    public static LevelObject genLevelLoader(Phenotype phen)
    {
        string[] text = ReadWriteToFile.transformToString(phen).Split('\n');
        return levelLoader(text, phen);
    }

    public static LevelObject levelLoader(string[] text, Phenotype phen)
    {
        string[] height_width = text[0].Split(';');
        int height = Convert.ToInt32(height_width[0]);
        int width = Convert.ToInt32(height_width[1]);

        height = (height / TILE_SIZE);
        width = (width / TILE_SIZE);

        int[] level = new int[height * width];
        Debug.Log("Made it! " + height + " " + width);
        string[] tile_str = text[1].Split(';');

        for (int i = 0; i < level.Length; i++)
        {
            level[i] = Convert.ToInt32(tile_str[i])+1;
        }

        List<DoorObject> doors = new List<DoorObject>();
        string[] door_str = text[2].Split(';');
        for (int i = 0; i < door_str.Length; i++)
        {
            string[] connections = door_str[i].Split(',');
            int id1 = Convert.ToInt32(connections[0]);
            int id2 = Convert.ToInt32(connections[1]);

            doors.Add(new DoorObject(id1, id2, level[id1], level[id2]));
            level[id1] = -1*level[id1];
            level[id2] = -1*level[id2];
        }

        List<int> monsters = new List<int>();
        List<int> items = new List<int>();
        List<int> sub_items = new List<int>();
        List<int> lights = new List<int>();
        List<int> soundFX = new List<int>();

        string[] spawn_str = text[3].Split(';');
        for (int i = 0; i < spawn_str.Length; i++)
        {
            string[] spawn = spawn_str[i].Split(',');
            int pos = Convert.ToInt32(spawn[0]);
            int id = Convert.ToInt32(spawn[1]);
            if (id == 0)
            {
                items.Add(pos);
            }
            else if (id == 1)
            {
                monsters.Add(pos);
            }
            else if (id == 2)
            {
                sub_items.Add(pos);
            }
            else if (id == 3)
            {
                soundFX.Add(pos);
            }
            else if (id == 4)
            {
                lights.Add(pos);
            }

        }
        return new LevelObject(level, doors, items, monsters, sub_items, lights, soundFX, height, width, phen);
    }

    public static List<DoorBuilder> getDoors()
    {
        List<DoorBuilder> doors = new List<DoorBuilder>();

        doors.Add(new DoorBuilder(0, 1, 3, 4));
        doors.Add(new DoorBuilder(1, 3, 20, 34));
        doors.Add(new DoorBuilder(0, 5, 71, 85));
        doors.Add(new DoorBuilder(4, 2, 93, 94));
        doors.Add(new DoorBuilder(2, 7, 110, 124));
        doors.Add(new DoorBuilder(6, 7, 151, 152));
        doors.Add(new DoorBuilder(5, 6, 158, 159));

        return doors;
    }

    public static int numRooms()
    {
        return 7;
    }

    public static Phenotype readFromFile(string path)
    {
        string[] text = System.IO.File.ReadAllLines(@path);

        // Get the Width and Height number of tiles!
        string[] height_width = text[0].Split(';');
        int height = Convert.ToInt32(height_width[0]);
        int width = Convert.ToInt32(height_width[1]);

        // Create Tile and Room Map.
        List<Tile> tiles = Tile.createTileMap(height, width);
        List<Room> rooms = new List<Room>();
        List<int> roomAdded = new List<int>();

        string[] tile_str = text[1].Split(';');

        for (int i = 0; i < tile_str.Length; i++)
        {
            int roomId = Convert.ToInt32(tile_str[i]);

            if (roomAdded.Contains(roomId))
            {
                // Add tile to current room.
                foreach (Room r in rooms)
                {
                    if (r.getID() == roomId)
                    {
                        tiles[i].setRoom(r);
                        r.addTile(tiles[i]);
                    }
                }
            }
            else
            {

                // Create a new room for that and add tile.
                Room room = new Room(roomId);
                tiles[i].setRoom(room);
                room.addTile(tiles[i]);

                rooms.Add(room);
                roomAdded.Add(roomId);
            }
        }

        // Create the Door Map
        string[] door_str = text[2].Split(';');
        List<Door> doors = new List<Door>();

        for (int i = 0; i < door_str.Length; i++)
        {
            string[] connections = door_str[i].Split(',');
            Door door = new Door(i);

            int id1 = Convert.ToInt32(connections[0]);
            foreach (Tile t in tiles)
            {
                if (t.getID() == id1)
                {
                    door.setAdj1(t);
                    t.setDoor(door);
                    t.getRoom().addDoor(door);
                }
            }

            int id2 = Convert.ToInt32(connections[1]);
            foreach (Tile t in tiles)
            {
                if (t.getID() == id2)
                {
                    door.setAdj2(t);
                    t.setDoor(door);
                    t.getRoom().addDoor(door);
                }
            }
            doors.Add(door);
        }

        // Create Spawn Points
        string[] spawn_str = text[3].Split(';');
        List<SpawnPoint> spawns = new List<SpawnPoint>();

        for (int i = 0; i < spawn_str.Length; i++)
        {
            string[] spawn = spawn_str[i].Split(',');
            int id = Convert.ToInt32(spawn[0]);

            foreach (Tile t in tiles)
            {
                if (t.getID() == id)
                {
                    SpawnPoint sp = SpawnPoint.SpawnPointFactory(Convert.ToInt32(spawn[1]), t.getRoom().getID());
                    sp.setTile(t);
                    spawns.Add(sp);
                }
            }
        }

        Map map = new Map(height, width, rooms, tiles, doors, spawns);
        Phenotype phen = new Phenotype(map);
        SimpleFitness sf = new SimpleFitness();
        sf.evaluate(phen);
        
        return phen;
    }
}
