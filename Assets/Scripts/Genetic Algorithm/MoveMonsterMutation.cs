using ProjectMaze.GeneticInterfaces;
using ProjectMaze.LogSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.GeneticAlgorithm
{
    public class MoveMonsterMutation : IMutation
    {
        private double mutationRate;

        private ILogger logger;

        private static string name_ID = "Monster Move Spawn Mutation";

        public MoveMonsterMutation(double mutationRate)
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
                int pos = getRandomMonsterPos(gen);

                // If there are monsters, move them around. Else just don't do anything.
                if (pos != -1)
                {
                    int[] chosen_spawn = gen.getSpawnPoints()[pos];

                    // Pick new Room
                    int new_room = DoorMutation.getRandomAdjacentID(gen, chosen_spawn[1], chosen_spawn[0]);
                    if (logger != null)
                    {
                        logger.writeLog("Monster Move Mutation -- Moved Monster from Room " + chosen_spawn[1] + " to Room " + new_room);
                    }

                    // Commit change.
                    if (new_room >= 0)
                        gen.getSpawnPoints()[pos][1] = new_room;
                }
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
            this.logger = log;
        }

        private int getRandomMonsterPos(Genotype gen)
        {
            List<int> pos = new List<int>();

            for (int i = 0; i < gen.getSpawnPoints().Count; i++)
            {
                if (gen.getSpawnPoints()[i][0] == 1)
                {
                    pos.Add(i);
                }
            }

            if (pos.Count > 0)
                return pos[Util.MyRandom.getRandom().random().Next(pos.Count)];
            else
                return -1;
        }
    }
}
