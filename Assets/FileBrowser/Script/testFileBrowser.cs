using UnityEngine;
using System.Collections;

public class testFileBrowser : MonoBehaviour {
	//skins and textures
	public GUISkin[] skins;
	public Texture2D file,folder,back,drive;
	
	string[] layoutTypes = {"Type 0","Type 1"};
	//initialize file browser
	FileBrowser fb = new FileBrowser();
	string output = "no file";
    // Use this for initialization


    private GameObject loadPanel;
    private GameObject filePanel;
    private ExperimentFileLoader infoLoad;

    void Awake()
    {
        loadPanel = GameObject.Find("LoadPanel");
        infoLoad = GameObject.Find("Main Camera").GetComponent<ExperimentFileLoader>();
        filePanel = gameObject;
    }

	void Start () {

		//setup file browser style
		//fb.guiSkin = skins[0]; //set the starting skin
		//set the various textures
		fb.fileTexture = file; 
		fb.directoryTexture = folder;
		fb.backTexture = back;
		fb.driveTexture = drive;
		//show the search bar
		fb.showSearch = true;
		//search recursively (setting recursive search may cause a long delay)
		fb.searchRecursively = true;
	}
	
	void OnGUI(){
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		//GUILayout.Label("Layout Type");
		//fb.setLayout(GUILayout.SelectionGrid(fb.layoutType,layoutTypes,1));
		GUILayout.Space(10);
		//select from available gui skins
		//GUILayout.Label("GUISkin");
		//foreach(GUISkin s in skins){
		//	if(GUILayout.Button(s.name)){
		//		fb.guiSkin = s;
		//	}
		//}
		//GUILayout.Space(10);
		//fb.showSearch = GUILayout.Toggle(fb.showSearch,"Show Search Bar");
		//fb.searchRecursively = GUILayout.Toggle(fb.searchRecursively,"Search Sub Folders");
		GUILayout.EndVertical();
		GUILayout.Space(10);
		GUILayout.Label("Selected File: "+output);
		GUILayout.EndHorizontal();
		//draw and display output
		if(fb.draw()){ //true is returned when a file has been selected
            //the output file is a member if the FileInfo class, if cancel was selected the value is null
            if (fb.outputFile == null)
            {
                closeFileLoader();
            }
            else
            {
                
                output = fb.outputFile.ToString();
                
                //output = fb.currentDirectory.ToString() + "\\" + output;
                

                readExperiment(output);
            }
			//output = (fb.outputFile==null)?"cancel hit":fb.outputFile.ToString();
		}
	}

    void closeFileLoader()
    {
        loadPanel.SetActive(true);
        filePanel.SetActive(false);
    }

    void readExperiment(string path)
    {
        System.IO.StreamReader file = new System.IO.StreamReader(path);
        string[] expInfo = file.ReadLine().Split(',');

        infoLoad.expID = expInfo[0];

        // Add More lvls than just 2
        infoLoad.lvlPaths = new System.Collections.Generic.List<string>();
        for(int i = 0; i < expInfo.Length; i++)
        {
            if(i != 0)
            {
                infoLoad.lvlPaths.Add(expInfo[i]);
            }
        }

        //infoLoad.path_lvl1 = expInfo[1];
        //infoLoad.path_lvl2 = expInfo[2];

        infoLoad.isFileLoaded();
        closeFileLoader();
    }
}
