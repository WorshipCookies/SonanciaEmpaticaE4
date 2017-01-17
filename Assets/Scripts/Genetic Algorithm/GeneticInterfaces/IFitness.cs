using ProjectMaze.LogSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.GeneticInterfaces
{
    public interface IFitness
    {

        double evaluate(IPhenotype pheno);

        void setLogger(ILogger log);

    }
}
