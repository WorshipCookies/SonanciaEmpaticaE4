using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Util
{
    public class GeneralFunctions
    {

        public static void sortPopulation(IPhenotype[] population)
        {
            for (int i = 0; i < population.Length; i++)
            {
                for (int j = 0; j < population.Length; j++)
                {
                    if (population[j].getFitness() < population[i].getFitness())
                    {
                        // Do Swap
                        IPhenotype swapper = population[j];
                        population[j] = population[i];
                        population[i] = swapper;
                    }
                }
            }
        }

        public static void sortPopulationDescending(IPhenotype[] population)
        {
            for (int i = 0; i < population.Length; i++)
            {
                for (int j = 0; j < population.Length; j++)
                {
                    if (population[j].getFitness() > population[i].getFitness())
                    {
                        // Do Swap
                        IPhenotype swapper = population[j];
                        population[j] = population[i];
                        population[i] = swapper;
                    }
                }
            }
        }

        public static double normalizeValue(double value, double max_value, double min_value)
        {
            return ((value - min_value) / (max_value - min_value));
        }

        public static double sumAllFitnessPopulation(IPhenotype[] p)
        {
            double sum = 0.0;

            foreach (IPhenotype phen in p)
            {
                if (Double.IsNaN(phen.getFitness()))
                {
                    Console.Write("");
                }

                sum += phen.getFitness();
            }

            return sum;
        }

        // Normalized vector value between two points, going from (x1,y1) to (x2,y2).
        public static double[] vectorNormalization(int x1, int y1, int x2, int y2)
        {
            int new_x = x2 - x1;
            int new_y = y2 - y1;

            double v_unit = Math.Sqrt(Math.Pow(new_x, 2) + Math.Pow(new_y, 2));

            double[] u_value = { (new_x / v_unit), (new_y / v_unit) };

            return u_value;

        }

        public static double calculateDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }
    }
}
