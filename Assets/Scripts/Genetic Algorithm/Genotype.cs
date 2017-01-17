using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.GeneticInterfaces;
using ProjectMaze.Visual;
using ProjectMaze.Util;
using ProjectMaze.LogSystem.Interfaces;
using ProjectMaze.Visual.Spawn;

namespace ProjectMaze.GeneticAlgorithm
{
    public class Genotype : IGenotype
    {

        private int[,] geno;
        private List<int[]> doors;

        private List<int[]> spawnpoints;

        private ILogger logger;

        public Genotype(int height, int width)
        {
            this.geno = new int[Map.HEIGHT_TILE_SIZE, Map.WIDTH_TILE_SIZE];
            
            this.doors = new List<int[]>();
            this.spawnpoints = new List<int[]>();

            this.logger = null;
        }

        public void setGeno(int[,] geno)
        {
            this.geno = geno;
        }

        public int[,] getGeno()
        {
            return geno;
        }

        public void setDoors(List<int[]> doors)
        {
            this.doors = doors;
        }

        public List<int[]> getDoors()
        {
            return this.doors;
        }

        public void resetDoors()
        {
            this.doors = new List<int[]>();
        }

        public void addDoors(int ID1, int ID2)
        {
            if (logger != null)
            {
                logger.writeLog("Adding a new Door from Room " + ID1 + " to Room " + ID2 + ";\n");
            }
            int[] door = { ID1, ID2 };
            this.doors.Add(door);
        }

        // Remove a random door.
        public void removeRandomDoor()
        {
            int pos = MyRandom.getRandom().random().Next(this.doors.Count);
            if (logger != null)
            {
                logger.writeLog("Eliminating Door from Room " + this.doors[pos][0] + " to Room " + this.doors[pos][1] + ";\n");
            }
            this.doors.RemoveAt(pos);
        }

        public void setLogger(LogSystem.Interfaces.ILogger log)
        {
            this.logger = log;
        }

        // Transform a Phenotype p into a Genotype
        public void toGenotype(Phenotype p)
        {
            // Always Clear the Genotype! Before Transformation
            this.geno = new int[Map.HEIGHT_TILE_SIZE, Map.WIDTH_TILE_SIZE];
            doors = new List<int[]>();
            spawnpoints = new List<int[]>();

            // Transform tiles into a list of numbers (Position = TileID && Data = RoomID).
            int counter = -1;
            foreach (Tile t in p.getMap().getTiles())
            {
                if (t.getID() % Map.WIDTH_TILE_SIZE == 0)
                    counter++;

                geno[counter, t.getID()%Map.WIDTH_TILE_SIZE] = t.getRoom().getID();
            }

            
            // Add Doors at the end of the geno.
            foreach (Door d in p.getMap().getDoors())
            {
                int[] i = { d.getConnectingTiles()[0].getRoom().getID(), d.getConnectingTiles()[1].getRoom().getID() };
                doors.Add(i);
            }

            // Add Spawn points in the geno.
            foreach (SpawnPoint sp in p.getMap().getSpawnPoints())
            {
                int[] i = { sp.getType(), sp.getRoom() };
                spawnpoints.Add(i);
            }

        }

        public void updateTilesOnly(Phenotype p)
        {
            
            // Transform tiles into a list of numbers (Position = TileID && Data = RoomID).
            int counter = -1;
            foreach (Tile t in p.getMap().getTiles())
            {
                if (t.getID() % Map.WIDTH_TILE_SIZE == 0)
                    counter++;

                geno[counter, t.getID() % Map.WIDTH_TILE_SIZE] = t.getRoom().getID();
            }

            // Add Doors at the end of the geno.
            foreach (Door d in p.getMap().getDoors())
            {
                int[] i = { d.getConnectingTiles()[0].getRoom().getID(), d.getConnectingTiles()[1].getRoom().getID() };
                doors.Add(i);
            }
        }

        public bool isDoorConnected(int ID1, int ID2)
        {
            foreach (int[] door in doors)
            {
                if ((door[0] == ID1 && door[1] == ID2) || (door[0] == ID2 && door[1] == ID1))
                {
                    return true;
                }
            }
            return false;
        }

        // Spawn Point additions!
        public void setSpawnPoints(List<int[]> spawnpoints)
        {
            this.spawnpoints = spawnpoints;
        }

        public List<int[]> getSpawnPoints()
        {
            return this.spawnpoints;
        }

        public void resetSpawns()
        {
            this.spawnpoints = new List<int[]>();
        }

        public void addSpawnPoint(int type, int roomID)
        {
            if (logger != null)
            {
                logger.writeLog("Adding a SpawnPoint of type " + type + " to Room " + roomID + ";\n");
            }
            int[] spawn = { type, roomID };
            this.spawnpoints.Add(spawn);
        }

        public void removeRandomSpawnPoint()
        {
            int pos = MyRandom.getRandom().random().Next(this.spawnpoints.Count);
            if (logger != null)
            {
                logger.writeLog("Eliminating Spawnpoint of type " + this.spawnpoints[pos][0] + " of Room " + this.spawnpoints[pos][1] + ";\n");
            }
            spawnpoints.RemoveAt(pos);
        }

        public void flipRandomSpawnPointType()
        {
            int pos = MyRandom.getRandom().random().Next(this.spawnpoints.Count);
            if (this.spawnpoints[pos][0] == 0)
            {
                if (logger != null)
                {
                    logger.writeLog("Changing Spawnpoint of type " + this.spawnpoints[pos][0] + " of Room " + this.spawnpoints[pos][1] + " to type new type 1"  + ";\n");
                }
                this.spawnpoints[pos][0] = 1;
            }
            else if (this.spawnpoints[pos][0] == 0)
            {
                if (logger != null)
                {
                    logger.writeLog("Changing Spawnpoint of type " + this.spawnpoints[pos][0] + " of Room " + this.spawnpoints[pos][1] + " to type new type 0" + ";\n");
                }
                this.spawnpoints[pos][0] = 0;
            }
        }

        // Replaces the spawns of that room in case that room suddenly disappears!
        public void replaceAllRoomOfSpawns(int original_roomID)
        {
            foreach(int[] sp in spawnpoints)
            {
                if (sp[1] == original_roomID)
                {
                    sp[1] = geno[MyRandom.getRandom().random().Next(geno.GetLength(0)),MyRandom.getRandom().random().Next(geno.GetLength(1))];
                }
            }
        }

        public List<int> getAllRoomIDs()
        {
            List<int> roomIds = new List<int>();

            for (int i = 0; i < geno.GetLength(0); i++)
            {
                for (int j = 0; j < geno.GetLength(1); j++)
                {
                    if (!roomIds.Contains(geno[i, j]))
                    {
                        roomIds.Add(geno[i, j]);
                    }
                }
            }
            return roomIds;
        }

    }
}
