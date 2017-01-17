using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.TensionMapGeneration.TensionMapFitnesses
{
    /*
        This fitness gives higher fitness to all of the rooms after the highest peak, 
        who are lower in tension then the peak. 
    */
    public class DenovementTensionFitness : ITensionFitness
    {
        private ILogger logger;

        public DenovementTensionFitness()
        {

        }
        
        public double evaluate(IPhenotype pheno)
        {
            TensionMapPhenotype p = (TensionMapPhenotype)pheno;
            int highestPeak = PeakValleyCalculation.getHighestPeakIndex(p.getTensionMap());

            if (highestPeak == -1)
            {
                return 0;
            }
            
            double value = highestPeak;
            double val = value / (p.getMaxRooms() - 2);
            return val;
        }

        public string GetFitnessName()
        {
            return "Denouement";
        }

        public void setLogger(ILogger log)
        {
            this.logger = log;
        }
    }
}
