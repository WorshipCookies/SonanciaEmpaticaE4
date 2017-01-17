using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.GeneticInterfaces;
using ProjectMaze.Util;

namespace ProjectMaze.GeneticAlgorithm
{
    public class GreedySelection : ISelection
    {

        private int elitism;
        private int selection_num;

        public GreedySelection(int elitism, int selection_num)
        {
            this.elitism = elitism;
            this.selection_num = selection_num;
        }

        public void setLogger(LogSystem.Interfaces.ILogger log)
        {
            throw new NotImplementedException();
        }


        public IPhenotype[] selectIndividuals(IPhenotype[] population)
        {
            //sortPopulation(population);
            Phenotype[] selected = new Phenotype[population.Length];

            //  The Greedy Selection pick the top selection_num fittest solutions 
            int counter = 0;
            while (counter < selection_num && counter < population.Length)
            {
                selected[counter] = (Phenotype)population[counter];
                counter++;
            }

            // If population not full, randomly pick an individual who is part of the top half of fittest individuals in the population
            while (counter < population.Length)
            {
                selected[counter] = (Phenotype)population[MyRandom.getRandom().random().Next(population.Length/2)];
                counter++;
            }
            return selected;
        }

        public void setElitism(int choice)
        {
            this.elitism = choice;
        }

        public int isElitism()
        {
            return elitism;
        }

        public IPhenotype selectIndividual(IPhenotype[] population)
        {
            return selectIndividuals(population)[0];
        }
    }
}
