using ProjectMaze.GeneticInterfaces;
using ProjectMaze.LogSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.GeneticAlgorithm
{
    public class PlaceMonsterMutation : IMutation
    {
        private double mutationRate;
        private ILogger logger;

        private static string name_ID = "Monster Place Spawn Mutation";

        private static int MAX_MONSTER_PER_ROOM = 1;

        public PlaceMonsterMutation(double mutation_rate)
        {
            this.mutationRate = mutation_rate;
            this.logger = null;
        }

        public void mutate(IGenotype geno)
        {
            // This mutation adds or removes a monster from a room (i.e. Max Number of Monsters per Room = 1).

            // Choose a random room. If a monster is in it remove, if not add!
            try
            {
                Genotype gen = (Genotype)geno;
                
                // Get All Room Id's currently Available.
                List<int> roomIds = gen.getAllRoomIDs();

                // Pick a room Id at random
                int pickedRoom = roomIds[Util.MyRandom.getRandom().random().Next(roomIds.Count)];

                // If a Monster is already in that room remove it; if not add it!
                List<int> pos_monster = isMonsterInRoom(gen, pickedRoom);

                if (pos_monster.Count >= MAX_MONSTER_PER_ROOM)
                {
                    // If a monster exists remove it!
                    if (MAX_MONSTER_PER_ROOM > 1)
                    {
                        //Choose randomly
                        gen.getSpawnPoints().RemoveAt(pos_monster[Util.MyRandom.getRandom().random().Next(pos_monster.Count)]);
                    }
                    else
                    {
                        gen.getSpawnPoints().RemoveAt(pos_monster[0]); // Remove the monster!
                    }
                }
                else
                {
                    if (MAX_MONSTER_PER_ROOM > 1 && pos_monster.Count > 0)
                    {
                        // 50-50 chance of adding one more monster or removing it
                        if (Util.MyRandom.getRandom().random().NextDouble() < 0.5)
                        {
                            // Add Monster
                            int[] new_monster = { 1, pickedRoom };
                            gen.getSpawnPoints().Add(new_monster);
                        }
                        else
                        {
                            // Remove Monster by choosing randomly
                            gen.getSpawnPoints().RemoveAt(pos_monster[Util.MyRandom.getRandom().random().Next(pos_monster.Count)]);
                        }

                    }
                    else
                    {
                        // Can only add monster!
                        int[] new_monster = { 1 , pickedRoom };
                        gen.getSpawnPoints().Add(new_monster);
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

        public List<int> isMonsterInRoom(Genotype gen, int roomID)
        {
            List<int> pos = new List<int>();

            for (int i = 0; i < gen.getSpawnPoints().Count; i++)
            {
                if (gen.getSpawnPoints()[i][0] == 1 && gen.getSpawnPoints()[i][1] == roomID)
                {
                    pos.Add(i);
                }
            }
            return pos;
        }

    }
}
