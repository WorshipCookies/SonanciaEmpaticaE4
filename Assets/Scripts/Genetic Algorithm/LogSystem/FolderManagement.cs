using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectMaze.LogSystem
{
    public class FolderManagement
    {
        // This is main Folder where all Logs end up.
        public static string mainLogFolder = Path.GetDirectoryName(UnityEngine.Application.dataPath) + "//Logs";

        public static void mainLogFolderCheck()
        {
            if (!Directory.Exists(mainLogFolder))
            {
                System.IO.Directory.CreateDirectory(mainLogFolder);
            }
        }

        // This is the Experiment Folder (Where are the runs are kept)
        public string expFolder;

        // This is the Run Folder (Where the values of a single run of an experiment is kept)
        public string runFolder;

        // This is the Generation Folder (Where the values of a specific generation of a given run is kept)
        public string genFolder;

        public FolderManagement(string expFolder)
        {
            this.expFolder = mainLogFolder + "\\" + expFolder;
            
            if (!Directory.Exists(this.expFolder))
            {
                System.IO.Directory.CreateDirectory(this.expFolder);
            }
            this.runFolder = "";
            this.genFolder = "";
        }

        public void setNewExperimentFolder(string expFolder)
        {
            this.expFolder = mainLogFolder + "\\" + expFolder;

            if (!Directory.Exists(this.expFolder))
            {
                System.IO.Directory.CreateDirectory(this.expFolder);
            }
            this.runFolder = "";
            this.genFolder = "";
        }

        public void setNewRunFolder(string runFolder)
        {
            this.runFolder = expFolder + "\\" + runFolder;

            if (!Directory.Exists(this.runFolder))
            {
                System.IO.Directory.CreateDirectory(this.runFolder);
            }

            this.genFolder = "";
        }

        public void setNewGenerationFolder(string genFolder)
        {
            this.genFolder = this.runFolder + "\\" + genFolder;

            if (!Directory.Exists(this.genFolder))
            {
                System.IO.Directory.CreateDirectory(this.genFolder);
            }
        }

        public string getExperimentFolder()
        {
            return expFolder;
        }

        public string getRunFolder()
        {
            return runFolder;
        }

        public string getGenFolder()
        {
            return genFolder;
        }
    }
}
