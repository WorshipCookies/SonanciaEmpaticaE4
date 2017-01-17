using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.GeneticInterfaces
{
    public interface ICrossover : IOperator
    {
        void setCrossoverRate(double rate);

        void Crossover(IGenotype geno1, IGenotype geno2);

        double getCrossoverRate();
    }
}
