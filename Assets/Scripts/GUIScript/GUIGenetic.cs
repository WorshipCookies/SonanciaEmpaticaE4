using UnityEngine;
using System.Collections;
using ProjectMaze.GeneticInterfaces;
using ProjectMaze.GeneticAlgorithm;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using ProjectMaze.Util;
using UnityEngine.SceneManagement;

public class GUIGenetic : MonoBehaviour {

    // The GA Parameters --- Go Here!
    public static double[] tension_map = Tension_Maps.Inverse_U;
    public static MapGenetic gen;
    public static int pop_num = 100;
    public static int gen_num = 100;
    public static int num_runs = 100;

    public static int HEIGHT_TILE_NUM = 14;
    public static int WIDTH_TILE_NUM = 14;
    public static int WINDOW_HEIGHT = 700; // 700; 1000; 1800
    public static int WINDOW_WIDTH = 700;  // 700; 1000; 1000 

    Text guiText;
    Text guiFit;
    int current_gen;

	// Use this for initialization
	void Start () {

        guiText = GameObject.Find("textGen").GetComponent<Text>();
        guiFit = GameObject.Find("textFit").GetComponent<Text>();
        current_gen = 1;

        Debug.Log("Going to Generate Level!");

        // Static Individual - With New Room Mutation
        //List<IMutation> mutations = new List<IMutation>();
        //mutations.Add(new AddRoomMutation(0.9));
        //mutations.Add(new SimpleMutation(0.8));
        //mutations.Add(new DoorMutation(0.9));
        //mutations.Add(new ItemMoveSpawnMutation(0.6));
        //mutations.Add(new MoveMonsterMutation(0.7));
        //mutations.Add(new PlaceMonsterMutation(0.7));

        List<IMutation> mutations = new List<IMutation>();
        mutations.Add(new AddRoomMutation(0.9));
        mutations.Add(new SimpleMutation(0.8));
        mutations.Add(new DoorMutation(0.9));
        mutations.Add(new ItemMoveSpawnMutation(0.6));
        mutations.Add(new MoveMonsterMutation(0.7));
        mutations.Add(new MoveLightMutation(0.7));
        mutations.Add(new MoveSoundFXMutation(0.7));
        mutations.Add(new PlaceMonsterMutation(0.5));
        mutations.Add(new PlaceLightMutation(0.5));
        mutations.Add(new PlaceSoundFXMutation(0.5));


        gen = new MapGenetic(pop_num, 1.0, WINDOW_HEIGHT, WINDOW_WIDTH, new RouletteSelection(3), new SimpleFitness(), mutations, true);

	}
	
	// Update is called once per frame
	void Update () {
        if (current_gen <= gen_num)
        {
            gen.run();
            double percentage = ( Convert.ToDouble(current_gen) / Convert.ToDouble(gen_num) ) * 100.0;
            
            guiText.text = percentage + "%";
            guiFit.text = "Current Fitness = " + gen.getBestIndividual().getFitness();
            current_gen++;
        }
        else
        {
            StaticPhenotype.setPhenotype((Phenotype)gen.getBestIndividual());

            ReadWriteToFile.writeToFile(StaticPhenotype.pheno, "LevelDebug.snc");
            
            SceneManager.LoadScene("buildingscenes");
        }
	}
}
