using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.TensionMapGeneration.TensionMapFitnesses
{

    /*
        This Fitness Function gives a high fitness to tension maps with the highest number of peaks. 
    */
    public class RiseFallTensionFitness : ITensionFitness
    {

        public ILogger logger;

        public RiseFallTensionFitness()
        {

        }

        public double evaluate(IPhenotype pheno)
        {
            TensionMapPhenotype p = (TensionMapPhenotype)pheno;
            int numPeak = PeakValleyCalculation.totalPeaks(p.getTensionMap());
            double norm = (p.getMaxRooms() - 1) / 2;
            norm = Math.Floor(norm); // For Normalization Specifically
            double val = numPeak / norm;
            return val;

        }

        public string GetFitnessName()
        {
            return "Rise and Fall";
        }

        public void setLogger(ILogger log)
        {
            this.logger = log;
        }
    }
}
