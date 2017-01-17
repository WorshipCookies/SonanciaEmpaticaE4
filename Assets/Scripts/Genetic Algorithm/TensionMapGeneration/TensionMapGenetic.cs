using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem;
using ProjectMaze.LogSystem.Interfaces;
using ProjectMaze.Util;

namespace ProjectMaze.TensionMapGeneration
{
    public class TensionMapGenetic : IGeneticAlgorithm
    {

        private int population_size;
        private double mutation_rate;
        private int totalRooms;

        private TensionMapGenotype[] geno_population;
        private TensionMapPhenotype[] pheno_population;
        private ISelection selection;
        private ICrossover crossover;
        private IFitness fitness;
        private List<IMutation> mutations;

        // Logging System
        private TensionMapLogger logger;


        public TensionMapGenetic(int population_size, double mutationRate, ISelection select, ICrossover crossover, int totalRooms,
            IFitness fitness, List<IMutation> mutation)
        {
            this.population_size = population_size;
            this.selection = select;
            this.crossover = crossover;
            this.fitness = fitness;
            this.mutations = mutation;
            this.mutation_rate = mutationRate;
            this.totalRooms = totalRooms;

            geno_population = new TensionMapGenotype[population_size];
            pheno_population = new TensionMapPhenotype[population_size];
            //this.createLogger(folderManager);

            initializePopulation();
        }

        public TensionMapGenetic(int population_size, double mutationRate, ISelection select, ICrossover crossover, int totalRooms,
            IFitness fitness, List<IMutation> mutation, FolderManagement folderManager)
        {
            this.population_size = population_size;
            this.selection = select;
            this.crossover = crossover;
            this.fitness = fitness;
            this.mutations = mutation;
            this.mutation_rate = mutationRate;
            this.totalRooms = totalRooms;

            geno_population = new TensionMapGenotype[population_size];
            pheno_population = new TensionMapPhenotype[population_size];
            this.createLogger(folderManager);

            initializePopulation();
        }

        // Initializes the population with random values
        public void initializePopulation()
        {
            for(int i = 0; i < pheno_population.Length; i++)
            {
                pheno_population[i] = new TensionMapPhenotype(totalRooms);
                geno_population[i] = new TensionMapGenotype(pheno_population[i]);
                pheno_population[i].setFitness(fitness.evaluate(pheno_population[i]));
            }

            // Order pheno population from fittest individual to unfittest.
            GeneralFunctions.sortPopulation(pheno_population);
        }
        
        public void createLogger(FolderManagement folderManager)
        {
            // Logger Initialization
            //folderManager.setNewRunFolder(Convert.ToString(run_num)); // Set Run Number for Folder.
            folderManager.setNewGenerationFolder(Convert.ToString(0));

            this.logger = new TensionMapLogger(folderManager);
            this.logger.changeFile("LogTensionMapData");

            // Set Logger for All!!
            this.selection.setLogger(this.logger);
            this.fitness.setLogger(this.logger);
            foreach (IMutation mutation in this.mutations)
            {
                mutation.setLogger(this.logger);
            }


        }

        public double getAvgFitness()
        {
            throw new NotImplementedException();
        }

        public double getBestFitness()
        {
            throw new NotImplementedException();
        }

        public IPhenotype getBestIndividual()
        {
            return pheno_population[0];
        }

        public List<IPhenotype> getBestIndividuals(int num)
        {
            throw new NotImplementedException();
        }

        public IFitness getFitnesFunction()
        {
            return this.fitness;
        }

        public IPhenotype getIndividual(int index)
        {
            if (index < pheno_population.Length)
            {
                return pheno_population[index];
            }
            return null;
        }

        public double getWorseFitness()
        {
            throw new NotImplementedException();
        }

        public void run()
        {
            // Clone the phenotypes for the selection method.
            TensionMapPhenotype[] next_gen = new TensionMapPhenotype[population_size];
            TensionMapPhenotype[] clonePrevious = new TensionMapPhenotype[population_size];

            // If there is elitism add the best individuals to the next gen automatically.
            for(int i = 0; i < selection.isElitism(); i++)
            {
                next_gen[i] = pheno_population[i].clonePheno();
            }

            // Clone the entire last generation
            for(int i = 0; i < pheno_population.Length; i++)
            {
                clonePrevious[i] = pheno_population[i].clonePheno();
            }

            int selectionCounter = selection.isElitism();
            while(selectionCounter < next_gen.Length)
            {
                // Select two children
                TensionMapPhenotype c1 = ((TensionMapPhenotype)selection.selectIndividual(clonePrevious)).clonePheno();
                TensionMapPhenotype c2 = ((TensionMapPhenotype)selection.selectIndividual(clonePrevious)).clonePheno();

                // Transform them into Genotype
                TensionMapGenotype g1 = new TensionMapGenotype(c1);
                TensionMapGenotype g2 = new TensionMapGenotype(c2);

                // Apply Crossover
                if (MyRandom.getRandom().random().NextDouble() < crossover.getCrossoverRate())
                {
                    crossover.Crossover(g1, g2);
                }

                // Mutate Genotypes
                foreach (IMutation m in mutations)
                {
                    if (MyRandom.getRandom().random().NextDouble() < m.getMutationRate())
                    {
                        m.mutate(g1);
                    }

                    if (MyRandom.getRandom().random().NextDouble() < m.getMutationRate())
                    {
                        m.mutate(g2);
                    }
                }

                // Add First Individual
                if(selectionCounter < next_gen.Length)
                {
                    next_gen[selectionCounter] = new TensionMapPhenotype(g1);
                    selectionCounter++;
                }

                if(selectionCounter < next_gen.Length)
                {
                    next_gen[selectionCounter] = new TensionMapPhenotype(g2);
                    selectionCounter++;
                }

            }

            // Apply Fitness
            int counter = 0;
            for(int i = 0; i < next_gen.Length; i++)
            {
                if (logger != null)
                {
                    logger.writeLog("Individual " + counter + ": ");
                }
                next_gen[i].setFitness(fitness.evaluate(next_gen[i]));
                //Console.WriteLine("Fitness Value of Individual " + counter + " is " + p.getFitness());

                counter++;
            }
            
            // Re-Assign them to the population
            pheno_population = next_gen;

            // Order pheno population from fittest individual to unfittest.
            GeneralFunctions.sortPopulation(pheno_population);

            // Just in case keep the pheno population as geno
            for (int i = 0; i < pheno_population.Length; i++)
            {
                geno_population[i] = new TensionMapGenotype(pheno_population[i]);
            }
        }

        public void run(int num_gen)
        {
            // Gen Cycle
            int current_gen = 1;
            while (current_gen <= num_gen)
            {
                Console.WriteLine("Current Gen = " + current_gen);
                if (logger != null)
                {

                    phenoLogger();

                    // Python Logger - get fitness values of all individuals.
                    for (int i = 0; i < pheno_population.Length; i++)
                    {
                        if (i == pheno_population.Length - 1)
                        {
                            logger.writePythonLog(Convert.ToString(pheno_population[i].getFitness()) + "\n");
                        }
                        else
                        {
                            logger.writePythonLog(Convert.ToString(pheno_population[i].getFitness()) + ",");
                        }
                    }

                    logger.writePythonBestFitnessIndividual(Convert.ToString(pheno_population[0].getFitness()) + ",");

                    logger.getFolderManager().setNewGenerationFolder(Convert.ToString(current_gen));
                }
                run();
                current_gen++;
            }

            Console.WriteLine("Genetic Algorithm Complete!");

            if (logger != null)
            {

                phenoLogger();

                // Python Logger - get fitness values of all individuals.
                for (int i = 0; i < pheno_population.Length; i++)
                {
                    if (i == pheno_population.Length - 1)
                    {
                        logger.writePythonLog(Convert.ToString(pheno_population[i].getFitness()) + "\n");
                    }
                    else
                    {
                        logger.writePythonLog(Convert.ToString(pheno_population[i].getFitness()) + ",");
                    }
                }

                logger.writePythonBestFitnessIndividual(Convert.ToString(pheno_population[0].getFitness()) + "\n");

                logger.writePythonLog("NewRun\n");
                //logger.writePythonStructureFitnessLog("NewRun\n");
                //logger.writePythonMonsterFitnessLog("NewRun\n");
                //logger.writePythonBestFitnessIndividual("\n");
                //logger.writePythonBestStructureFitnessIndividual("NewRun\n");
                //logger.writePythonBestAnxietyFitnessIndividual("NewRun\n");

            }
        }

        public void phenoLogger()
        {
            for (int i = 0; i < pheno_population.Length; i++)
            {
                logger.savePhenoStructure(pheno_population[i], Convert.ToString(i));
            }
        }
    }
}
