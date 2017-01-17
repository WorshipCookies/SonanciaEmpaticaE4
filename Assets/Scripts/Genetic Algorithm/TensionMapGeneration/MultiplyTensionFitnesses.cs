using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.TensionMapGeneration
{
    public class MultiplyTensionFitnesses : ITensionFitness
    {

        private List<ITensionFitness> fitnesses;
        private ILogger logger;

        public MultiplyTensionFitnesses()
        {
            fitnesses = new List<ITensionFitness>();
        }

        public MultiplyTensionFitnesses(List<ITensionFitness> fitnesses)
        {
            this.fitnesses = fitnesses;
        }

        public void AddFitness(ITensionFitness fitness)
        {
            fitnesses.Add(fitness);
        }

        public double evaluate(IPhenotype pheno)
        {
            double val = 1;
            foreach (ITensionFitness f in fitnesses)
            {
                val = val * f.evaluate(pheno);
            }
            return val;
        }

        public string GetFitnessName()
        {
            return fitnesses[0].GetFitnessName() + " or " + fitnesses[1].GetFitnessName();
        }

        public string getFitnessNames()
        {
            string value = "";

            int i = 0;
            foreach (ITensionFitness f in fitnesses)
            {
                string[] fitnessStruct = f.GetType().ToString().Split('.');
                string fitnessName = fitnessStruct[fitnessStruct.Length - 1];

                if (i != fitnesses.Count - 1)
                    value += fitnessName + " or ";
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
