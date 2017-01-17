using ProjectMaze.GeneticInterfaces;
using ProjectMaze.LogSystem.Interfaces;
using ProjectMaze.Visual.Spawn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.GeneticAlgorithm
{
    public class ItemMoveSpawnMutation : IMutation
    {
        private double mutationRate;

        private ILogger logger;

        private static string name_ID = "Move Spawn Mutation";

        public ItemMoveSpawnMutation(double mutationRate)
        {
            this.mutationRate = mutationRate;
            this.logger = null;
        }

        public void mutate(IGenotype geno)
        {
            try
            {
                Genotype gen = (Genotype)geno;

                // Pick Random Spawn Point
                //int pos = Util.MyRandom.getRandom().random().Next(gen.getSpawnPoints().Count);
                int pos = getRandomItemPos(gen);
                int[] chosen_spawn = gen.getSpawnPoints()[pos];

                // Pick new Room
                int new_room = DoorMutation.getRandomAdjacentID(gen, chosen_spawn[1], chosen_spawn[0]);


                if (logger != null)
                {
                    logger.writeLog("Item Move Spawn Mutation -- Moved Item from Room " + chosen_spawn[1] + " to Room " + new_room);
                }

                // Commit change.

                if(new_room >= 0)
                    gen.getSpawnPoints()[pos][1] = new_room;

                //Console.WriteLine("Commit Spawn ROOM! " + pos);

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
            logger = log;
        }

        private int getRandomItemPos(Genotype gen)
        {
            List<int> pos = new List<int>();

            for (int i = 0; i < gen.getSpawnPoints().Count; i++)
            {
                if (gen.getSpawnPoints()[i][0] == 0)
                {
                    pos.Add(i);
                }
            }
            return pos[Util.MyRandom.getRandom().random().Next(pos.Count)];
        }

    }
}
