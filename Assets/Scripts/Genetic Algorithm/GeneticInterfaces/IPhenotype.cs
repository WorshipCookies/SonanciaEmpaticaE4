using ProjectMaze.LogSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.GeneticInterfaces
{
    public interface IPhenotype
    {

        void setFitness(double fitness);

        void setLogger(ILogger log);

        double getFitness();

    }
}
