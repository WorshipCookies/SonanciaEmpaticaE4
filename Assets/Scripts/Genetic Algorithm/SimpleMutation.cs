using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.GeneticInterfaces;
using ProjectMaze.Util;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.GeneticAlgorithm
{
    public class SimpleMutation : IMutation
    {
        private double mutationRate;

        private static int MIN_SIZE = 5;

        private static string name_ID = "Adjacent Mutation";

        private ILogger logger;

        // This is a very "simple" mutation that once triggered copies the "adjacent" room ID to the current one.

        public SimpleMutation(double mutationRate)
        {
            this.mutationRate = mutationRate;
            this.logger = null;
        }

        public void mutate(IGenotype geno)
        {
            try
            {
                //Console.WriteLine("Simple Mutation Function");

                Genotype gen = (Genotype)geno;
                
                // Genotype Mutation - We need to choose adjacent tiles to rooms and transform them into another room (extending it).
                // 1 - Get All Adjacent Tiles and randomly pick one (so we can then filter the rest, so we can transform it).
                List<int[]> getTiles = getAdjacentTiles(gen);
                int rand = MyRandom.getRandom().random().Next(getTiles.Count);

                if (getTiles.Count < 1)
                {
                    // This is the case where no more rooms are available! - Zero Rooms means a crash! 
                }
                else
                {
                    // Biased to larger rooms this way -- Find a fairer way of random picking.
                    int pickedID1 = getTiles[rand][4];
                    int pickedID2 = getTiles[rand][5];

                    // 2 - Filter the Tiles so we only want the ones that matter.
                    List<int[]> aux = new List<int[]>();
                    foreach (int[] t in getTiles)
                    {
                        if (t[4] == pickedID1 && t[5] == pickedID2)
                        {
                            aux.Add(t);
                        }
                    }

                    // 3 - Randomly pick to Extend or Retract ID1 with ID2
                    if (MyRandom.getRandom().random().NextDouble() > 0.5)
                    {
                        if (logger != null)
                        {
                            logger.writeLogTimeStamp(" ------- " + name_ID + " ------- \n");
                            logger.writeLog("Extending Room: " + pickedID1 + " to Room: " + pickedID2 + ";\n");
                        }

                        foreach (int[] t in aux)
                        {
                            gen.getGeno()[t[0], t[1]] = gen.getGeno()[t[2], t[3]];
                        }
                    }
                    else
                    {
                        if (logger != null)
                        {
                            logger.writeLogTimeStamp(" ------- " + name_ID + " ------- \n");
                            logger.writeLog("Retracting Room: " + pickedID1 + " to Room: " + pickedID2 + ";\n");
                        }

                        foreach (int[] t in aux)
                        {
                            gen.getGeno()[t[2], t[3]] = gen.getGeno()[t[0], t[1]];
                        }
                    }

                    // 4 - If a room does not satisfy size constraint, it get fully assimilated to the other room. (If there are problems with Mutation this is the most likely place...)
                    if (heightByWidthRoom(pickedID1, gen) < MIN_SIZE)
                    {
                        if (logger != null)
                        {
                            logger.writeLog("Room: " + pickedID1 + " did not satisfy size constraint transforming into Room: " + pickedID2 + ";\n");
                        }
                        replaceIDWithAnother(pickedID1, pickedID2, gen);

                        // Fix Spawn Points
                        spawnPointRepair(pickedID1, gen);

                    }
                    else if (heightByWidthRoom(pickedID2, gen) < MIN_SIZE)
                    {
                        if (logger != null)
                        {
                            logger.writeLog("Room: " + pickedID2 + " did not satisfy size constraint transforming into Room: " + pickedID1 + ";\n");
                        }
                        replaceIDWithAnother(pickedID2, pickedID1, gen);

                        spawnPointRepair(pickedID2, gen);
                    }

                    // Now Handle the Doors ... (For now lets just add a door for each adjacent room) - Not Messing with doors in SimpleMutation at the moment!
                    //List<int[]> roomIDs = new List<int[]>();
                    //for (int i = 0; i < gen.getGeno().GetLength(0) - 1; i++)
                    //{
                    //    for (int j = 0; j < gen.getGeno().GetLength(1) - 1; j++)
                    //    {
                    //        // Check Right
                    //        if (gen.getGeno()[i, j] != gen.getGeno()[i, j + 1])
                    //        {
                    //            // Lowest ID value is ALWAYS in the first position 
                    //            int[] temp = new int[2];
                    //            if (gen.getGeno()[i, j] < gen.getGeno()[i, j + 1])
                    //            {
                    //                temp[0] = gen.getGeno()[i, j];
                    //                temp[1] = gen.getGeno()[i, j + 1];
                    //            }
                    //            else
                    //            {
                    //                temp[0] = gen.getGeno()[i, j + 1];
                    //                temp[1] = gen.getGeno()[i, j];
                    //            }

                    //            bool flag = true;

                    //            foreach (int[] r in roomIDs)
                    //            {
                    //                if (r[0] == temp[0] && r[1] == temp[1])
                    //                {
                    //                    flag = false;
                    //                }
                    //            }

                    //            if (flag)
                    //            {
                    //                roomIDs.Add(temp);
                    //            }
                    //        }

                    //        // Check Down
                    //        if (gen.getGeno()[i, j] != gen.getGeno()[i + 1, j])
                    //        {
                    //            // Lowest ID value is ALWAYS in the first position 
                    //            int[] temp = new int[2];
                    //            if (gen.getGeno()[i, j] < gen.getGeno()[i + 1, j])
                    //            {
                    //                temp[0] = gen.getGeno()[i, j];
                    //                temp[1] = gen.getGeno()[i + 1, j];
                    //            }
                    //            else
                    //            {
                    //                temp[0] = gen.getGeno()[i + 1, j];
                    //                temp[1] = gen.getGeno()[i, j];
                    //            }

                    //            bool flag = true;

                    //            foreach (int[] r in roomIDs)
                    //            {
                    //                if (r[0] == temp[0] && r[1] == temp[1])
                    //                {
                    //                    flag = false;
                    //                }
                    //            }

                    //            if (flag)
                    //            {
                    //                roomIDs.Add(temp);
                    //            }
                    //        }
                    //    }

                    //}

                    //gen.getDoors().Clear();
                    //foreach (int[] r in roomIDs)
                    //{
                    //    gen.addDoors(r[0], r[1]);
                    //}
                }

                if (gen.getDoors().Count < 1)
                {
                    //Console.WriteLine("We Have a Problem! In SIMPLE MUTATION!");
                }

                // HOT DAMN! If this doesn't contain any bugs it's a goddamn miracle!!!!.... it does
            }
            catch (Exception e)
            {
                throw e;
                //throw new InvalidCastException("Must be Class Genotype...");
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

        public static List<int[]> getAdjacentTiles(Genotype geno)
        {
            List<int[]> adj_tiles = new List<int[]>();

            // Check for Horizontal Adjacency
            for (int i = 0; i < geno.getGeno().GetLength(0); i++)
            {
                for (int j = 0; j < geno.getGeno().GetLength(1); j++)
                {
                    int[] coord = { i, j };
                    List<int[]> neighbours = getAvailableNeighbours(geno, coord);

                    foreach (int[] l in neighbours)
                    {
                        adj_tiles.Add(l);
                    }
                }
            }
            return adj_tiles;
        }

        public static List<int[]> getAvailableNeighbours(Genotype geno, int[] pos)
        {
            int roomID = geno.getGeno()[pos[0], pos[1]];
            List<int[]> neighbours = new List<int[]>();
            
            // CONFUSING AS HOLY SHIT!! - a int[] consists of coord of the adjacent tiles (x1,y1)(x2,y2) + room IDs of both so a data structure = [x1,y1,x2,y2,ID1,ID2].
            // All tiles that are in Room X and Room Y are added into the same position of the "global" list. If that doesn't exist create a new position.

            // Check Right
            if (pos[1] < geno.getGeno().GetLength(1) - 1)
            {
                if (geno.getGeno()[pos[0], pos[1]] != geno.getGeno()[pos[0], pos[1] + 1])
                {
                    int[] new_tile = { pos[0], pos[1], pos[0], pos[1] + 1, geno.getGeno()[pos[0], pos[1]], geno.getGeno()[pos[0], pos[1] + 1] };
                    neighbours.Add(new_tile);
                }
            }

            //Check Down
            if (pos[0] < geno.getGeno().GetLength(0) - 1)
            {
                if (geno.getGeno()[pos[0], pos[1]] != geno.getGeno()[pos[0] + 1, pos[1]])
                {
                    int[] new_tile = { pos[0], pos[1], pos[0] + 1, pos[1], geno.getGeno()[pos[0], pos[1]], geno.getGeno()[pos[0] + 1, pos[1]] };
                    neighbours.Add(new_tile);
                }
            }

            return neighbours;
        }

        public int heightByWidthRoom(int ID, Genotype geno)
        {
            int count = 0;
            for (int i = 0; i < geno.getGeno().GetLength(0); i++)
            {
                for (int j = 0; j < geno.getGeno().GetLength(1); j++)
                {
                    if (geno.getGeno()[i, j] == ID)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public void replaceIDWithAnother(int toReplaceID, int replacerID, Genotype geno)
        {
            for (int i = 0; i < geno.getGeno().GetLength(0); i++)
            {
                for (int j = 0; j < geno.getGeno().GetLength(1); j++)
                {
                    if (geno.getGeno()[i, j] == toReplaceID)
                    {
                        geno.getGeno()[i, j] = replacerID;
                    }
                }
            }
        }

        public static void spawnPointRepair(int roomID, Genotype geno)
        {
            geno.replaceAllRoomOfSpawns(roomID);
        }


        // NOT IMPLEMENTED!!!
        public void repairFunction(int ID1, int ID2, Genotype geno)
        {
            int totalID1 = 0;
            int totalID2 = 0;
            Queue<int[]> queue = new Queue<int[]>();
            Queue<int[]> queue2 = new Queue<int[]>();

            // Count the total of that room exists in the Genotype and the first tile encounter to the queue.
            for (int i = 0; i < geno.getGeno().GetLength(0); i++)
            {
                for (int j = 0; j < geno.getGeno().GetLength(1); j++)
                {
                    if (geno.getGeno()[i, j] == ID1) 
                    {
                        if(totalID1 == 0)
                        {
                            int[] pos = {i, j};
                            queue.Enqueue(pos);
                        }
                        totalID1++;
                    }
                    if (geno.getGeno()[i, j] == ID2)
                    {
                        if(totalID2 == 0)
                        {
                            int[] pos = {i, j};
                            queue.Enqueue(pos);
                        }
                        totalID2++;
                    }
                }
            }
        }
    }
}
