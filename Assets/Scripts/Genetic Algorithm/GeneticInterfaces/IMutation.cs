using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.GeneticInterfaces
{
    public interface IMutation : IOperator
    {

        void mutate(IGenotype geno);

        void setMutationRate(double mutation_rate);

        double getMutationRate();

    }
}
