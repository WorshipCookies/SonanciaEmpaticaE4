using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.GeneticInterfaces;
using ProjectMaze.Util;
using ProjectMaze.LogSystem.Interfaces;
using ProjectMaze.LogSystem;

namespace ProjectMaze.GeneticAlgorithm
{
    public class MapGenetic : IGeneticAlgorithm
    {

        private int population_size;
        private int gen_num;
        private double mutation_rate;
        private Genotype[] geno_population;
        private Phenotype[] pheno_population;
        private ISelection selection;
        private IFitness fitness;
        private List<IMutation> mutations;

        // Map Specific variables
        private int map_height;
        private int map_width;

        // Logging System
        private ILogger logger;

        public MapGenetic(int population_size, double mutationRate, int map_height, int map_width, ISelection select, IFitness fitness, IMutation mutation, bool is_static)
        {
            this.population_size = population_size;
            this.selection = select;
            this.fitness = fitness;
            this.mutations = new List<IMutation>();
            this.mutations.Add(mutation);
            this.mutation_rate = mutationRate;
            this.map_height = map_height;
            this.map_width = map_width;

            this.gen_num = 0;
            geno_population = new Genotype[population_size];
            pheno_population = new Phenotype[population_size];

            this.logger = null;

            initializePopulation(is_static);
            
            
        }

        public MapGenetic(int population_size, double mutationRate, int map_height, int map_width, ISelection select, IFitness fitness, List<IMutation> mutation, bool is_static)
        {
            this.population_size = population_size;
            this.selection = select;
            this.fitness = fitness;
            this.mutations = mutation;
            this.mutation_rate = mutationRate;
            this.map_height = map_height;
            this.map_width = map_width;

            this.gen_num = 0;
            geno_population = new Genotype[population_size];
            pheno_population = new Phenotype[population_size];

            this.logger = null;

            initializePopulation(is_static);


        }

        public MapGenetic(int population_size, double mutationRate, int map_height, int map_width, ISelection select, IFitness fitness, List<IMutation> mutation, Phenotype phen)
        {
            this.population_size = population_size;
            this.selection = select;
            this.fitness = fitness;
            this.mutations = mutation;
            this.mutation_rate = mutationRate;
            this.map_height = map_height;
            this.map_width = map_width;

            this.gen_num = 0;
            geno_population = new Genotype[population_size];
            pheno_population = new Phenotype[population_size];

            this.logger = null;

            initializePopulation(phen);
        }

        // Map Genetic Constructor Specific for Logging!
        public MapGenetic(int population_size, double mutationRate, int map_height, int map_width, ISelection select, 
            IFitness fitness, List<IMutation> mutation, bool is_static, FolderManagement folderManager)
        {
            this.population_size = population_size;
            this.selection = select;
            this.fitness = fitness;
            this.mutations = mutation;
            this.mutation_rate = mutationRate;
            this.map_height = map_height;
            this.map_width = map_width;

            this.gen_num = 0;
            geno_population = new Genotype[population_size];
            pheno_population = new Phenotype[population_size];
            this.createLogger(folderManager);

            initializePopulation(is_static);


        }

        public void initializePopulation(bool is_static)
        {
            for (int i = 0; i < population_size; i++)
            {
                pheno_population[i] = new Phenotype(map_height, map_width, is_static);
                geno_population[i] = new Genotype(map_height, map_width);

                if (logger != null)
                {
                    pheno_population[i].setLogger(logger);
                    geno_population[i].setLogger(logger);
                }
            }

            // Apply Fitness to each Phenotype
            int pop_num = 0;
            foreach (Phenotype p in pheno_population)
            {
                if (logger != null)
                {
                    logger.writeLog("Individual " + pop_num + ": ");
                }
                p.setFitness(fitness.evaluate(p));
                //Console.WriteLine("Fitness Value of Individual " + pop_num + " is " + p.getFitness());
                pop_num++;
            }

            // Order pheno population from fittest individual to unfittest.
            GeneralFunctions.sortPopulation(pheno_population);
        }

        public void initializePopulation(Phenotype phen)
        {
            for (int i = 0; i < population_size; i++)
            {
                pheno_population[i] = new Phenotype(phen.getMap());
                geno_population[i] = new Genotype(map_height, map_width);

                if (logger != null)
                {
                    pheno_population[i].setLogger(logger);
                    geno_population[i].setLogger(logger);
                }
            }
            // Apply Fitness to each Phenotype
            int pop_num = 0;
            foreach (Phenotype p in pheno_population)
            {
                if (logger != null)
                {
                    logger.writeLog("Individual " + pop_num + ": ");
                }
                p.setFitness(fitness.evaluate(p));
                //Console.WriteLine("Fitness Value of Individual " + pop_num + " is " + p.getFitness());
                pop_num++;
            }

            // Order pheno population from fittest individual to unfittest.
            GeneralFunctions.sortPopulation(pheno_population);
        }

        public void run()
        {

            // Re-doing the Genetic Order ( Fixing Genetic Code ) -- Not improving...
            Phenotype[] next_gen = new Phenotype[population_size]; // The new Generation Population!
            
            // Clone last generation
            for (int i = 0; i < next_gen.Length; i++)
            {
                next_gen[i] = pheno_population[i].clonePheno();
            }

            // Time to use our selection method!
            Phenotype[] selectedIndividuals = (Phenotype[])selection.selectIndividuals(next_gen);

            // Transform selected individuals into genos and then mutate!
            // Don't forget if Elitism is on, ignore the first individuals so they are untouched for the next generation
            int selected_individuals_counter = 0;
            for (int i = selection.isElitism(); i < geno_population.Length; i++)
            {
                // Transform Pheno into Geno
                geno_population[i].toGenotype(selectedIndividuals[selected_individuals_counter]);

                if (logger != null)
                {
                    logger.writeLogTimeStamp(" ------ Individual: " + i + " Mutation ------\n");
                }

                // Mutate selected_geno
                foreach (IMutation m in mutations)
                {
                    if (MyRandom.getRandom().random().NextDouble() < m.getMutationRate())
                    {
                        m.mutate(geno_population[i]);
                        //has_mutated = true;
                    }
                }

                // Transform mutated geno back to pheno.
                next_gen[i].toPhenotype(geno_population[i]);
                selected_individuals_counter++;
            }

            // Mutation
            //foreach (Genotype geno in geno_population)
            //{
            //    if (mutations.Count < 1)
            //    {
            //        mutations[0].mutate(geno);
            //    }
            //    else
            //    {
            //        bool has_mutated = false;
            //        foreach (IMutation m in mutations)
            //        {
            //            if (MyRandom.getRandom().random().NextDouble() < m.getMutationRate() && !has_mutated)
            //            {
            //                m.mutate(geno);
            //                //has_mutated = true;
            //            }
            //        }
            //    }
            //}

            // Apply Fitness
            int counter = 0;
            foreach (Phenotype p in next_gen)
            {
                if (logger != null)
                {
                    logger.writeLog("Individual " + counter + ": ");
                }
                p.setFitness(fitness.evaluate(p));
                //Console.WriteLine("Fitness Value of Individual " + counter + " is " + p.getFitness());
                
                counter++;
            }

            pheno_population = next_gen;

            // Order pheno population from fittest individual to unfittest.
            GeneralFunctions.sortPopulation(pheno_population);

            // Transform population back to Phenotype and then evaluate!
            //for (int i = 0; i < population_size; i++)
            //{
            //    pheno_population[i].toPhenotype(geno_population[i]);
            //    pheno_population[i].setFitness(fitness.evaluate(pheno_population[i]));

                
            //}

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
                    //pictureLogger(); // Pictures of previous gen individuals.

                    bestIndividualPictureLogger();
                    phenoLogger();

                    // Python Logger - get fitness values of all individuals.
                    for (int i = 0; i < pheno_population.Length; i++)
                    {
                        if (i == pheno_population.Length - 1)
                        {
                            logger.writePythonLog(Convert.ToString(pheno_population[i].getFitness()) + "\n");
                            logger.writePythonStructureFitnessLog(Convert.ToString(pheno_population[i].getMultiPathFitness() + "\n"));
                            logger.writePythonMonsterFitnessLog(Convert.ToString(pheno_population[i].getMonsterFitness() + "\n"));
                        }
                        else
                        {
                            logger.writePythonLog(Convert.ToString(pheno_population[i].getFitness()) + ",");
                            logger.writePythonStructureFitnessLog(Convert.ToString(pheno_population[i].getMultiPathFitness() + ","));
                            logger.writePythonMonsterFitnessLog(Convert.ToString(pheno_population[i].getMonsterFitness() + ","));
                        }
                    }

                    logger.writePythonBestFitnessIndividual(Convert.ToString(pheno_population[0].getFitness()) + ",");
                    logger.writePythonBestStructureFitnessIndividual(Convert.ToString(pheno_population[0].getMultiPathFitness() + ","));
                    logger.writePythonBestAnxietyFitnessIndividual(Convert.ToString(pheno_population[0].getMonsterFitness() + ","));
                    
                    logger.getFolderManager().setNewGenerationFolder(Convert.ToString(current_gen));
                }
                run();
                current_gen++;
            }

            Console.WriteLine("Genetic Algorithm Complete!");


            if (logger != null)
            {
                //pictureLogger(); // Pictures of previous gen individuals.

                bestIndividualPictureLogger();
                phenoLogger();

                // Python Logger - get fitness values of all individuals.
                for (int i = 0; i < pheno_population.Length; i++)
                {
                    if (i == pheno_population.Length - 1)
                    {
                        logger.writePythonLog(Convert.ToString(pheno_population[i].getFitness()) + "\n");
                        logger.writePythonStructureFitnessLog(Convert.ToString(pheno_population[i].getMultiPathFitness() + "\n"));
                        logger.writePythonMonsterFitnessLog(Convert.ToString(pheno_population[i].getMonsterFitness() + "\n"));
                    }
                    else
                    {
                        logger.writePythonLog(Convert.ToString(pheno_population[i].getFitness()) + ",");
                        logger.writePythonStructureFitnessLog(Convert.ToString(pheno_population[i].getMultiPathFitness() + ","));
                        logger.writePythonMonsterFitnessLog(Convert.ToString(pheno_population[i].getMonsterFitness() + ","));
                    }
                }

                logger.writePythonBestFitnessIndividual(Convert.ToString(pheno_population[0].getFitness()) + "\n");
                logger.writePythonBestStructureFitnessIndividual(Convert.ToString(pheno_population[0].getMultiPathFitness() + "\n"));
                logger.writePythonBestAnxietyFitnessIndividual(Convert.ToString(pheno_population[0].getMonsterFitness() + "\n"));

                /*
                logger.writePythonLog("NewRun\n");
                logger.writePythonStructureFitnessLog("NewRun\n");
                logger.writePythonMonsterFitnessLog("NewRun\n");
                logger.writePythonBestFitnessIndividual("NewRun\n");
                logger.writePythonBestStructureFitnessIndividual("NewRun\n");
                logger.writePythonBestAnxietyFitnessIndividual("NewRun\n");
                */

            }
        }

        public List<IPhenotype> getBestIndividuals(int num)
        {
            return null;
        }

        public IPhenotype getBestIndividual()
        {
            //return null;
            //Console.WriteLine("The Fittest Individual is " + pheno_population[0].getFitness());
            return pheno_population[0]; // Just return the first one in the population which is the fittest individual.
        }

        public IPhenotype getIndividual(int index)
        {
            if (index < pheno_population.Length)
            {
                return pheno_population[index];
            }
            return null;
        }

        public double getAvgFitness()
        {
            throw new NotImplementedException();
        }

        public double getBestFitness()
        {
            throw new NotImplementedException();
        }

        public double getWorseFitness()
        {
            throw new NotImplementedException();
        }

        public IFitness getFitnesFunction()
        {
            return this.fitness;
        }

        public void createLogger(FolderManagement folderManager)
        {
            // Logger Initialization
            //folderManager.setNewRunFolder(Convert.ToString(run_num)); // Set Run Number for Folder.
            folderManager.setNewGenerationFolder(Convert.ToString(0));

            this.logger = Logger.LogFactory(folderManager);
            this.logger.changeFile("LogData");

            // Set Logger for All!!
            this.selection.setLogger(this.logger);
            this.fitness.setLogger(this.logger);
            foreach (IMutation mutation in this.mutations)
            {
                mutation.setLogger(this.logger);
            }
        }

        public void pictureLogger()
        {
            for (int i = 0; i < pheno_population.Length; i++)
            {
                logger.takePicture(pheno_population[i], Convert.ToString(i));
            }
        }

        public void bestIndividualPictureLogger()
        {
            logger.takePicture(getBestIndividual(), "BestIndividual");
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
