using ProjectMaze.LogSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.GeneticInterfaces
{
    public interface ISelection
    {

        void setElitism(int choice);

        int isElitism();

        IPhenotype selectIndividual(IPhenotype[] population);

        IPhenotype[] selectIndividuals(IPhenotype[] population);

        void setLogger(ILogger log);
    }
}
