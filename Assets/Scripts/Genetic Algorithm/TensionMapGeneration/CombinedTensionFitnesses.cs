using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.TensionMapGeneration
{

    // Combining different curves of tension -- This Fitness sums all of the associated fitnesses
    public class CombinedTensionFitnesses : ITensionFitness
    {
        private List<ITensionFitness> fitnesses;
        private ILogger logger;

        // Different Fitness curves!
        public CombinedTensionFitnesses()
        {
            fitnesses = new List<ITensionFitness>();
        }

        public CombinedTensionFitnesses(List<ITensionFitness> fitnesses)
        {
            this.fitnesses = fitnesses;
        }

        public void AddFitness(ITensionFitness fitness)
        {
            fitnesses.Add(fitness);
        }

        public double evaluate(IPhenotype pheno)
        {
            double val = 0;
            foreach(ITensionFitness f in fitnesses)
            {
                val += f.evaluate(pheno);
            }
            val = (val / fitnesses.Count);
            return val;
        }

        public string GetFitnessName()
        {
            return fitnesses[0].GetFitnessName() + " and " + fitnesses[1].GetFitnessName();
        }

        public string getFitnessNames()
        {
            string value = "";

            int i = 0;
            foreach(ITensionFitness f in fitnesses)
            {
                string[] fitnessStruct = f.GetType().ToString().Split('.');
                string fitnessName = fitnessStruct[fitnessStruct.Length - 1];

                if (i != fitnesses.Count - 1)
                    value += fitnessName + " and ";
                else
                    value += fitnessName + ".";

                i++;
            }
            return value;
        }

        public void setLogger(ILogger log)
        {
            this.logger = log;
        }
    }
}
