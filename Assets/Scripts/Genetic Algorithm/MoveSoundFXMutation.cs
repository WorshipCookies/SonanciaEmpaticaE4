using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.GeneticAlgorithm
{
    public class MoveSoundFXMutation : IMutation
    {

        private double mutationRate;

        private ILogger logger;

        private static string name_ID = "SoundFX Move Spawn Mutation";

        public MoveSoundFXMutation(double mutation_rate)
        {
            this.mutationRate = mutation_rate;
            this.logger = null;
        }

        public double getMutationRate()
        {
            return mutationRate;
        }

        public void mutate(IGenotype geno)
        {
            try
            {
                Genotype gen = (Genotype)geno;

                // Pick Random Spawn Point
                //int pos = Util.MyRandom.getRandom().random().Next(gen.getSpawnPoints().Count);
                int pos = getRandomSoundFXPos(gen);

                // If there are soundfx, move them around. Else just don't do anything.
                if (pos != -1)
                {
                    int[] chosen_spawn = gen.getSpawnPoints()[pos];

                    // Pick new Room
                    int new_room = DoorMutation.getRandomAdjacentID(gen, chosen_spawn[1], chosen_spawn[0]);
                    if (logger != null)
                    {
                        logger.writeLog("Sound FX Move Mutation -- Moved Light from Room " + chosen_spawn[1] + " to Room " + new_room);
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

        public void setLogger(ILogger log)
        {
            this.logger = log;
        }

        public void setMutationRate(double mutation_rate)
        {
            this.mutationRate = mutation_rate;
        }

        private int getRandomSoundFXPos(Genotype gen)
        {
            List<int> pos = new List<int>();

            for (int i = 0; i < gen.getSpawnPoints().Count; i++)
            {
                if (gen.getSpawnPoints()[i][0] == 3)
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
