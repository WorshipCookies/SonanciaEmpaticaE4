
using ProjectMaze.GeneticInterfaces;
using System.Collections;
using ProjectMaze.LogSystem.Interfaces;
using System;

namespace ProjectMaze.GeneticInterfaces
{
    public interface ITensionFitness : IFitness
    {
        string GetFitnessName();
    }
}