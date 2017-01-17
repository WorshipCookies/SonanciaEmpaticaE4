using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;
using ProjectMaze.Util;

namespace ProjectMaze.TensionMapGeneration
{
    public class TensionMutation : IMutation
    {

        private double mutationRate;
        private ILogger logger;

        private static string name_ID = "Tension Curve Mutation";

        public TensionMutation(double mutationRate)
        {
            this.mutationRate = mutationRate;
        }

        public double getMutationRate()
        {
            return mutationRate;
        }

        public void mutate(IGenotype geno)
        {
            try
            {
                TensionMapGenotype g = (TensionMapGenotype)geno;

                // This mutation will pick one random position in the array and will rise or lower tension
                int pos = MyRandom.getRandom().random().Next(g.getMaxRooms());

                if (g.getTensionMap()[pos] == 0)
                {
                    g.addTension(pos);
                }
                else if (g.getTensionMap()[pos] == TensionMapPhenotype.MAX_THRESHOLD)
                {
                    g.removeTension(pos);
                }
                else
                {
                    double rand = MyRandom.getRandom().random().NextDouble();
                    if (rand > 0.5)
                    {
                        g.addTension(pos);
                    }
                    else
                    {
                        g.removeTension(pos);
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
    }
}
