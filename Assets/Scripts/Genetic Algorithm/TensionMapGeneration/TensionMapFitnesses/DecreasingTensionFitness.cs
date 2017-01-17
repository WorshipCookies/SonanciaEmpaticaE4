using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.TensionMapGeneration.TensionMapFitnesses
{

    /*
        This Fitness will give high fitness values to levels who have the most amount of rooms, in which 
        tension is lower then the previous room.
    */
    public class DecreasingTensionFitness : ITensionFitness
    {
        private ILogger logger;

        public DecreasingTensionFitness()
        {

        }

        public double evaluate(IPhenotype pheno)
        {
            // For every room where t increases + 1/totalRooms
            double val = 0;

            TensionMapPhenotype p = (TensionMapPhenotype)pheno;

            for (int i = 1; i < p.getMaxRooms(); i++)
            {
                if (p.getTensionMap()[i - 1] > p.getTensionMap()[i])
                {
                    val += 1;
                }
            }
            double fit = val / (p.getMaxRooms() - 1);
            return fit;
        }

        public string GetFitnessName()
        {
            return "Decreasing";
        }

        public void setLogger(ILogger log)
        {
            this.logger = log;
        }

    }
}
