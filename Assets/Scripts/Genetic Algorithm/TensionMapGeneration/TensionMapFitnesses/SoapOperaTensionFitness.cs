using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.TensionMapGeneration.TensionMapFitnesses
{
    /*
        This Fitness Function will give higher values to tension maps who have a mid map peak 
        and which its final room ends with a tension value higher then the mid peak.
    */
    public class SoapOperaTensionFitness : ITensionFitness
    {

        private ILogger logger;

        public SoapOperaTensionFitness()
        {

        }

        public double evaluate(IPhenotype pheno)
        {

            double value = 0;
            TensionMapPhenotype p = (TensionMapPhenotype)pheno;

            if (PeakValleyCalculation.anyPeak(p.getTensionMap()))
            {
                value += 0.5;

                double peakValue = PeakValleyCalculation.calculateSpikeHeight(p.getTensionMap());
                double finalT = p.getTensionMap()[p.getTensionMap().Length - 1];

                value += (finalT - peakValue > 0) ? 0.5: 0;

            }
            return value;
        }

        public string GetFitnessName()
        {
            return "Cliffhanger";
        }

        public void setLogger(ILogger log)
        {
            this.logger = log;
        }
    }
}
