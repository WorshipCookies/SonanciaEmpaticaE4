using ProjectMaze.LogSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.GeneticInterfaces;
using System.IO;
using System.Diagnostics;
using ProjectMaze.TensionMapGeneration;

namespace ProjectMaze.LogSystem
{
    class TensionMapLogger : ILogger
    {

        private FolderManagement folderManager;
        private string filename;
        private string phenoLog = "tensionMapPheno";

        public TensionMapLogger(FolderManagement folderManager)
        {
            this.folderManager = folderManager;
        }

        public void changeFile(string filename)
        {
            this.filename = filename + ".csv";
        }

        public FolderManagement getFolderManager()
        {
            return folderManager;
        }

        public void savePhenoStructure(IPhenotype pheno, string filename)
        {
            if (!Directory.Exists(folderManager.getGenFolder() + "\\" + phenoLog))
            {
                System.IO.Directory.CreateDirectory(folderManager.getGenFolder() + "\\" + phenoLog);
            }

            TensionMapPhenotype p = (TensionMapPhenotype)pheno;
            using (StreamWriter writer = new StreamWriter(folderManager.getGenFolder() + "\\" + phenoLog + "\\" + filename + ".txt", true))
            {
                int counter = 0;
                foreach(Double d in p.getTensionMap())
                {
                    if(counter == 0)
                    {
                        writer.Write("[ " + d + " , ");
                    }
                    else if (counter == p.getMaxRooms() - 1)
                    {
                        writer.Write(d + " ]");
                    }
                    else
                    {
                        writer.Write(d + " , ");
                    }
                    counter++;
                }
            }
        }

        public void setFolderManager(FolderManagement folderManager)
        {
            throw new NotImplementedException();
        }

        public void takePicture(IPhenotype pheno, string filename)
        {
            throw new NotImplementedException();
        }

        public void writeLog(string log)
        {
            using (StreamWriter writer = new StreamWriter(folderManager.getGenFolder() + "\\" + filename, true))
            {
                writer.Write(log);
            }
        }

        public void writeLogTimeStamp(string log)
        {
            using (StreamWriter writer = new StreamWriter(folderManager.getGenFolder() + "\\" + filename, true))
            {
                writer.Write(Stopwatch.GetTimestamp() + " : " + log);
            }
        }

        public void writePythonBestAnxietyFitnessIndividual(string log)
        {
            throw new NotImplementedException();
        }

        public void writePythonBestFitnessIndividual(string log)
        {
            using (StreamWriter writer = new StreamWriter(folderManager.getExperimentFolder() + "\\PythonBestFitnessIndividualTensionMap.csv", true))
            {
                writer.Write(log);
            }
        }

        public void writePythonBestStructureFitnessIndividual(string log)
        {
            throw new NotImplementedException();
        }

        public void writePythonLog(string log)
        {
            using (StreamWriter writer = new StreamWriter(folderManager.getExperimentFolder() + "\\PythonLogTensionMap.csv", true))
            {
                writer.Write(log);
            }
        }

        public void writePythonBestTypeFitnessIndividual(string log, IFitness fitness)
        {
            String fitType = fitness.GetType().ToString();
            using (StreamWriter writer = new StreamWriter(folderManager.getExperimentFolder() + "\\PythonLogBestIndividualTension_" + fitType + ".csv", true))
            {
                writer.Write(log);
            }
        }

        public void writePythonMonsterFitnessLog(string log)
        {
            throw new NotImplementedException();
        }

        public void writePythonStructureFitnessLog(string log)
        {
            throw new NotImplementedException();
        }
    }
}
