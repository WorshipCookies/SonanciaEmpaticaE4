using ProjectMaze.GeneticAlgorithm;
using ProjectMaze.Visual;
using ProjectMaze.Visual.Spawn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Util
{
    public class ReadWriteToFile
    {

        // Text File Structure -- EACH LINE IS A DIFFERENT PART OF THE PHENOTYPE. (SEMI-COLON SEPERATED)
        // Zero Line -> Height Number of Tiles and Width Number of Tiles
        // First Line -> Tile Map + Room Structure (Index = Tile ID && Value = Room ID)
        // Second Line -> Doors. Each Door is seperated by a Semi-Colon, each Room ID in the door section is seperated by a comma.
        // Third Line -> Spawns. Each Spawn is seperated by a Semi-Colon, each spawn type and tile location is sperated by a comma.

        public static void writeToFile(Phenotype phen, string path)
        {
            string file = "";


            // Write the HEIGHT and WIDTH number of tiles
            file += phen.getMap().getHeightWidth()[0] + ";" + phen.getMap().getHeightWidth()[1] + "\n";

            // Write the Tile Line in Text
            for (int i = 0; i < phen.getMap().getTiles().Count; i++)
            {
                if (i == phen.getMap().getTiles().Count - 1)
                {
                    file += phen.getMap().getTiles()[i].getRoom().getID() + "\n";
                }
                else
                {
                    file += phen.getMap().getTiles()[i].getRoom().getID() + ";";
                }
            }

            // Write the Door Line in Text
            for (int i = 0; i < phen.getMap().getDoors().Count; i++)
            {
                Tile[] adjtiles = phen.getMap().getDoors()[i].getConnectingTiles();
                if (i == phen.getMap().getDoors().Count - 1)
                {
                    file += adjtiles[0].getID() + "," + adjtiles[1].getID() + "\n";
                }
                else
                {
                    file += adjtiles[0].getID() + "," + adjtiles[1].getID() + ";";
                }
            }

            // Items and Monsters!
            for (int i = 0; i < phen.getMap().getSpawnPoints().Count; i++)
            {
                if (i == phen.getMap().getSpawnPoints().Count - 1)
                {
                    if(phen.getMap().getSpawnPoints()[i].getType() == 0)
                    {
                        if(phen.getMap().getSpawnPoints()[i].getRoom() == phen.getMainItem().getRoom())
                        {
                            // Main Item
                            file += phen.getMap().getSpawnPoints()[i].getTile().getID() + "," + "0" + "\n";
                        }
                        else
                        {
                            // Sub Item
                            file += phen.getMap().getSpawnPoints()[i].getTile().getID() + "," + "2" + "\n";
                        }
                    }
                    else
                    {
                        file += phen.getMap().getSpawnPoints()[i].getTile().getID() + "," + phen.getMap().getSpawnPoints()[i].getType() + "\n";
                    }
                }
                else
                {
                    if (phen.getMap().getSpawnPoints()[i].getType() == 0)
                    {
                        if (phen.getMap().getSpawnPoints()[i].getRoom() == phen.getMainItem().getRoom())
                        {
                            // Main Item
                            file += phen.getMap().getSpawnPoints()[i].getTile().getID() + "," + "0" + ";";
                        }
                        else
                        {
                            // Sub Item
                            file += phen.getMap().getSpawnPoints()[i].getTile().getID() + "," + "2" + ";";
                        }
                    }
                    else
                    {
                        file += phen.getMap().getSpawnPoints()[i].getTile().getID() + "," + phen.getMap().getSpawnPoints()[i].getType() + ";";
                    }
                }
            }

            System.IO.File.WriteAllText(@path, file); // Write the text to the file -- For now I think this is enough. Lets see...
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

            return new Phenotype(map);
        }

        public static string transformToString(Phenotype phen)
        {
            string file = "";

            // Write the HEIGHT and WIDTH number of tiles
            file += phen.getMap().getHeightWidth()[0] + ";" + phen.getMap().getHeightWidth()[1] + "\n";

            // Write the Tile Line in Text
            for (int i = 0; i < phen.getMap().getTiles().Count; i++)
            {
                if (i == phen.getMap().getTiles().Count - 1)
                {
                    file += phen.getMap().getTiles()[i].getRoom().getID() + "\n";
                }
                else
                {
                    file += phen.getMap().getTiles()[i].getRoom().getID() + ";";
                }
            }

            // Write the Door Line in Text
            for (int i = 0; i < phen.getMap().getDoors().Count; i++)
            {
                Tile[] adjtiles = phen.getMap().getDoors()[i].getConnectingTiles();
                if (i == phen.getMap().getDoors().Count - 1)
                {
                    file += adjtiles[0].getID() + "," + adjtiles[1].getID() + "\n";
                }
                else
                {
                    file += adjtiles[0].getID() + "," + adjtiles[1].getID() + ";";
                }
            }

            // Items and Monsters!
            for (int i = 0; i < phen.getMap().getSpawnPoints().Count; i++)
            {
                if (i == phen.getMap().getSpawnPoints().Count - 1)
                {
                    if (phen.getMap().getSpawnPoints()[i].getType() == 0)
                    {
                        if (phen.getMap().getSpawnPoints()[i].getRoom() == phen.getMainItem().getRoom())
                        {
                            // Main Item
                            file += phen.getMap().getSpawnPoints()[i].getTile().getID() + "," + "0" + "\n";
                        }
                        else
                        {
                            // Sub Item
                            file += phen.getMap().getSpawnPoints()[i].getTile().getID() + "," + "2" + "\n";
                        }
                    }
                    else
                    {
                        file += phen.getMap().getSpawnPoints()[i].getTile().getID() + "," + phen.getMap().getSpawnPoints()[i].getType() + "\n";
                    }
                }
                else
                {
                    if (phen.getMap().getSpawnPoints()[i].getType() == 0)
                    {
                        if (phen.getMap().getSpawnPoints()[i].getRoom() == phen.getMainItem().getRoom())
                        {
                            // Main Item
                            file += phen.getMap().getSpawnPoints()[i].getTile().getID() + "," + "0" + ";";
                        }
                        else
                        {
                            // Sub Item
                            file += phen.getMap().getSpawnPoints()[i].getTile().getID() + "," + "2" + ";";
                        }
                    }
                    else
                    {
                        file += phen.getMap().getSpawnPoints()[i].getTile().getID() + "," + phen.getMap().getSpawnPoints()[i].getType() + ";";
                    }
                }
            }
            return file;
        }
    }
}
