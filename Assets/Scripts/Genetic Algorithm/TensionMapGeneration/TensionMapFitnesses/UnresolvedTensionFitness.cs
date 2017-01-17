using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.TensionMapGeneration.TensionMapFitnesses
{
    /*
        This Fitness function will give high fitness values to rooms who have the same 
        tension value of the previous room.
    */

    public class UnresolvedTensionFitness : ITensionFitness
    {
        private ILogger logger;

        public UnresolvedTensionFitness()
        {

        }
        
        public double evaluate(IPhenotype pheno)
        {
            TensionMapPhenotype p = (TensionMapPhenotype)pheno;
            double value = 0;
            
            for (int i = 0; i < p.getTensionMap().Length-1; i++)
            {
                if(p.getTensionMap()[i] == p.getTensionMap()[i + 1])
                {
                    value++;
                }
            }
            double val = value / (p.getMaxRooms() - 1);
            return val;
        }

        public string GetFitnessName()
        {
            return "Unresolved";
        }

        public void setLogger(ILogger log)
        {
            this.logger = log;
        }
    }
}
