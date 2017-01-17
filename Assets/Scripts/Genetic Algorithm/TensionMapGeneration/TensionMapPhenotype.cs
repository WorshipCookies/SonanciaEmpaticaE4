using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;
using ProjectMaze.Util;

namespace ProjectMaze.TensionMapGeneration
{

    public class TensionMapPhenotype : IPhenotype
    {

        public static double MAX_THRESHOLD = 3.0;

        private double fitness;
        private ILogger logger;

        private double[] tension_map;
        private int maxRooms;
        
        // Automatically initialize the phenotype.
        public TensionMapPhenotype(int maxRooms)
        {
            this.tension_map = new double[maxRooms];
            this.maxRooms = maxRooms;

            initializePhenotype();
        }

        public TensionMapPhenotype(TensionMapGenotype g)
        {
            this.tension_map = g.getTensionMap();
            this.maxRooms = g.getMaxRooms();
            this.logger = g.logger;
        }

        // USED ONLY FOR CLONING
        public TensionMapPhenotype(double[] tension_map, int maxRooms, ILogger logger)
        {
            this.tension_map = tension_map;
            this.maxRooms = maxRooms;
            this.logger = logger;
        }

        public TensionMapPhenotype clonePheno()
        {
            TensionMapPhenotype p = new TensionMapPhenotype((double[])tension_map.Clone(), maxRooms, logger);
            p.setFitness(this.fitness);
            return p;
        }

        public void initializePhenotype()
        {
            for(int i = 0; i < tension_map.Length; i++)
            {
                tension_map[i] = MyRandom.getRandom().random().Next(12)*0.25;
            }
        }

        public double getFitness()
        {
            return fitness;
        }

        public void toPhenotype(TensionMapGenotype geno)
        {
            this.tension_map = geno.getTensionMap();
            this.maxRooms = geno.getMaxRooms();
        }

        public double[] getTensionMap()
        {
            return tension_map;
        }

        public int getMaxRooms()
        {
            return maxRooms;
        }

        public ILogger getLogger()
        {
            return logger;
        }

        public void setFitness(double fitness)
        {
            this.fitness = fitness;
        }

        public void setLogger(ILogger log)
        {
            this.logger = log;
        }
    }
}
