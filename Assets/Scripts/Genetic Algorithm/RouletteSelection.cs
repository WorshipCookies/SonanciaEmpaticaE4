using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.GeneticInterfaces;
using ProjectMaze.Util;
using ProjectMaze.LogSystem;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.GeneticAlgorithm
{
    public class RouletteSelection : ISelection
    {

        private int elitist_num;
        private ILogger logger;
        private static string name_ID = "Roulette Selection";

        public RouletteSelection(int elitist_num)
        {
            this.elitist_num = elitist_num;
            this.logger = null;
        }

        public void setElitism(int choice)
        {
            this.elitist_num = choice;
        }

        public int isElitism()
        {
            return elitist_num;
        }

        public IPhenotype selectIndividual(IPhenotype[] population)
        {
            double[] normalized_fitValues = normalizeValue(population); // Normalize values.
            return population[rouletteSelection(normalized_fitValues)];
        }

        public IPhenotype[] selectIndividuals(IPhenotype[] population)
        {
            double[] normalized_fitValues = normalizeValue(population); // Normalize values.
            Phenotype[] selected_individuals = new Phenotype[population.Length - elitist_num]; // Don't select individuals we don't need in case of elitism.

            if(logger != null)
            {
                // Log the Roulette Selection Process!
                logger.writeLogTimeStamp(" ------- " + name_ID + " ------- \n");
                for (int i = 0; i < selected_individuals.Length; i++)
                {
                    logger.writeLog("For Individual: " + i + "; ");
                    selected_individuals[i] = (Phenotype)population[rouletteSelection(normalized_fitValues)];
                    logger.writeLog("\n");
                }
            }
            else
            {
                for (int i = 0; i < selected_individuals.Length; i++)
                {
                    selected_individuals[i] = (Phenotype)population[rouletteSelection(normalized_fitValues)];
                }
            }
            return selected_individuals;
        }

        public void setLogger(LogSystem.Interfaces.ILogger log)
        {
            this.logger = log;
        }

        // Normalizes fitness values!
        public double[] normalizeValue(IPhenotype[] population)
        {
            double[] normalized_fitValues = new double[population.Length];
            double sum_value = GeneralFunctions.sumAllFitnessPopulation(population);

            if (Double.IsNaN(sum_value))
            {
                Console.Write("");
            }

            // Normalize all values in the population.
            for (int i = 0; i < population.Length; i++)
            {
                normalized_fitValues[i] = (population[i].getFitness() / sum_value);
            }

            // For Debugging Purposes only!
            double total = 0;
            foreach (double d in normalized_fitValues)
            {
                total += d;
            }
            if (Math.Round(total, 0) != 1)
                Console.WriteLine("NORMALIZING BUG!");

            return normalized_fitValues;
        }

        // Return the position of the selected individual
        public int rouletteSelection(double[] normalizedValues)
        {
            double stop = MyRandom.getRandom().random().NextDouble();
            double spinner = 0;

            int chosen = -1;

            if (logger != null) // For Logging Purposes!
            {
                logger.writeLog("Stop Value: " + stop + ";");
                while (spinner < stop)
                {
                    chosen++;
                    spinner += normalizedValues[chosen];
                }
                logger.writeLog("Previous Chosen " + chosen + ";");
            }
            else
            {
                while (spinner < stop)
                {
                    chosen++;
                    spinner += normalizedValues[chosen];
                }
            }

            return chosen;
        }
    }
}
