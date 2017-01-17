using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.GeneticInterfaces;
using ProjectMaze.Util;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.GeneticAlgorithm
{
    public class DoorMutation : IMutation
    {

        private double mutationRate;

        private static double MAGIC_NUMBER = 0.5;

        private ILogger logger;

        private static string name_ID = "Door Mutation";

        public DoorMutation(double mutationRate)
        {
            this.mutationRate = mutationRate;
            logger = null;
        }

        public void mutate(IGenotype geno)
        {

            //Console.WriteLine("DOOR MUTATION!!!");

            try
            {
                // Door Mutation goes here!
                Genotype gen = (Genotype)geno;

                // Get Room IDs -- This is necessary to have a sort of balance between the number of doors, but it is also necessary to add a door (unless we randomly add a door)
                // While we do this we should get the adjacent rooms as well... you never know when you need to add that door!
                List<int> roomIds = new List<int>();
                List<int[]> adjacentIds = new List<int[]>();
                for (int i = 0; i < gen.getGeno().GetLength(0); i++)
                {
                    for (int j = 0; j < gen.getGeno().GetLength(1); j++)
                    {
                        if (!roomIds.Contains(gen.getGeno()[i, j]))
                        {
                            roomIds.Add(gen.getGeno()[i, j]);
                        }
                    }
                }

                // For Logging Purposes only
                if (logger != null)
                {
                    logger.writeLogTimeStamp(" ------- " + name_ID + " ------- \n");
                }

                // Obviously only remove or add doors if there is one or more rooms existent in the geno.
                if (roomIds.Count > 1)
                {
                    // 50 - 50 chance in adding or removing door
                    List<int[]> doors = gen.getDoors();

                    int num_rooms = roomIds.Count;
                    int num_doors = doors.Count;

                    // Lame ... 
                    double rand = MyRandom.getRandom().random().NextDouble();
                    if (rand > MAGIC_NUMBER && num_doors > 1)
                    {
                        gen.removeRandomDoor();
                    }
                    else
                    {
                        // First Check and see if a room isn't connected -- give priority to un-connected rooms.
                        List<int> unconnected_rooms = GetUnconnectedRooms(roomIds, doors);
                        if (unconnected_rooms.Count > 0)
                        {
                            int picked_room = unconnected_rooms[MyRandom.getRandom().random().Next(unconnected_rooms.Count)];
                            gen.addDoors(picked_room, getRandomAdjacentID(gen, picked_room));
                        }
                        else
                        {
                            // If all rooms are connected... then just remove a door!
                            gen.removeRandomDoor();

                            // OLD CODE - ADD RANDOM DOOR 
                            //int picked_room = roomIds[MyRandom.getRandom().random().Next(roomIds.Count)];
                            //gen.addDoors(picked_room, getRandomAdjacentID(gen, picked_room));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void setMutationRate(double mutation_rate)
        {
            this.mutationRate = mutation_rate;
        }

        public double getMutationRate()
        {
            return mutationRate;
        }

        public void setLogger(LogSystem.Interfaces.ILogger log)
        {
            this.logger = log;
        }

        // Returns a list of Ids that are no connected to any room
        public List<int> GetUnconnectedRooms(List<int> roomIds, List<int[]> doors)
        {
            List<int> unconnectedIds = new List<int>(roomIds);
            foreach (int[] d in doors)
            {
                if (unconnectedIds.Contains(d[0]))
                {
                    unconnectedIds.Remove(d[0]);
                }

                if (unconnectedIds.Contains(d[1]))
                {
                    unconnectedIds.Remove(d[1]);
                }
            }
            return unconnectedIds;
        }

        public static int getRandomAdjacentID(Genotype geno, int picked_ID)
        {
            //List<int[]> adjacent = SimpleMutation.getAdjacentTiles(geno);

            //List<int> adj_ids = new List<int>();
            //foreach (int[] adj in adjacent)
            //{
            //    if ( (adj[4] == picked_ID && !adj_ids.Contains(adj[5])) )
            //    {
            //        adj_ids.Add(adj[5]);
            //    }

            //    if ( (adj[5] == picked_ID && !adj_ids.Contains(adj[4])) )
            //    {
            //        adj_ids.Add(adj[4]);
            //    }
            //}
            List<int> adj_ids = getAdjacentIDs(geno, picked_ID);
            return adj_ids[MyRandom.getRandom().random().Next(adj_ids.Count)];
        }

        public static List<int> getAdjacentIDs(Genotype geno, int picked_ID)
        {
            List<int[]> adjacent = SimpleMutation.getAdjacentTiles(geno);

            List<int> adj_ids = new List<int>();
            foreach (int[] adj in adjacent)
            {
                if ((adj[4] == picked_ID && !adj_ids.Contains(adj[5])))
                {
                    adj_ids.Add(adj[5]);
                }

                if ((adj[5] == picked_ID && !adj_ids.Contains(adj[4])))
                {
                    adj_ids.Add(adj[4]);
                }
            }
            return adj_ids;
        }

        public static int getRandomAdjacentID(Genotype geno, int picked_ID, int spawnType)
        {
            List<int> adj_ids = getAdjacentIDs(geno, picked_ID);
            List<int[]> spawn = geno.getSpawnPoints();

            List<int> canPick = new List<int>();
            foreach(int id in adj_ids)
            {
                bool hasSpawn = false;
                foreach(int[] s in spawn)
                {
                    if(s[1] == id && s[0] == spawnType)
                    {
                        hasSpawn = true;
                    }
                }
                if (!hasSpawn)
                {
                    canPick.Add(id);
                }    
            }

            if(canPick.Count > 0)
            {
                return canPick[MyRandom.getRandom().random().Next(canPick.Count)];
            }
            else
            {
                return -1;
            }
        }
    }
}
