using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.TensionMapGeneration.TensionMapFitnesses
{

    /*
        This fitness function will give high fitness values according to the deepest valley of a 
        tension curve.
    */
    public class RestingPointFitness : ITensionFitness
    {
        private ILogger logger;

        public RestingPointFitness()
        {

        }

        public double evaluate(IPhenotype pheno)
        {
            TensionMapPhenotype p = (TensionMapPhenotype)pheno;
            double val = PeakValleyCalculation.calculateValleyDepth(p.getTensionMap()) / TensionMapPhenotype.MAX_THRESHOLD;
            return val;
        }

        public string GetFitnessName()
        {
            return "Resting Point";
        }

        public void setLogger(ILogger log)
        {
            this.logger = log;
        }


    }
}
