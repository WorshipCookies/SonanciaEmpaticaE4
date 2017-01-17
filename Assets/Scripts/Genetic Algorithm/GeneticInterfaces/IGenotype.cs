using ProjectMaze.LogSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.GeneticInterfaces
{
    public interface IGenotype
    {

        void setLogger(ILogger log);
    }
}
