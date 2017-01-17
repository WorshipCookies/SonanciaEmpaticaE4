using ProjectMaze.GeneticInterfaces;
using ProjectMaze.TensionMapGeneration;
using ProjectMaze.TensionMapGeneration.TensionMapFitnesses;
using ProjectMaze.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.TensionMapGeneration
{
    public class TensionPickRandomizer
    {

        List<ITensionFitness> tensionMapFitnesses;

        public TensionPickRandomizer()
        {
            tensionMapFitnesses = new List<ITensionFitness>();
            tensionMapFitnesses.Add(new DecreasingTensionFitness());
            tensionMapFitnesses.Add(new DenovementTensionFitness());
            tensionMapFitnesses.Add(new EscalatingTensionFitness());
            tensionMapFitnesses.Add(new RestingPointFitness());
            tensionMapFitnesses.Add(new RiseFallTensionFitness());
            tensionMapFitnesses.Add(new SoapOperaTensionFitness());
            tensionMapFitnesses.Add(new SurprisingMomentFitness());
            tensionMapFitnesses.Add(new UnresolvedTensionFitness());
        }


        public ITensionFitness chooseRandomFitness()
        {
            List<int> fit_index = Enumerable.Range(0, tensionMapFitnesses.Count-1).ToList();

            double randval = MyRandom.getRandom().random().NextDouble();

            List<ITensionFitness> newList = new List<ITensionFitness>();

            // Combination Fitness Function
            if (randval > 0.5)
            {
                // Pick a Fitness Randomly
                int pos1 = MyRandom.getRandom().random().Next(fit_index.Count);
                ITensionFitness fit1 = tensionMapFitnesses[fit_index[pos1]];
                fit_index.RemoveAt(pos1);

                int pos2 = MyRandom.getRandom().random().Next(fit_index.Count);
                ITensionFitness fit2 = tensionMapFitnesses[fit_index[pos2]];
                fit_index.RemoveAt(pos2);

                CombinedTensionFitnesses combo = new CombinedTensionFitnesses();
                combo.AddFitness(fit1);
                combo.AddFitness(fit2);

                return combo;

                //newList.Add(combo);

            }

            // Multiplication Fitness Function
            else
            {
                // Pick a Fitness Randomly
                int pos1 = MyRandom.getRandom().random().Next(fit_index.Count);
                ITensionFitness fit1 = tensionMapFitnesses[fit_index[pos1]];
                fit_index.RemoveAt(pos1);

                int pos2 = MyRandom.getRandom().random().Next(fit_index.Count);
                ITensionFitness fit2 = tensionMapFitnesses[fit_index[pos2]];
                fit_index.RemoveAt(pos2);

                MultiplyTensionFitnesses multi = new MultiplyTensionFitnesses();
                multi.AddFitness(fit1);
                multi.AddFitness(fit2);

                return multi;

                //newList.Add(multi);
            }
        }

        public ITensionFitness chooseOneFitnessOnly()
        {
            return tensionMapFitnesses[MyRandom.getRandom().random().Next(tensionMapFitnesses.Count)];
        }
    }
}
