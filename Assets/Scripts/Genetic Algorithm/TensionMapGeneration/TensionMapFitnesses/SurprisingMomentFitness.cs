using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.TensionMapGeneration.TensionMapFitnesses
{
    /*
        This fitness function will give high fitness values according to the highest peak of a 
        tension curve.
    */
    public class SurprisingMomentFitness : ITensionFitness
    {
        private ILogger logger;

        public SurprisingMomentFitness()
        {

        }
        
        public double evaluate(IPhenotype pheno)
        {
            TensionMapPhenotype p = (TensionMapPhenotype)pheno;
            double val = PeakValleyCalculation.calculateSpikeHeight(p.getTensionMap()) / TensionMapPhenotype.MAX_THRESHOLD;
            return val;
        }

        public string GetFitnessName()
        {
            return "Surprising Moment";
        }

        public void setLogger(ILogger log)
        {
            this.logger = log;
        }
    }
}
