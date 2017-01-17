using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.GeneticAlgorithm
{
    public class PlaceLightMutation : IMutation
    {
        private double mutationRate;
        private ILogger logger;

        private static string name_ID = "Light Place Spawn Mutation";

        private static int MAX_LIGHT_PER_ROOM = 1; // THIS CAN CHANGE IN THE FUTURE! We can add 2 or even more lights!

        public PlaceLightMutation(double mutation_rate)
        {
            this.mutationRate = mutation_rate;
            this.logger = null;
        }

        public double getMutationRate()
        {
            return this.mutationRate;
        }

        public void mutate(IGenotype geno)
        {
            // This mutation adds or removes a light from a room (i.e. Max Number of Lights per Room = MAX_LIGHT_PER_ROOM).

            // Choose a random room. Randomly add or remove a light from a room, obviously if no lights exist add light only, or if the max number of lights 
            // has been achieved remove only.
            try
            {
                Genotype gen = (Genotype)geno;

                // Get All Room Id's currently Available.
                List<int> roomIds = gen.getAllRoomIDs();

                // Pick a room Id at random
                int pickedRoom = roomIds[Util.MyRandom.getRandom().random().Next(roomIds.Count)];

                // If a Light is already in that room remove it; if not add it!
                List<int> pos_light = isLightInRoom(gen, pickedRoom);

                if (pos_light.Count >= MAX_LIGHT_PER_ROOM)
                {
                    // If a light exists remove it!
                    if (MAX_LIGHT_PER_ROOM > 1)
                    {
                        //Choose randomly
                        gen.getSpawnPoints().RemoveAt(pos_light[Util.MyRandom.getRandom().random().Next(pos_light.Count)]);
                    }
                    else
                    {
                        gen.getSpawnPoints().RemoveAt(pos_light[0]); // Remove the monster!
                    }
                }
                else
                {
                    if (MAX_LIGHT_PER_ROOM > 1 && pos_light.Count > 0)
                    {
                        // 50-50 chance of adding one more light or removing it
                        if (Util.MyRandom.getRandom().random().NextDouble() < 0.5)
                        {
                            // Add Light
                            int[] new_light = { 4, pickedRoom }; // CHANGE
                            gen.getSpawnPoints().Add(new_light);
                        }
                        else
                        {
                            // Remove Light by choosing randomly
                            gen.getSpawnPoints().RemoveAt(pos_light[Util.MyRandom.getRandom().random().Next(pos_light.Count)]);
                        }

                    }
                    else
                    {
                        // Can only add light!
                        int[] new_light = { 4, pickedRoom };
                        gen.getSpawnPoints().Add(new_light);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void setLogger(ILogger log)
        {
            this.logger = log;
        }

        public void setMutationRate(double mutation_rate)
        {
            this.mutationRate = mutation_rate;
        }

        public List<int> isLightInRoom(Genotype gen, int roomID)
        {
            List<int> pos = new List<int>();

            for (int i = 0; i < gen.getSpawnPoints().Count; i++)
            {
                if (gen.getSpawnPoints()[i][0] == 4 && gen.getSpawnPoints()[i][0] == roomID)
                {
                    pos.Add(i);
                }
            }
            return pos;
        }
    }
}
