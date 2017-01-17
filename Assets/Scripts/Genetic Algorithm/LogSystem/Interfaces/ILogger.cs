using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.LogSystem.Interfaces
{
    public interface ILogger
    {

        void writeLog(string log);

        void writeLogTimeStamp(string log);

        void writePythonLog(string log);

        void writePythonBestFitnessIndividual(string log);

        void writePythonBestStructureFitnessIndividual(string log);

        void writePythonBestAnxietyFitnessIndividual(string log);

        void writePythonStructureFitnessLog(string log);

        void writePythonMonsterFitnessLog(string log);

        void takePicture(IPhenotype pheno, string filename);

        void savePhenoStructure(IPhenotype pheno, string filename);

        void setFolderManager(FolderManagement folderManager);

        FolderManagement getFolderManager();

        void changeFile(string filename);

    }
}
