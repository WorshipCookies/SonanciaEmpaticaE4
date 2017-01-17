using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ProjectMaze.TensionMapGeneration;
using ProjectMaze.GeneticInterfaces;
using System.Collections.Generic;
using ProjectMaze.GeneticAlgorithm;
using UnityEngine.SceneManagement;
using ProjectMaze.Util;

public class TensionGraphGeneration : MonoBehaviour {

    private Text narrativeDisplay;
    private Text generationDisplay;

    private int generation;
    
    public int tensionMap_popNum = 100;
    public int tensionMap_genNum = 100;

    private TensionMapGenetic tensionGen;

    // Use this for initialization
    void Start () {

        generation = 0; 

        narrativeDisplay = GameObject.Find("ChosenNarrative").GetComponent<Text>();
        generationDisplay = GameObject.Find("Percentage").GetComponent<Text>();

        narrativeDisplay.text = "";
        generationDisplay.text = "0%";

        // Choose Fitnesses
        double randVal = MyRandom.getRandom().random().NextDouble();

        TensionPickRandomizer fitRandomizer = new TensionPickRandomizer();
        if (randVal < 0.5)
        {
            // No Combination
            ITensionFitness fit = fitRandomizer.chooseOneFitnessOnly();
            narrativeDisplay.text = fit.GetFitnessName();

            List<IMutation> tensionMutations = new List<IMutation>();
            tensionMutations.Add(new TensionMutation(0.2));
            tensionGen = new TensionMapGenetic(tensionMap_popNum, 0.8, new RouletteSelection(5), new OnePointCrossover(0.7), 8, fit, tensionMutations);
        }
        else
        {
            // Combination
            ITensionFitness tensionMapFitnesses = fitRandomizer.chooseRandomFitness();
            List<IMutation> tensionMutations = new List<IMutation>();
            tensionMutations.Add(new TensionMutation(0.2));


            narrativeDisplay.text = tensionMapFitnesses.GetFitnessName();
            tensionGen = new TensionMapGenetic(tensionMap_popNum, 0.8, new RouletteSelection(5), new OnePointCrossover(0.7), 8, tensionMapFitnesses, tensionMutations);
            
        }
    }

    // Update is called once per frame
    void Update ()
    {
        if(generation < tensionMap_genNum)
        {
            // Run Tension Map Generation
            tensionGen.run();
            generation++;
            generationDisplay.text = generation + " %";
        }
        else
        {
            // Complete Generation Run the level
            RunLevelGeneration();
        }
	}


    void RunLevelGeneration()
    {
        LevelBuilder.tension_map = ((TensionMapPhenotype)tensionGen.getBestIndividual()).getTensionMap();
        SceneManager.LoadScene("UIScene");
    }
}
