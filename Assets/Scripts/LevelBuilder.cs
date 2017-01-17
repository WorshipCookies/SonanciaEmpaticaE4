using UnityEngine;
using System.Collections;
using ProjectMaze.GeneticAlgorithm;
using ProjectMaze.GeneticInterfaces;
using System.Collections.Generic;
//using UnityEditor;
using UnityStandardAssets.Characters.FirstPerson;
using System.IO;
using UnityEngine.UI;
using ProjectMaze.Util;
using Assets.Scripts;
using ProjectMaze.Visual;
using System;
using UnityEngine.SceneManagement;

public class LevelBuilder : MonoBehaviour {

    public GameObject Player;
    public Transform tile_prefab;
    public Transform tile_model;
    public Transform wall_prefab;
    public Transform wall_model;
    public Transform door_prefab;
    public Transform roof_model;

    public Transform TwoWallCorner;
    public Transform ThreeWallCorner;

    public Transform monster_prefab;

    // Objective prefabs
    public Transform item_prefab;
    public int objectiveRoomID;
    public Transform subitem_prefab;

    public Transform soundTest_prefab;
    private ZingerDictionary zingerDictionary; // Don't confuse with soundfx dictionary. This is the available Zinger Resources in total so the algorithm may allocate them into the level.

    public bool playerSet = false;
    public static string playerID = "FPSPlayer";
    public static int playerPos = 0;

    private LevelInfo levelstruct;

    SoundManager soundManager;
    LevelObject lvl;

    Dictionary<int, Vector3[]> lookUpTable;

    // Monster Dictionary
    Dictionary<int, Vector3> monsterDictionary; // The monsters that exist within the level.
    public static Dictionary<int,Transform[]> monsterWaypointDictionary;
    public int totalMonsterPatrolWaypoints = 5; // Total number of Patrol Points.


    // Lights Dictionary
    List<int> lightDictionary; // The lights that exist within the level!

    // SoundFX Dictionary
    List<int> soundfxDictionary; // The sound fx that exist within the level!

    // The GA Parameters --- Go Here!
    public static double[] tension_map = Tension_Maps.U_Shape; // TENSION MAP YO!


    public static MapGenetic gen;
    public static int pop_num = 100;
    public static int gen_num = 100;
    public static int num_runs = 100;

    public static int HEIGHT_TILE_NUM = 14;
    public static int WIDTH_TILE_NUM = 14;
    public static int WINDOW_HEIGHT = 700; // 700; 1000; 1800
    public static int WINDOW_WIDTH = 700;  // 700; 1000; 1000 

    public static Color[] colors = { Color.black, Color.blue, Color.cyan, Color.green, Color.yellow, Color.gray, Color.magenta };

    // Number of lights per room
    public static int MIN_WALL_LIGHTS_PER_ROOM = 2;
    //public static int MIN_WALL_LIGHTS_PER_ROOM = 4;

    private Dictionary<int, List<Tuple<GameObject, int>>> wallPos;

    private AstarPath navMeshPath;

    private EventDataBase logSystem;

    // User Experiment Information
    private int experimentID;
    private int usrID;
    private string levelPath;
    public static int soundrank;
    public static int soundpick;
    public static int soundallocation;
    public static int soundplay;
    public static string levelName;
    public bool expHasStarted = false;
    
    public static bool isGamePaused = false;

    private GameObject EmpaticaClientObject;

    public static bool playerReachedEnd = false;

    private float maxLvlTime;

    private bool waitingForEmpaticaResponse;

    // Use this for initialization
    void Start () {

        // Set the Experiment with the Logistics Loader
        experimentID = LogisticLoader.EXP_ID;
        usrID = LogisticLoader.USR_ID;
        LogisticLoader.getNextLevel(out levelPath, out levelName, out soundrank, 
            out soundpick, out soundallocation, out soundplay);

        isGamePaused = false;
        playerReachedEnd = false;
        this.expHasStarted = false;

        if (levelPath == null)
        {
            // Experiment Finished Switch to End Experiment Screen!
            //EmpaticaClientObject = GameObject.Find("EmpaticaClient");
            //ICATEmpaticaBLEClient empatica = EmpaticaClientObject.GetComponent<ICATEmpaticaBLEClient>();
            //empatica.closeClientAndDisconnect();

            Application.Quit();
        } 
        else
        {
            maxLvlTime = 180f; //300f;

            Cursor.visible = false; // Hide Mouse Cursor
            //logSystem = gameObject.AddComponent<EventDataBase>();
            EmpaticaClientObject = GameObject.Find("EmpaticaClient");
            logSystem = EmpaticaClientObject.GetComponent<EventDataBase>();

            // Initialize the Zinger Dictionary for later use
            zingerDictionary = new ZingerDictionary();

            // Initialize the Monster Dictionary
            monsterDictionary = new Dictionary<int, Vector3>();
            LevelBuilder.monsterWaypointDictionary = new Dictionary<int, Transform[]>();

            // Initialize Lights Dictionary
            lightDictionary = new List<int>();

            // Initialize Sound FX Dictionary
            soundfxDictionary = new List<int>();

            navMeshPath = GetComponent<AstarPath>();

            // Do Level Generation Here
            //loadStaticLevel();
            //loadandGenerateLevel();
            loadExperimentLevel(levelPath); // LOAD LEVELS FOR THE USER EXPERIMENTS UNCOMMENT

            soundManager = new SoundManager(lvl);

            // Do after the GUI is destroyed
            Player = (GameObject)Instantiate(Player);//Instantiate(Player, new Vector3(x, 0, y), Quaternion.identity);
            Player.AddComponent<BoxCollider>();
            Player.name = playerID;
            Player.tag = "Player";

            placeObjective();

            buildNavMesh();
            calculatePathOfRoom();

            monsterInstantiation(); // After the player has been initialized

            //// Kickstart the Logging Process.
            //logSystem.createNewExperiment(Convert.ToString(experimentID), Convert.ToString(usrID), levelName);
            pauseGame();
            waitingForEmpaticaResponse = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (!waitingForEmpaticaResponse)
        {
            // if (Input.GetKeyDown(KeyCode.Escape) || playerReachedEnd || maxLvlTime < 0) -- For Debugging Only!
            if (playerReachedEnd || maxLvlTime < 0)
            {
                //Application.Quit();
                waitingForEmpaticaResponse = true;
                logSystem.stopStreaming();
            }

            if (Input.GetKeyDown(KeyCode.P) && !expHasStarted)
            {
                // Kickstart the Logging Process.
                logSystem.createNewExperiment(Convert.ToString(experimentID), Convert.ToString(usrID), levelName);
                pauseGame(); // Unpause the game
                expHasStarted = !expHasStarted;
            }

            Vector3 playerPos = Player.transform.position;

            if (maxLvlTime >= 0)
            {
                maxLvlTime -= Time.deltaTime;
                Debug.Log(Convert.ToString(Mathf.Floor(maxLvlTime)) + " seconds left.");
            }
        }
        else
        {
            //Wait for stream to pause
            if (ICATEmpaticaBLEClient.isStreaming)
            {
                Debug.Log("Waiting for Stream to Pause");
            }
            else
            {
                reloadBuilderScene();
            }
        }
        
        //Debug.Log("Current Player Position = " + Player.GetComponent<FirstPersonController>().getPlayerPos());
    }

    //private void loadStaticLevel()
    //{
    //    var path = EditorUtility.OpenFilePanel("Pick .snc File", "", "snc");

    //    ////var path = "C:\\Users\\Phil\\Google Drive\\Unity Projects\\Sonancia Level Files\\test2.snc";
    //    lvl = LevelLoader.staticLevelLoader(path);
    //    build3DLevel(lvl);
    //}

    private void loadExperimentLevel(string path)
    {
        Debug.Log("Loading from Path " + path);
        lvl = LevelLoader.staticLevelLoader(path);
        build3DLevel(lvl);
    }

    private void loadandGenerateLevel()
    {
        //Debug.Log("Going to Generate Level!");

        //// Static Individual - With New Room Mutation
        //List<IMutation> mutations = new List<IMutation>();
        //mutations.Add(new AddRoomMutation(0.9));
        //mutations.Add(new SimpleMutation(0.8));
        //mutations.Add(new DoorMutation(0.9));
        //mutations.Add(new ItemMoveSpawnMutation(0.6));
        //mutations.Add(new MoveMonsterMutation(0.7));
        //mutations.Add(new PlaceMonsterMutation(0.7));

        //gen = new MapGenetic(pop_num, 1.0, WINDOW_HEIGHT, WINDOW_WIDTH, new RouletteSelection(3), new SimpleFitness(), mutations, true);

        //gen.run(gen_num);

        lvl = LevelLoader.genLevelLoader(StaticPhenotype.pheno);
        build3DLevel(lvl);
    }

    private void build3DLevel(LevelObject lvl)
    {
        // Change the Width and Heigh depending on the level -- IMPORTANT!
        HEIGHT_TILE_NUM = lvl.getNumTileHeight();
        WIDTH_TILE_NUM = lvl.getNumTileWidth();

        SoundPlayType.PlayType playType = (SoundPlayType.PlayType)soundplay;

        int tileIndexID = 0;
        int[] staticLevel = lvl.getLevel();//StaticLevel.myInverseStaticLevel(); //StaticLevel.myStaticLevel();

        Dictionary<int, List<GameObject>> list3DTile = new Dictionary<int, List<GameObject>>(); // Lets keep the prefabs here!
        List<GameObject> walls3DList = new List<GameObject>(); // A list of the 3d walls

        wallPos = new Dictionary<int, List<Tuple<GameObject, int>>>(); // Just a Test --- 0 = Down; 1 = Up; 2 = Left; 3 = Right.

        // CRAPPY TILES --- HARDCODED SCALE.... :/
        float tileScale = tile_prefab.localScale.x; //4.8f; -- Just Testing, if not revert back.

        Debug.Log("tile Scale = " + tileScale);

        createLookUpTable(HEIGHT_TILE_NUM, WIDTH_TILE_NUM, (int)tileScale); // Create a tile lookup table to know the positions for texturing.

        List<WallCorner> corners = lvl.detectCorners();

        // For Loops determine the positioning.
        for (int y = 0; y < HEIGHT_TILE_NUM; y++)
        {
            for (int x = 0; x < WIDTH_TILE_NUM; x++)
            {
                
                float realX = x * tileScale;
                float realY = y * tileScale;

                float wallNudge = 3.77f;

                int roomID = Mathf.Abs(staticLevel[tileIndexID]);
                if (!list3DTile.ContainsKey(roomID))
                {
                    List<GameObject> aux = new List<GameObject>();
                    list3DTile.Add(roomID, aux);
                }

                if (staticLevel[tileIndexID] == 1 || staticLevel[tileIndexID] == -1)
                {

                    Transform t = Instantiate(tile_prefab, new Vector3(realX, 0, realY), Quaternion.identity) as Transform;
                    GameObject tile = t.gameObject;
                    tile.name = "tile." + tileIndexID;
                    list3DTile[roomID].Add(tile);

                    // Add just the model on top
                    Transform mod = Instantiate(tile_model, new Vector3(realX, 0, realY), Quaternion.identity) as Transform;
                    GameObject mod_f = mod.gameObject;
                    this.setToWalkable(mod_f);
                    mod_f.name = "tile_" + tileIndexID;


                    // Add the roof if not adjacent
                    if (!lvl.getGeneticMap().getMap().getAllExtremityTiles().Contains(tileIndexID)
                        || !lvl.getAdjacentTileIDs(roomID).Contains(tileIndexID))
                    {
                        Instantiate(roof_model, new Vector3(realX, 7.3f, realY), Quaternion.Euler(180f,0,0));
                    }

                    //Instantiate(startRoomTile_prefab, new Vector3(x, 0, y), Quaternion.identity).name = "tile." + tileIndexID; // Instantiate the tile prefab
                    // Player Position Set
                    if (!playerSet)
                    {
                        Player.transform.position = new Vector3(realX, 1, realY);
                        //Player = (GameObject)Instantiate(Player, new Vector3(x, 0, y), Quaternion.identity);
                        //Player.name = playerID;
                        playerSet = true;
                    }
                }
                else
                {
                    Transform t = Instantiate(tile_prefab, new Vector3(realX, 0, realY), Quaternion.identity) as Transform;
                    GameObject tile = t.gameObject;
                    tile.name = "tile." + tileIndexID;
                    list3DTile[roomID].Add(tile);

                    // Add Just the model on top
                    Transform mod = Instantiate(tile_model, new Vector3(realX, 0, realY), Quaternion.identity) as Transform;
                    GameObject mod_f = mod.gameObject;
                    this.setToWalkable(mod_f);
                    mod_f.name = "tile_" + tileIndexID;

                    // Add the roof if not adjacent
                    lvl.getAllAdjacentTiles(roomID);
                    if (!lvl.getGeneticMap().getMap().getAllExtremityTiles().Contains(tileIndexID)
                        || !lvl.getAdjacentTileIDs(roomID).Contains(tileIndexID))
                    {
                        Instantiate(roof_model, new Vector3(realX, 7.3f, realY), Quaternion.Euler(180f, 0, 0));
                    }

                    //Instantiate(tile_prefab, new Vector3(x, 0, y), Quaternion.identity).name = "tile." + tileIndexID; // Instantiate the tile prefab
                }

                // Add Walls to the Right
                if (x < WIDTH_TILE_NUM - 1)
                {
                    if (Mathf.Abs(staticLevel[tileIndexID]) != Mathf.Abs(staticLevel[tileIndexID + 1]))
                    {
                        if (staticLevel[tileIndexID] < 0 && staticLevel[tileIndexID + 1] < 0)
                        {
                            // Add Door to the Right
                            float prefabY = door_prefab.transform.position.y;
                            float prefabX = door_prefab.transform.position.x;
                            float prefabZ = door_prefab.transform.position.z;

                            // The 0.09f is because of the crappy tileset... I have to nudge them so it looks good...
                            Transform t1 = Instantiate(door_prefab, new Vector3(realX - 0.09f, prefabY, realY), Quaternion.Euler(270f,180f,0)) as Transform;
                            //GameObject f1 = t1.gameObject;
                            //this.setToWalkable(f1);

                            // Add Door to the Left
                            Transform t2 = Instantiate(door_prefab, new Vector3(realX + tileScale + 0.09f, prefabY, realY), Quaternion.Euler(270f,0f, 0)) as Transform;
                            //GameObject f2 = t2.gameObject;
                            //this.setToWalkable(f2);
                        }
                        else
                        {
                            // Add Wall to the Right
                            float XOffset = tileScale;

                            if (!WallCorner.cornerIDs.ContainsKey(tileIndexID))
                            {
                                //GameObject wall = instantiateWall(roomID, wall_prefab, new Vector3(realX, prefabY, realY), Quaternion.AngleAxis(270f, new Vector3(1.0f, 0, 0)));
                                float prefabY = wall_prefab.transform.position.y;
                                GameObject wall = instantiateWall(roomID, wall_prefab, new Vector3(realX + wallNudge, prefabY, realY), Quaternion.Euler(0f, 90f, 0f));
                                walls3DList.Add(wall);

                            }
                            else
                            {
                                if (!WallCorner.cornerIDs[tileIndexID])
                                {
                                    WallCorner corner = getCorner(tileIndexID, corners);

                                    if(corner.getType() == CornerType.TWOWALL)
                                    {
                                        float prefabY = TwoWallCorner.transform.position.y;
                                        GameObject wall = instantiateCorner(roomID, TwoWallCorner, new Vector3(realX, prefabY, realY), corner.getAngle());
                                        walls3DList.Add(wall);
                                    }
                                    else
                                    {
                                        float prefabY = ThreeWallCorner.transform.position.y;
                                        GameObject wall = instantiateCorner(roomID, ThreeWallCorner, new Vector3(realX, prefabY, realY), corner.getAngle());
                                        walls3DList.Add(wall);

                                    }
                                    WallCorner.cornerIDs[tileIndexID] = true;
                                }
                            }

                            int neighbourID = tileIndexID + 1;

                            if (!WallCorner.cornerIDs.ContainsKey(neighbourID))
                            {
                                // Due to the Tile set being irregular I need to slightly nudge it...
                                //GameObject wall2 = instantiateWall(Mathf.Abs(lvl.getLevel()[neighbourID]), wall_prefab, new Vector3(realX + tileScale - 0.2f, prefabY, realY), Quaternion.Euler(270f, 180f, 0f));

                                float prefabY = wall_prefab.transform.position.y;
                                GameObject wall2 = instantiateWall(Mathf.Abs(lvl.getLevel()[neighbourID]), wall_prefab, new Vector3(realX + tileScale - wallNudge, prefabY, realY), Quaternion.Euler(0f, 270f, 0f));
                                walls3DList.Add(wall2);
                            }
                            else
                            {
                                if (!WallCorner.cornerIDs[neighbourID])
                                {
                                    WallCorner corner = getCorner(neighbourID, corners);

                                    if (corner.getType() == CornerType.TWOWALL)
                                    {
                                        float prefabY = TwoWallCorner.transform.position.y;
                                        GameObject wall2 = instantiateCorner(Mathf.Abs(lvl.getLevel()[neighbourID]), TwoWallCorner, new Vector3(realX + tileScale, prefabY, realY), corner.getAngle());
                                        walls3DList.Add(wall2);

                                    }
                                    else
                                    {
                                        float prefabY = ThreeWallCorner.transform.position.y;
                                        GameObject wall2 = instantiateCorner(Mathf.Abs(lvl.getLevel()[neighbourID]), ThreeWallCorner, new Vector3(realX + tileScale, prefabY, realY), corner.getAngle());
                                        walls3DList.Add(wall2);
                                    }
                                    WallCorner.cornerIDs[neighbourID] = true;
                                }
                            }
                        }
                    }
                }

                // Add Walls to the bottom
                if (y < HEIGHT_TILE_NUM - 1)
                {
                    if (Mathf.Abs(staticLevel[tileIndexID]) != Mathf.Abs(staticLevel[tileIndexID + HEIGHT_TILE_NUM]))
                    {
                        if (staticLevel[tileIndexID] < 0 && staticLevel[tileIndexID + HEIGHT_TILE_NUM] < 0)
                        {

                            //Instantiate(door_prefab, new Vector3(x, 0.5f, y + 0.5f), Quaternion.AngleAxis(10f, new Vector3(0, 1.0f, 0)));

                            // Add Door on the Bottom
                            float prefabY = door_prefab.transform.position.y;
                            float prefabX = door_prefab.transform.position.x;
                            float prefabZ = door_prefab.transform.position.z;
                            Transform t1 = Instantiate(door_prefab, new Vector3(realX, prefabY, realY - 0.09f), Quaternion.Euler(270f, 90f, 0f)) as Transform;
                            //GameObject f1 = t1.gameObject;
                            //this.setToWalkable(f1);

                            // Add Door Upwards
                            Transform t2 = Instantiate(door_prefab, new Vector3(realX, prefabY, realY + tileScale + 0.09f), Quaternion.Euler(270f, 270f, 0)) as Transform;
                            //GameObject f2 = t2.gameObject;
                            //this.setToWalkable(f2);
                        }
                        else
                        {
                            // Add Wall on the bottom
                            float XOffset = tileScale;

                            if (!WallCorner.cornerIDs.ContainsKey(tileIndexID))
                            {
                                //GameObject wall = instantiateWall(roomID, wall_prefab, new Vector3(realX, prefabY, realY), Quaternion.Euler(270f, 270f, 0f));

                                float prefabY = wall_prefab.transform.position.y;
                                GameObject wall = instantiateWall(roomID, wall_prefab, new Vector3(realX, prefabY, realY + wallNudge), Quaternion.Euler(0f, 0f, 0f));
                                walls3DList.Add(wall);
                            }
                            else
                            {
                                if (!WallCorner.cornerIDs[tileIndexID])
                                {
                                    WallCorner corner = getCorner(tileIndexID, corners);

                                    if (corner.getType() == CornerType.TWOWALL)
                                    {
                                        float prefabY = TwoWallCorner.transform.position.y;
                                        GameObject wall = instantiateCorner(roomID, TwoWallCorner, new Vector3(realX, prefabY, realY), corner.getAngle());
                                        walls3DList.Add(wall);
                                    }
                                    else
                                    {
                                        float prefabY = ThreeWallCorner.transform.position.y;
                                        GameObject wall = instantiateCorner(roomID, ThreeWallCorner, new Vector3(realX, prefabY, realY), corner.getAngle());
                                        walls3DList.Add(wall);
                                    }

                                    WallCorner.cornerIDs[tileIndexID] = true;
                                }
                            }

                            int neighbourID = tileIndexID + HEIGHT_TILE_NUM;
                            if (!WallCorner.cornerIDs.ContainsKey(neighbourID))
                            {
                                // Add another wall for the adjacent room
                                //Transform w2 = Instantiate(wall_prefab, new Vector3(realX, prefabY, realY + tileScale), Quaternion.Euler(270f, 90f, 0f)) as Transform;
                                //GameObject wall2 = w2.gameObject;
                                //wall2.name = "wall." + Mathf.Abs(lvl.getLevel()[neighbourID]);


                                //this.setToUnWalkable(wall2);
                                //addWallPos(Mathf.Abs(lvl.getLevel()[neighbourID]), wall2, 1);
                                //GameObject wall2 = instantiateWall(Mathf.Abs(lvl.getLevel()[neighbourID]), wall_prefab, new Vector3(realX, prefabY, realY + tileScale - 0.2f), Quaternion.Euler(270f, 90f, 0f));

                                float prefabY = wall_prefab.transform.position.y;
                                GameObject wall2 = instantiateWall(Mathf.Abs(lvl.getLevel()[neighbourID]), wall_prefab, new Vector3(realX, prefabY, realY + tileScale - wallNudge), Quaternion.Euler(0f, 180f, 0f));
                                walls3DList.Add(wall2);
                            }
                            else
                            {
                                if (!WallCorner.cornerIDs[neighbourID])
                                {
                                    WallCorner corner = getCorner(neighbourID, corners);
                                    if (corner.getType() == CornerType.TWOWALL)
                                    {
                                        float prefabY = TwoWallCorner.transform.position.y;
                                        GameObject wall2 = instantiateCorner(Mathf.Abs(lvl.getLevel()[neighbourID]), TwoWallCorner, new Vector3(realX, prefabY, realY + tileScale), corner.getAngle());
                                        walls3DList.Add(wall2);
                                    }
                                    else
                                    {
                                        float prefabY = ThreeWallCorner.transform.position.y;
                                        GameObject wall2 = instantiateCorner(Mathf.Abs(lvl.getLevel()[neighbourID]), ThreeWallCorner, new Vector3(realX, prefabY, realY + tileScale), corner.getAngle());
                                        walls3DList.Add(wall2);
                                    }
                                    WallCorner.cornerIDs[neighbourID] = true;
                                }
                            }
                        }
                    }
                }

                //  MONSTER ADDING CODE --- COMMENT THIS OUT IF YOU DON'T WANT MONSTERS OR ITEMS IN THE LEVEL
                if (lvl.getMonsters().Contains(tileIndexID) && lvl.getItems().Contains(tileIndexID))
                {
                    //GameObject g = Instantiate(monster_prefab, new Vector3(realX, 0.05f, realY - 0.25f), Quaternion.identity) as GameObject; //Quaternion.AngleAxis(90f, new Vector3(1.0f, 0, 0)));
                    monsterDictionary.Add(roomID, new Vector3(realX, 0.05f, realY - 0.25f));
                    //Instantiate(item_prefab, new Vector3(realX, 0.2f, realY + 0.25f), Quaternion.identity);
                }
                else if ((lvl.getMonsters().Contains(tileIndexID) && lvl.getSubItems().Contains(tileIndexID)))
                {
                    //GameObject g = Instantiate(monster_prefab, new Vector3(realX, 0.05f, realY - 0.25f), Quaternion.identity) as GameObject; //Quaternion.AngleAxis(90f, new Vector3(1.0f, 0, 0)));// new Vector3(x, 0.4f, y - 0.25f)Quaternion.identity);
                    monsterDictionary.Add(roomID, new Vector3(realX, 0.05f, realY - 0.25f));
                    //Instantiate(subitem_prefab, new Vector3(realX, 0.2f, realY + 0.25f), Quaternion.identity);
                }
                else if (lvl.getMonsters().Contains(tileIndexID))
                {
                    //GameObject g = Instantiate(monster_prefab, new Vector3(realX, 0.05f, realY), Quaternion.identity) as GameObject;//Quaternion.AngleAxis(90f, new Vector3(1.0f, 0, 0)));
                    monsterDictionary.Add(roomID, new Vector3(realX, 0.05f, realY));
                }

                if (lvl.getItems().Contains(tileIndexID))
                {
                    //Instantiate(item_prefab, new Vector3(realX, 0.2f, realY), Quaternion.identity);
                    objectiveRoomID = roomID;
                }
                if (lvl.getSubItems().Contains(tileIndexID))
                {
                    //Instantiate(subitem_prefab, new Vector3(realX, 0.4f, realY), Quaternion.identity);
                }
                if (lvl.getLights().Contains(tileIndexID))
                {
                    // Thre is Light in this Room
                    lightDictionary.Add(roomID);
                    
                }

                // CHECK THE PLAY TRIGGER
                if (lvl.getSoundFX().Contains(tileIndexID) && 
                    (playType == SoundPlayType.PlayType.ALL_ON || playType == SoundPlayType.PlayType.ZINGERS_ONLY) )
                {
                    // There is a Stinger // Zinger in this Room!
                    
                    soundfxDictionary.Add(roomID);
                    
                }
                tileIndexID++;
            }
        }

        for (int i = 0; i < lvl.getLevel().Length; i++)
        {
            lvl.addTileObject(GameObject.Find("tile." + i));
        }

        // Initial Tile placement algorithm method
        int roomIDCount = 0;
        for (int y = 0; y < HEIGHT_TILE_NUM; y++)
        {
            for (int x = 0; x < WIDTH_TILE_NUM; x++)
            {
                float realX = x * tileScale;
                float realY = y * tileScale;

                float wallNudge = 3.77f;
                
                //float XOffset = wall_prefab.transform.localScale.x / 2; // This determines the offset based on the wall size. For it to work the X scale must be equal to the scale of the tile.
                if (!WallCorner.cornerIDs.ContainsKey(roomIDCount))
                {
                    if (y == 0)
                    {
                        float prefabY = wall_prefab.transform.position.y;
                        Transform w = Instantiate(wall_prefab, new Vector3(realX, prefabY, realY - wallNudge), Quaternion.Euler(0f, 180f, 0f)) as Transform;
                        GameObject wall = w.gameObject;
                        wall.name = "wall." + Mathf.Abs(lvl.getLevel()[roomIDCount]);
                        walls3DList.Add(wall);

                        this.setToUnWalkable(wall);

                        addWallPos(Mathf.Abs(lvl.getLevel()[roomIDCount]), wall, 1);

                    }
                    if (y == HEIGHT_TILE_NUM - 1)
                    {
                        float prefabY = wall_prefab.transform.position.y;
                        Transform w = Instantiate(wall_prefab, new Vector3(realX, prefabY, realY + wallNudge), Quaternion.Euler(0f, 0f, 0f)) as Transform;
                        GameObject wall = w.gameObject;
                        wall.name = "wall." + Mathf.Abs(lvl.getLevel()[roomIDCount]);
                        walls3DList.Add(wall);

                        this.setToUnWalkable(wall);

                        addWallPos(Mathf.Abs(lvl.getLevel()[roomIDCount]), wall, 0);
                    }

                    if (x == 0)
                    {
                        float prefabY = wall_prefab.transform.position.y;
                        Transform w = Instantiate(wall_prefab, new Vector3(realX - wallNudge, prefabY, realY), Quaternion.Euler(0f, 270f, 0f)) as Transform; // Required for X Axis tiles always need to rotate keep this in mind.
                        GameObject wall = w.gameObject;
                        wall.name = "wall." + Mathf.Abs(lvl.getLevel()[roomIDCount]);
                        walls3DList.Add(wall);

                        this.setToUnWalkable(wall);

                        addWallPos(Mathf.Abs(lvl.getLevel()[roomIDCount]), wall, 2);

                    }

                    if (x == WIDTH_TILE_NUM - 1)
                    {
                        float prefabY = wall_prefab.transform.position.y;
                        Transform w = Instantiate(wall_prefab, new Vector3(realX + wallNudge, prefabY, realY), Quaternion.Euler(0f, 90f, 0f)) as Transform;
                        GameObject wall = w.gameObject;
                        wall.name = "wall." + Mathf.Abs(lvl.getLevel()[roomIDCount]);
                        walls3DList.Add(wall);

                        this.setToUnWalkable(wall);

                        addWallPos(Mathf.Abs(lvl.getLevel()[roomIDCount]), wall, 3);
                    }
                }
                else
                {
                    if (!WallCorner.cornerIDs[roomIDCount])
                    {
                        WallCorner corner = getCorner(roomIDCount, corners);

                        if (corner.getType() == CornerType.TWOWALL)
                        {
                            float prefabY = TwoWallCorner.transform.position.y;
                            GameObject wall = instantiateCorner(roomIDCount, TwoWallCorner, new Vector3(realX, prefabY, realY), corner.getAngle());
                            walls3DList.Add(wall);
                        }
                        else
                        {
                            float prefabY = ThreeWallCorner.transform.position.y;
                            GameObject wall = instantiateCorner(roomIDCount, ThreeWallCorner, new Vector3(realX, prefabY, realY), corner.getAngle());
                            walls3DList.Add(wall);
                        }
                        WallCorner.cornerIDs[tileIndexID] = true;
                    }
                }
                roomIDCount++;
            }
        }

        // For testing purposes
        meshCombiner(list3DTile);
        lvl.roomMeshList = list3DTile; // Keep a copy of each room mesh in the level object. This can be useful, YO!

        //lvl.wallMeshList = meshWallCombiner(walls3DList);

        // Add ground textures now!
        foreach(int i in lvl.roomMeshList.Keys)
        {
            GameObject f = lvl.roomMeshList[i][0];
            //GameObject fRoof = lvl.roomMeshList[i][1];
            //GameObject fWall = lvl.wallMeshList[i];
            //List<int> tiles = lvl.getRoomTileIDs(i);
            //BuildTexture(lvl.getNumTileHeight(), lvl.getNumTileWidth(), (int)tileScale, f, i);
            //BuildTexture(lvl.getNumTileHeight(), lvl.getNumTileWidth(), (int)tileScale, fRoof, i);
            BuildTexture(f);
            //BuildWallTexture(fWall, fRoof);

            if (lightDictionary.Contains(i))
                addLightSources(i);

            
        }

        //NavMeshBuilder.BuildNavMesh();

        //buildNavMesh();
        //calculatePathOfRoom();

        foreach(int i in list3DTile.Keys)
        {
            list3DTile[i][0].GetComponent<Renderer>().enabled = false;
        }

        Debug.Log("Finish Loading");
    }

    public void setToUnWalkable(GameObject f)
    {
        // New Code Added -- Adds Navigation Meshes to the Floor -- Keep it here so that it isn't cloned by the "Roof" Meshes.
        //StaticEditorFlags staticFlags = GameObjectUtility.GetStaticEditorFlags(f);
        //staticFlags |= StaticEditorFlags.NavigationStatic;
        //GameObjectUtility.SetStaticEditorFlags(f, staticFlags);

        //GameObjectUtility.SetNavMeshArea(f, 1);

        f.tag = "UnWalk";
    }

    public void setToWalkable(GameObject f)
    {
        // New Code Added -- Adds Navigation Meshes to the Floor -- Keep it here so that it isn't cloned by the "Roof" Meshes.
        //StaticEditorFlags staticFlags = GameObjectUtility.GetStaticEditorFlags(f);
        //staticFlags |= StaticEditorFlags.NavigationStatic;
        //GameObjectUtility.SetStaticEditorFlags(f, staticFlags);

        f.tag = "Walk";
    }

    public void meshCombiner(Dictionary<int, List<GameObject>> tile3DList)
    {
        foreach (int i in tile3DList.Keys)
        {
            MeshFilter[] meshFilters = new MeshFilter[tile3DList[i].Count];
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            int counter = 0;
            foreach (GameObject obj in tile3DList[i])
            {
                meshFilters[counter] = obj.GetComponent<MeshFilter>();
                counter++;
            }

            for (int j = 0; j < meshFilters.Length; j++)
            {
                combine[j].mesh = meshFilters[j].mesh;
                combine[j].transform = meshFilters[j].transform.localToWorldMatrix;
                meshFilters[j].gameObject.SetActive(false);
            }
            GameObject f = new GameObject();
            f.AddComponent<MeshFilter>();
            f.AddComponent<MeshRenderer>();
            f.AddComponent<MeshCollider>();

            f.name = "Room " + i;

            f.transform.GetComponent<MeshFilter>().mesh = new Mesh();
            f.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            f.transform.gameObject.SetActive(true);

            f.transform.GetComponent<MeshCollider>().sharedMesh = f.transform.GetComponent<MeshFilter>().mesh;

            f.AddComponent<VA_Mesh>();
            f.GetComponent<VA_Mesh>().Mesh = f.transform.GetComponent<MeshFilter>().mesh;
            f.GetComponent<VA_Mesh>().MeshFilter = f.transform.GetComponent<MeshFilter>();
            f.GetComponent<VA_Mesh>().MeshCollider = f.transform.GetComponent<MeshCollider>();
            

            cleanupObjects(tile3DList[i]);

            f.transform.GetComponent<MeshCollider>().enabled = false;
            f.transform.Translate(new Vector3(0f, 1.6f, 0f));

            tile3DList[i].Clear();
            tile3DList[i].Add(f);



            //Rigidbody gameObjectsRigidBody = f.AddComponent<Rigidbody>(); // Add the rigidbody.
            //gameObjectsRigidBody.isKinematic = true; // Set the GO's mass to 5 via the Rigidbody
            //gameObjectsRigidBody.useGravity = false;
            

            // Make the Roof for this Room.
            //GameObject fRoof = (GameObject)Instantiate(f, new Vector3(f.transform.position.x, f.transform.position.y + wall_prefab.transform.localScale.y, f.transform.position.z), f.transform.rotation);
            //fRoof.name = "Roof " + i;
            //tile3DList[i].Add(fRoof);

            //// New Code Added -- Adds Navigation Meshes to the Floor -- Keep it here so that it isn't cloned by the "Roof" Meshes.
            //StaticEditorFlags staticFlags = GameObjectUtility.GetStaticEditorFlags(f);
            //staticFlags |= StaticEditorFlags.NavigationStatic;
            //GameObjectUtility.SetStaticEditorFlags(f, staticFlags);
        }
    }

    public Dictionary<int, GameObject> meshWallCombiner(List<GameObject> wall3DList)
    {
        // First Phase seperate the objects by room
        Dictionary<int, List<GameObject>> wallsOfRoom = new Dictionary<int, List<GameObject>>();
        foreach (GameObject obj in wall3DList)
        {
            int roomID = int.Parse(obj.name.Split('.')[1]); //Get the Room ID that the wall is associated to.

            if (wallsOfRoom.ContainsKey(roomID))
            {
                wallsOfRoom[roomID].Add(obj);
            }
            else
            {
                wallsOfRoom.Add(roomID, new List<GameObject>());
                wallsOfRoom[roomID].Add(obj);
            }
        }

        Dictionary<int, GameObject> wallByID = new Dictionary<int, GameObject>();

        foreach (int i in wallsOfRoom.Keys)
        {
            MeshFilter[] meshFilters = new MeshFilter[wallsOfRoom[i].Count];
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];

            int counter = 0;
            foreach (GameObject obj in wallsOfRoom[i])
            {
                meshFilters[counter] = obj.GetComponent<MeshFilter>();
                counter++;
            }

            for (int j = 0; j < meshFilters.Length; j++)
            {
                combine[j].mesh = meshFilters[j].mesh;
                combine[j].transform = meshFilters[j].transform.localToWorldMatrix;
                meshFilters[j].gameObject.SetActive(false);
            }
            GameObject f = new GameObject();
            f.AddComponent<MeshFilter>();
            f.AddComponent<MeshRenderer>();
            f.AddComponent<MeshCollider>();

            f.name = "wall."+ i;

            f.transform.GetComponent<MeshFilter>().mesh = new Mesh();
            f.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            f.transform.gameObject.SetActive(true);

            f.transform.GetComponent<MeshCollider>().sharedMesh = f.transform.GetComponent<MeshFilter>().mesh;

            f.AddComponent<VA_Mesh>();
            f.GetComponent<VA_Mesh>().Mesh = f.transform.GetComponent<MeshFilter>().mesh;
            f.GetComponent<VA_Mesh>().MeshFilter = f.transform.GetComponent<MeshFilter>();
            f.GetComponent<VA_Mesh>().MeshCollider = f.transform.GetComponent<MeshCollider>();

            //cleanupObjects(wallsOfRoom[i]);

            //wallsOfRoom[i].Clear();
            wallByID.Add(i, f);

            Rigidbody gameObjectsRigidBody = f.AddComponent<Rigidbody>(); // Add the rigidbody.
            gameObjectsRigidBody.isKinematic = true; // Set the GO's mass to 5 via the Rigidbody
            gameObjectsRigidBody.useGravity = false;

            // New Code Added -- Adds Navigation Meshes to the Floor -- Keep it here so that it isn't cloned by the "Roof" Meshes.
            //StaticEditorFlags staticFlags = GameObjectUtility.GetStaticEditorFlags(f);
            //staticFlags |= StaticEditorFlags.NavigationStatic;
            //GameObjectUtility.SetStaticEditorFlags(f, staticFlags);

            //GameObjectUtility.SetNavMeshArea(f, 1);

            


        }

        cleanupObjects(wall3DList);
        wall3DList.Clear();

        return wallByID;
    }

    public void cleanupObjects(List<GameObject> tiles)
    {
        foreach (GameObject t in tiles)
        {
            Destroy(t);
        }
    }

    public void createLookUpTable(int size_x, int size_z, int tileSize)
    {
        this.lookUpTable = new Dictionary<int, Vector3[]>();

        int numTiles = size_x * size_z;
        int numTris = numTiles * 2;

        int vsize_x = size_x + 1;
        int vsize_z = size_z + 1;
        int numVerts = vsize_x * vsize_z;

        int idCounter = 0;
        for (int z = 0; z < size_z; z++)
        {
            for (int x = 0; x < size_x; x++)
            {
                Vector3[] vertices = new Vector3[4];

                vertices[0] = new Vector3( x * tileSize, 0, z * tileSize );
                vertices[1] = new Vector3( ( x * tileSize ) + tileSize, 0, z * tileSize );
                vertices[2] = new Vector3( x * tileSize, 0, ( z * tileSize ) + tileSize );
                vertices[3] = new Vector3( ( x * tileSize ) + tileSize, 0, ( z * tileSize ) + tileSize );

                lookUpTable.Add( idCounter, vertices );
                idCounter++;
            }
        }

    }

    // THIS METHOD IS TEMPORARY AS IT IS USED TO TEST THE TEXTURING METHODOLOGY OF UNITY! APPARENTLY IT WORKS!! YUPPIE!
    public void BuildTexture(int size_x, int size_z, int tileScale, GameObject f, int j)
    {
        // TEMPORARY CODE TO TEST MATERIALS! NOT AN IDEAL SOLUTION... 
        var material = new Material(Shader.Find("Diffuse"));
        //AssetDatabase.CreateAsset(material, "Assets/Resources/Materials/testMaterial" + j + ".mat");

        //Debug.Log(AssetDatabase.GetAssetPath(material));

        MeshRenderer mesh_renderer = f.GetComponent<MeshRenderer>();
        mesh_renderer.material = material;

        Texture2D texture = new Texture2D(size_x * tileScale, size_z * tileScale);

        for (int i = 0; i < lvl.getLevel().Length; i++)
        {
            int texUpX = (int)lookUpTable[i][0].x;
            int texUpZ = (int)lookUpTable[i][0].z;

            for (int z = texUpZ; z < texUpZ * tileScale; z++)
            {
                for (int x = texUpX; x < texUpX * tileScale; x++)
                {
                    //Color p = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                    int color_index = j % (colors.Length);
                    texture.SetPixel(x, z, colors[color_index]);
                }
            }

        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        mesh_renderer.sharedMaterials[0].mainTexture = texture;
    }

    public void BuildTexture(GameObject f)
    {
        MeshRenderer mesh_renderer = f.GetComponent<MeshRenderer>();

        // Randomly selects a material, from the available materials
        int floorMaterialIndex = UnityEngine.Random.Range(0, 1);
        Material mat = Resources.Load<Material>("Textures/floor" + floorMaterialIndex);
        mesh_renderer.material = mat;
    }

    public void BuildWallTexture(GameObject wall, GameObject roof)
    {
        // TEMPORARY CODE TO TEST MATERIALS! NOT AN IDEAL SOLUTION... 
        //var material = new Material(Shader.Find("Diffuse"));
        //AssetDatabase.CreateAsset(material, "Assets/Resources/Materials/wallTestMaterial" + j + ".mat");

        //Debug.Log(AssetDatabase.GetAssetPath(material));

        MeshRenderer wall_mesh_renderer = wall.GetComponent<MeshRenderer>();
        MeshRenderer roof_mesh_renderer = roof.GetComponent<MeshRenderer>();

        // Randomly selects a material, from the available materials
        int wallMaterialIndex = UnityEngine.Random.Range(0, 1);
        Material mat = Resources.Load<Material>("Textures/wall" + wallMaterialIndex);
        wall_mesh_renderer.material = mat;
        roof_mesh_renderer.material = mat;

        // White Wall Code
        //Texture2D texture = Resources.Load<Texture2D>("Assets/Resources/Textures/wall1");
        //mesh_renderer.material.mainTexture = texture;
    }

    public void addLightSources(int roomID)
    {
        Light light = Resources.Load<Light>("Lights/Spotlight");
        light.name = "Light " + roomID;

        Vector3 pos = calcRoomCentroids(lvl, roomID);
        light.transform.position = new Vector3(pos.x, wall_prefab.localScale.y - tile_prefab.localScale.y, pos.z);
        Instantiate(light);

        //if(lightDictionary.Contains(roomID))
        //    calcWallLights(lvl, roomID);
    }

    // Adding Light Sources on walls
    public void addLightSourcesWalls(int roomID)
    {
        List<int> adjacentTiles = lvl.getAllAdjacentTiles(roomID);

        Debug.Log("Testing 123");

    }

    public static Vector3 calcRoomCentroids(LevelObject lvl, int roomID)
    {
        float avg_x = 0f;
        float avg_y = 0f;
        float avg_z = 0f;
        int total_tiles = lvl.getRoomTileIDs(roomID).Count;
        foreach (int t in lvl.getRoomTileIDs(roomID))
        {
            avg_x += lvl.getTiles(t).transform.position.x;
            avg_y += lvl.getTiles(t).transform.position.y;
            avg_z += lvl.getTiles(t).transform.position.z;
        }
        avg_x = avg_x / total_tiles;
        avg_y = (avg_y / total_tiles); // +0.5f; // Usually there are no Y-Values so lets just climb it a bit
        avg_z = avg_z / total_tiles;

        return new Vector3(avg_x, avg_y, avg_z);
    }

    public void calcWallLights(LevelObject lvl, int roomID)
    {
        Light light = Resources.Load<Light>("Lights/WallLight");
        List<Tuple<GameObject, int>> getWalls = wallPos[roomID];

        // Cycle through the 4 posible positions
        for(int i = 0; i < 4; i++)
        {
            List<Vector3> wall = new List<Vector3>();

            // Build list with all available walls for that position.
            foreach (Tuple<GameObject, int> t in getWalls)
            {
                if(t.getSecond() == i)
                {
                    wall.Add(t.getFirst().transform.position);
                }
            }

            // If wall count higher then 0 -- Add Light
            if(wall.Count > 0)
            {
                // Sort by X Value
                if(i == 0 || i == 1)
                {
                    wall.Sort((a, b) => a.x.CompareTo(b.x));
                    int val = wall.Count / 2;
                    Vector3 vect = wall[val];

                    if (i == 0)
                    {
                        vect.z -= 0.5f;   
                    } else
                    {
                        vect.z += 0.5f;
                    }
                    Instantiate(light, vect, Quaternion.identity);
                }
                // Sort by Z Value
                else if(i == 2 || i == 3)
                {
                    wall.Sort((a, b) => a.z.CompareTo(b.z));
                    int val = wall.Count / 2;
                    Vector3 vect = wall[val];
                    if (i == 2)
                    {
                        vect.x += 0.5f;
                    }
                    else
                    {
                        vect.z -= 0.5f;
                    }
                    Instantiate(light, vect, Quaternion.identity);
                }
            }
        }
    }

    // Lets try and calculate a path between two points and draw a red line !!! 
    public void calculatePathOfRoom()
    {
        foreach (int roomID in soundfxDictionary)
        {
            List<DoorObject> doors = lvl.getDoors();
            List<int> tileIDsDoors = new List<int>();

            foreach (DoorObject d in doors)
            {
                if (d.getRoomID1() == roomID)
                {
                    tileIDsDoors.Add(d.getTileID1());
                }
                if (d.getRoomID2() == roomID)
                {
                    tileIDsDoors.Add(d.getTileID2());
                }
            }

            // For now only check rooms with at least 2 doors
            if (tileIDsDoors.Count > 1)
            {
                for (int i = 0; i < tileIDsDoors.Count - 1; i++)
                {
                    for (int j = 1; j < tileIDsDoors.Count; j++)
                    {
                        float x1 = lvl.getTiles(tileIDsDoors[i]).transform.position.x;
                        float y1 = lvl.getTiles(tileIDsDoors[i]).transform.position.y;
                        float z1 = lvl.getTiles(tileIDsDoors[i]).transform.position.z;

                        Vector3 vect1 = new Vector3(x1, y1, z1);

                        float x2 = lvl.getTiles(tileIDsDoors[j]).transform.position.x;
                        float y2 = lvl.getTiles(tileIDsDoors[j]).transform.position.y;
                        float z2 = lvl.getTiles(tileIDsDoors[j]).transform.position.z;

                        Vector3 vect2 = new Vector3(x2, y2, z2);

                        //NavMesh.CalculatePath(vect1, vect2, NavMesh.AllAreas, path);

                        Seeker s = GetComponent<Seeker>();
                        Pathfinding.Path p = s.StartPath(vect1, vect2);
                        AstarPath.WaitForPath(p);

                        //for (int k = 0; k < path.corners.Length - 1; k++)
                        //    Debug.DrawLine(path.corners[k], path.corners[k + 1], Color.red, 60, false);

                        for (int k = 0; k < p.vectorPath.Count-1; k++)
                        {
                            Debug.DrawLine(p.vectorPath[k], p.vectorPath[k + 1], Color.red, 60, false);
                        }

                        if (!zingerDictionary.keyExists(roomID))
                        {
                            calculateSoundNodePos(p, roomID);
                        }
                    }
                }
            }
        }
    }

    private int soundNodecounter = 0;
    public void calculateSoundNodePos(Pathfinding.Path path, int roomID)
    {
        int point_num = 0;
        float xSum = 0f;
        float ySum = 0f;
        float zSum = 0f;

        for (int k = 0; k < path.vectorPath.Count; k++)
        {
            xSum += path.vectorPath[k].x;
            ySum += path.vectorPath[k].y;
            zSum += path.vectorPath[k].z;
            point_num++;
        }

        float xVal = xSum / point_num;
        float yVal = ySum / point_num;
        float zVal = zSum / point_num;

        Vector3 currPos = new Vector3(xVal, yVal, zVal);
        SphereCollider sC = soundTest_prefab.GetComponent<SphereCollider>();

        float shiftValue = sC.radius / 2f; // get just a third so the shift is not too drastic.

        Collider[] hitColliders = Physics.OverlapSphere(currPos, sC.radius);

        if (hitColliders.Length > 0)
        {
            foreach(Collider col in hitColliders)
            {
                if (col.name == "wall." + roomID)
                {
                    Vector3 val = col.bounds.center;
                    if(val.x > currPos.x)
                    {
                        xVal = xVal + shiftValue;
                    } else
                    {
                        xVal = xVal - shiftValue;
                    }

                    if(val.z > currPos.z)
                    {
                        zVal = zVal + shiftValue;
                    } else
                    {
                        zVal = zVal - shiftValue;
                    }

                    Debug.Log("Collided at " + col.name);
                }
            }
        }
        Transform t = Instantiate(soundTest_prefab, new Vector3(xVal, yVal, zVal), Quaternion.identity) as Transform;
        zingerDictionary.addZinger(roomID, Zinger.createZinger(roomID, t)); // Add the Zinger to the Dictionary -- Later we will fill them with clips
    }

    public void monsterInstantiation()
    {
        foreach(int k in monsterDictionary.Keys)
        {
            LevelBuilder.monsterWaypointDictionary.Add(k,initializeMonsterWaypoints(k));
            Transform g = Instantiate(monster_prefab, monsterDictionary[k], Quaternion.identity) as Transform;
            g.name = "Ghoul." + k;
            //placeMonsterWaypoints(k,g);
        }
        
    }

    public void placeMonsterWaypoints(int roomID, Transform monster)
    {
        // Place WayPoints for Monsters
        //AIEnemy ai = monster.GetComponent<AIEnemy>();
        AstarAI ai = monster.GetComponent<AstarAI>();
        //AIPathScript ai = monster.GetComponent<AIPathScript>();

        List<int> tileIDs = new List<int>(lvl.getRoomTileIDs(roomID));

        Transform[] waypoints = new Transform[totalMonsterPatrolWaypoints];
            
        for(int p = 0; p < totalMonsterPatrolWaypoints; p++)
        {
            if(tileIDs.Count > 0)
            {
                int selection = MyRandom.getRandom().random().Next(tileIDs.Count);
                GameObject point = new GameObject();
                point.transform.position = lvl.getTiles(tileIDs[selection]).transform.position;
                waypoints[p] = point.transform;
                tileIDs.RemoveAt(selection);
            }
        }
        ai.setTargetPositions(waypoints);
        //ai.setWaypoints(waypoints);
        
    }

    public void addWallPos(int roomID, GameObject wall, int direction)
    {
        if (wallPos.ContainsKey(roomID))
        {
            wallPos[roomID].Add(new Tuple<GameObject,int>(wall, direction));
        }
        else
        {
            wallPos.Add(roomID, new List<Tuple<GameObject, int>>());
            wallPos[roomID].Add(new Tuple<GameObject, int>(wall, direction));
        }
    }

    public WallCorner getCorner(int tileID, List<WallCorner> corners)
    {
        foreach (WallCorner w in corners)
        {
            if(w.getTileID() == tileID)
            {
                return w;
            }
        }
        return null;
    }

    // Bool that states if the wall is on the right = false or down = true to place a wall.
    public GameObject instantiateWall(int roomID, Transform t, Vector3 pos, Quaternion q)
    {
        Transform w1 = Instantiate(t, pos, q) as Transform;

        GameObject wall = w1.gameObject;
        wall.name = "wall." + roomID;
            
        this.setToUnWalkable(wall);

        addWallPos(roomID, wall, 0);

        return wall;
    }

    public GameObject instantiateCorner(int roomID, Transform t, Vector3 pos, float angle)
    {
        Transform w1 = Instantiate(t, pos, Quaternion.Euler(0f, angle, 0f)) as Transform;
        w1.position = pos;
        GameObject wall = w1.gameObject;
        wall.name = "corner." + roomID;

        this.setToUnWalkable(wall);
        addWallPos(roomID, wall, 3);

        return wall;
    }

    public void buildNavMesh()
    {
        AstarPath.active.astarData.recastGraph.SnapForceBoundsToScene();
        navMeshPath.Scan();
    }

    public Transform[] initializeMonsterWaypoints(int roomID)
    {
        List<int> tileIDs = new List<int>(lvl.getRoomTileIDs(roomID));
        Transform[] waypoints = new Transform[totalMonsterPatrolWaypoints];

        for (int p = 0; p < totalMonsterPatrolWaypoints; p++)
        {
            if (tileIDs.Count > 0)
            {
                int selection = MyRandom.getRandom().random().Next(tileIDs.Count);
                GameObject point = new GameObject();
                point.transform.position = lvl.getTiles(tileIDs[selection]).transform.position;
                waypoints[p] = point.transform;
                tileIDs.RemoveAt(selection);
            }
        }
        return waypoints;
    }

    public void placeObjective ()
    {
        Vector3 vect = calcRoomCentroids(lvl, objectiveRoomID);
        Vector3 height = new Vector3(0f, 3f, 0f);

        Instantiate(item_prefab, vect + height, Quaternion.identity);
    }

    public int getRoomIDByTileID(int tileID)
    {
        return (lvl.getRoomByTileID(tileID) + 1);
    }

    public void pauseGame()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            soundManager.muteAllSources();
            Time.timeScale = 0;
        }
        else
        {
            soundManager.unmuteAllSources();
            Time.timeScale = 1;
        }
    }

    public void reloadBuilderScene()
    {
        // This is where this scene is reloaded to load the next level.
        Debug.Log("Re-Loading Next Level");

        //logSystem.stopStreaming();

        Cursor.visible = true;

        if (!LogisticLoader.HASFINISHED())
        {
            // Keep the Empatica Object Ready for the next scene!
            GameObject.DontDestroyOnLoad(EmpaticaClientObject);

            //SceneManager.UnloadScene(SceneManager.GetActiveScene().name);

            if(maxLvlTime <= 0)
            {
                SceneManager.LoadScene("timelimitScene");
            }
            else
            {
                // Load Level switch here!
                SceneManager.LoadScene("levelswitch");
            }
            
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            if (maxLvlTime <= 0)
            {
                SceneManager.LoadScene("timelimitScene");
            }
            else
            {
                SceneManager.LoadScene("endScreen");
            }
        }


    }
}
