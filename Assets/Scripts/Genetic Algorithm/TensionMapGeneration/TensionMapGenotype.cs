using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.TensionMapGeneration
{
    public class TensionMapGenotype : IGenotype
    {
        public ILogger logger;
        public static double INC_TENSION = 0.25;

        private double[] tensionMap;
        private int maxRooms;

        public TensionMapGenotype(int maxRooms)
        {
            this.tensionMap = new double[maxRooms];
            this.maxRooms = maxRooms;
        }

        public TensionMapGenotype(TensionMapPhenotype phen)
        {
            this.tensionMap = phen.getTensionMap();
            this.maxRooms = phen.getMaxRooms();
        }

        public void setPhenotype(TensionMapPhenotype phen)
        {
            this.tensionMap = phen.getTensionMap();
            this.maxRooms = phen.getMaxRooms();
        }

        public void setLogger(ILogger log)
        {
            this.logger = log;
        }

        public double[] getTensionMap()
        {
            return tensionMap;
        }

        public void addTension(int index)
        {
            tensionMap[index] += INC_TENSION;
        }

        public void removeTension(int index)
        {
            tensionMap[index] -= INC_TENSION;
        }

        public int getMaxRooms()
        {
            return maxRooms;
        }

        public ILogger getLogger()
        {
            return logger;
        }

    }
}
