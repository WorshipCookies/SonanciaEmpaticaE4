using ProjectMaze.GeneticInterfaces;
using ProjectMaze.LogSystem.Interfaces;
using ProjectMaze.Util;
using ProjectMaze.Visual.Spawn;
using QuickGraph.Concepts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ProjectMaze.GeneticAlgorithm
{
    public class AnxietySuspenseFitness : IFitness
    {

        private static string name_ID = "Anxiety Suspense Fitness";
        private ILogger logger;

        private static double ANXIETY_INCREASE = 1.0;
        private static double ANXIETY_DECAY = 0.5;
        private static double MAX_ANXIETY = 2.0;
        private static double ANXIETY_THRESHOLD = 1.0; // If values decrease and go up beyond this threshold no reward or penalties are given. To get rewards Anxiety must always be kept below this level!

        private static double PENALTY_VALUE = 1;
        private static double REWARD_VALUE = 1;

        public AnxietySuspenseFitness()
        {
            this.logger = null;
        }

        public double evaluate(IPhenotype pheno)
        {
            try
            {
                Phenotype phen = (Phenotype)pheno;

                // NORMALIZATION!!!
                double fit_value = AnxietySuspenseEvaluation(phen);
                if (fit_value > 0)
                {
                    // Do Normalization
                    fit_value = fit_value / (LevelBuilder.tension_map.Length * 2);//(phen.getMap().getAllRoomIDs().Count * REWARD_VALUE); // Normalize number of rewards to the total number of rewards possible in a map
                }

                if (logger != null)
                {
                    logger.writeLog("Anxiety Fitness Value: " + fit_value + "\n"); // Logging Fitness Calculation
                }

                return fit_value; // This is how we roll!
            }
            catch (Exception e)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(e, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();

                Console.WriteLine(e.ToString());

                throw e;
            }
            
        }

        public void setLogger(LogSystem.Interfaces.ILogger log)
        {
            this.logger = log;
        }

        public double AnxietySuspenseEvaluation(Phenotype phen)
        {

            // First lets get the longest path (thats our main quest!)
            List<int> item_path = new List<int>();
            foreach (List<int> path in phen.getRoomIDPaths())
            {
                if (path.Count > item_path.Count)
                {
                    item_path = path;
                }
            }

            if (item_path.Count > 0)
            {
                foreach (SpawnPoint sp in phen.getMap().getItemOfRoomID(item_path[item_path.Count - 1]))
                {
                    if (sp.getType() == 0)
                    {
                        phen.setMainSpawnItem(sp);
                        break;
                    }
                }
            }

            if (logger != null)
            {
                logger.writeLogTimeStamp(" ------- " + name_ID + " ------- \n");

                if (phen.getMainItem() == null)
                {
                    logger.writeLog("No main item path");
                }
                else
                {
                    logger.writeLog("Path to Item in Room " + phen.getMainItem().getRoom() + ": ");

                    foreach (int i in item_path)
                    {
                        logger.writeLog(i + ", ");
                    }
                }
                logger.writeLog("\n");
            }

            // We got the path! Now lets create an anxiety map.
            List<MyTuple> anxiety_map = new List<MyTuple>();  // Index = Sequence of Room IDs as the expected player path. Value = Anxiety value evaluation.
            
            // Create Anxiety Mapping
            for (int i = 0; i < item_path.Count; i++)
            {
                int roomID = item_path[i]; 

                if (!containsRoom(anxiety_map, roomID))
                {

                    // NEW REVISION CODE 20_04_2016 --- INSTEAD OF USING INT USE THE ELMENT OBJECT.

                    // Sum the values of all the spawn point within the level.
                    anxiety_map.Add(new MyTuple(roomID, getTensionValueOfRoom(roomID, phen)));

                    //// If there is a monster in the room 
                    //if (checkForMonster(roomID, phen))
                    //{
                    //    anxiety_map.Add(new MyTuple(roomID, 1.0));
                    //}
                    //else
                    //{
                    //    anxiety_map.Add(new MyTuple(roomID, 0.0));
                    //}
                }
            }

            if (logger != null)
            {
                logger.writeLog("First Pass Anxiety Map : ");

                foreach (MyTuple t in anxiety_map)
                {
                    logger.writeLog("( " + t.getID() + ", " + t.getValue() + " ); ");
                }
                logger.writeLog("\n");
            }
            

            // Build the tension map
            refineAnxietyMap(anxiety_map);

            if (logger != null)
            {
                logger.writeLog("Second Pass Anxiety Map : ");

                foreach (MyTuple t in anxiety_map)
                {
                    logger.writeLog("( " + t.getID() + ", " + t.getValue() + " ); ");
                }
                logger.writeLog("\n");
            }

            phen.setAnxietyMap(anxiety_map); // Set the Anxiety Map!

            double fit_value = evaluateAnxietyMap(anxiety_map);

            // return an evaluation!
            return fit_value;
        }

        public static bool checkForMonster(int roomID, Phenotype phen)
        {
            foreach(SpawnPoint sp in phen.getMap().getSpawnPoints())
            {
                if (sp.getType() == 1 && sp.getRoom() == roomID)
                {
                    return true;
                }
            }
            return false;
        }

        public static double getTensionValueOfRoom(int roomID, Phenotype phen)
        {
            double current_tension = 0.0;
            foreach (SpawnPoint sp in phen.getMap().getSpawnPoints())
            {
                if (sp.getRoom() == roomID)
                {
                    if(sp.getElement() != null) // SUB OBJECT RETURNS ELEMENT NULL -- BE CAREFUL!
                    {
                        current_tension += sp.getElement().tensionValue();
                    }
                }
            }
            return current_tension;
        }

        public static void refineAnxietyMap(List<MyTuple> map)
        {
            if (map.Count > 0)
            {
                map[0].setValue(Math.Max(map[0].getValue(), 0));
            }
            for (int i = 1; i < map.Count; i++)
            {
                //if (map[i].getValue() == 0.0)
                //{
                //    if (map[i - 1].getValue() > 0) // Make sure that decay never goes below 0
                //    {
                //        map[i].setValue( map[i - 1].getValue() - ANXIETY_DECAY );
                //    }
                //}
                //else
                //{
                //    map[i].setValue( map[i - 1].getValue() + map[i].getValue() );
                //}

                double sum_val = map[i - 1].getValue() + map[i].getValue();

                // If the value of the previous room is higher or equal then there is a decay
                if (map[i-1].getValue() >= sum_val)
                {
                    map[i].setValue(map[i-1].getValue() + (map[i].getValue() - ANXIETY_DECAY) );
                }
                // If it keeps growing then there is no decay!
                else
                {
                    map[i].setValue(map[i - 1].getValue() + map[i].getValue());
                }

                // Safe guard that no value goes below 0
                map[i].setValue( Math.Max(map[i].getValue(), 0) ); 
            }
        }

        public double evaluateAnxietyMap(List<MyTuple> map)
        {
            double fit_value = 0.0;

            //fit_value += evaluateAnxietyPeaks(map);
            //fit_value += evaluateMaxThreshold(map);
            //fit_value += evaluateConstantLowAnxiety(map);

            fit_value += evaluateTensionMap(map);

            // Lets normalize the value
            if (fit_value <= 0)
            {
                fit_value = 0;
            }

            return fit_value;
        }

        // New Version of the Anxiety Map Evaluation
        public static double evaluateTensionMap(List<MyTuple> local_map)
        {
            // Obtain samples of the real tension map.
            List<MyTuple> real_values = Tension_Maps.translateMap(local_map.Count, LevelBuilder.tension_map);

            if (real_values == null)
            {
                return 0.0;
            }

            double evaluation = 0.0;

            for (int i = 0; i < real_values.Count; i++)
            {              
                evaluation += 2 - Math.Abs((real_values[i].getValue() - local_map[i].getValue()));
            }

            return evaluation;
        }

        public static double evaluateMaxThreshold(List<MyTuple> map)
        {
            double fit_value = 0.0;
            // Give the F-U to all values who go above the MAXIMUM_THRESHOLD!!
            for (int i = 0; i < map.Count; i++)
            {
                if (map[i].getValue() > MAX_ANXIETY)
                {
                    fit_value -= PENALTY_VALUE;
                }
            }

            return fit_value;
        }

        public static double evaluateConstantLowAnxiety(List<MyTuple> map)
        {
            double fit_value = 0.0;

            for (int i = 0; i < map.Count - 1; i++)
            {
                if (map[i].getValue() == 0 && map[i + 1].getValue() == 0)
                {
                    fit_value -= PENALTY_VALUE;
                }
            }

            return fit_value;
        }

        public static double evaluateAnxietyPeaks(List<MyTuple> map)
        {
            double fit_value = 0.0;
            // Evaluate a Peak between 3 Points
            for (int i = 1; i < map.Count - 1; i++)
            {
                double a = map[i - 1].getValue(); // Left
                double b = map[i].getValue(); // Mid
                double c = map[i + 1].getValue(); // Right

                if (b > a && b > c)
                {
                    fit_value += REWARD_VALUE;
                }
            }

            return fit_value;

        }

        public static bool containsRoom(List<MyTuple> map, int roomID)
        {
            foreach (MyTuple t in map)
            {
                if (t.getID() == roomID)
                {
                    return true;
                }
            }
            return false;
        }

        // This Method allows for the creation of tension maps for other paths!
        public static List<MyTuple> createTensionMap(List<int> my_path, Phenotype phen)
        {
            List<MyTuple> anxiety_map = new List<MyTuple>();
            // Create Anxiety Mapping
            for (int i = 0; i < my_path.Count; i++)
            {
                int roomID = my_path[i];

                if (!containsRoom(anxiety_map, roomID))
                {
                    // If there is a monster in the room 
                    if (checkForMonster(roomID, phen))
                    {
                        anxiety_map.Add(new MyTuple(roomID, 1.0));
                    }
                    else
                    {
                        anxiety_map.Add(new MyTuple(roomID, 0.0));
                    }
                }
            }

            // Build the tension map
            refineAnxietyMap(anxiety_map);

            return anxiety_map;
        }


    }
}
