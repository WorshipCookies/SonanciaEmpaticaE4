using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;
using ProjectMaze.Util;

namespace ProjectMaze.TensionMapGeneration
{
    public class OnePointCrossover : ICrossover
    {
        private double crossoverRate;
        public ILogger logger;

        public OnePointCrossover(double crossoverRate)
        {
            this.crossoverRate = crossoverRate;
        }
        
        public void Crossover(IGenotype geno1, IGenotype geno2)
        {
            TensionMapGenotype g1 = (TensionMapGenotype)geno1;
            TensionMapGenotype g2 = (TensionMapGenotype)geno2;

            int point = MyRandom.getRandom().random().Next(g1.getMaxRooms() - 1);
            
            
            TensionMapGenotype aux1 = new TensionMapGenotype(g1.getMaxRooms());
            TensionMapGenotype aux2 = new TensionMapGenotype(g2.getMaxRooms());

            for(int i = 0; i < point; i++)
            {
                aux1.getTensionMap()[i] = g1.getTensionMap()[i];
                aux2.getTensionMap()[i] = g2.getTensionMap()[i];
            }

            for(int i = point; i < g1.getMaxRooms(); i++)
            {
                aux1.getTensionMap()[i] = g2.getTensionMap()[i];
                aux2.getTensionMap()[i] = g1.getTensionMap()[i];
            }

            g1 = aux1;
            g2 = aux2;

        }

        public double getCrossoverRate()
        {
            return crossoverRate;
        }

        public void setCrossoverRate(double rate)
        {
            this.crossoverRate = rate;
        }

        public void setLogger(ILogger log)
        {
            this.logger = log;
        }
    }
}
