using ProjectMaze.GeneticAlgorithm;
using ProjectMaze.GeneticInterfaces;
using ProjectMaze.LogSystem;
using ProjectMaze.TensionMapGeneration.TensionMapFitnesses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ProjectMaze.TensionMapGeneration
{
    public class TensionMapExperiment
    {

        private int num_runs;
        private int tensionMap_genNum;
        private int tensionMap_popNum;
        private TensionMapGenetic tensionGen;
        
        List<IFitness> tensionMapFitnesses = new List<IFitness>();


        public TensionMapExperiment(int num_runs, int tensionMap_genNum, int tensionMap_popNum)
        {
            this.num_runs = num_runs;
            this.tensionMap_genNum = tensionMap_genNum;
            this.tensionMap_popNum = tensionMap_popNum;

            tensionMapFitnesses = new List<IFitness>();
            tensionMapFitnesses.Add(new DecreasingTensionFitness());
            tensionMapFitnesses.Add(new DenovementTensionFitness());
            tensionMapFitnesses.Add(new EscalatingTensionFitness());
            tensionMapFitnesses.Add(new RestingPointFitness());
            tensionMapFitnesses.Add(new RiseFallTensionFitness());
            tensionMapFitnesses.Add(new SoapOperaTensionFitness());
            tensionMapFitnesses.Add(new SurprisingMomentFitness());
            tensionMapFitnesses.Add(new UnresolvedTensionFitness());

            CombinedTensionFitnesses combo = new CombinedTensionFitnesses();
            combo.AddFitness(new DecreasingTensionFitness());
            combo.AddFitness(new RestingPointFitness());
            tensionMapFitnesses.Add(combo);

            MultiplyTensionFitnesses multi = new MultiplyTensionFitnesses();
            multi.AddFitness(new DecreasingTensionFitness());
            multi.AddFitness(new RestingPointFitness());
            tensionMapFitnesses.Add(multi);

        }

        public void runExp()
        {
            foreach (IFitness f in tensionMapFitnesses)
            {

                string[] fitnessStruct = f.GetType().ToString().Split('.');
                string fitnessName = fitnessStruct[fitnessStruct.Length - 1];

                Console.WriteLine("Starting Fitness " + fitnessName);

                if(fitnessName == "CombinedTensionFitnesses" )
                {
                    CombinedTensionFitnesses aux = (CombinedTensionFitnesses)f;
                    fitnessName += "_" + aux.getFitnessNames();
                }
                else if(fitnessName == "MultiplyTensionFitnesses")
                {
                    MultiplyTensionFitnesses aux = (MultiplyTensionFitnesses)f;
                    fitnessName += "_" + aux.getFitnessNames();
                }

                FolderManagement folderManager = new FolderManagement("Experiment_" + fitnessName + " " + Stopwatch.GetTimestamp());

                // Run Cycle goes here
                for (int i = 0; i < num_runs; i++)
                {
                    folderManager.setNewRunFolder("Run " + i);

                    // TEMP CODE FOR TENSION MAP GENERATION
                    List<IMutation> tensionMutations = new List<IMutation>();
                    tensionMutations.Add(new TensionMutation(0.2));
                    tensionGen = new TensionMapGenetic(tensionMap_popNum, 0.8, new RouletteSelection(5), new OnePointCrossover(0.7), 8, f, tensionMutations, folderManager);
                    
                    // Run Tension Map Generation
                    tensionGen.run(tensionMap_genNum);
                }
            }
        }
    }
}
