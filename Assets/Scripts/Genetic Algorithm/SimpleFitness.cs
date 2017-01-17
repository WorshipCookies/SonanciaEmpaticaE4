using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.GeneticInterfaces;
using ProjectMaze.Visual;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.ShortestPath;
using QuickGraph.Algorithms.Search;
using QuickGraph.Concepts;
using ProjectMaze.GeneticAlgorithm.GraphRepresentation;
using QuickGraph.Collections;
using QuickGraph.Concepts.Traversals;
using ProjectMaze.Util;
using ProjectMaze.LogSystem.Interfaces;
using ProjectMaze.Visual.Spawn;
using System.Diagnostics;

namespace ProjectMaze.GeneticAlgorithm
{
    public class SimpleFitness : IFitness
    {

        private List<List<IEdge>> paths;

        private static int REWARD_VALUE = 10;
        private static int PENALTY_VALUE = 10;
        private static int TOTAL_ROOMS = LevelBuilder.tension_map.Length + 5; // TOTAL ROOMS

        private static string name_ID = "Path Finding Fitness";
        private ILogger logger;

        private AnxietySuspenseFitness monsterFit;


        private double multipath_fit;
        private double monster_fit;

        public SimpleFitness()
        {
            List<List<IEdge>> paths = new List<List<IEdge>>();
            monsterFit = new AnxietySuspenseFitness();
            this.logger = null;
        }

        public double evaluate(IPhenotype pheno)
        {
            try
            {
                Phenotype phen = (Phenotype)pheno;

                //Console.WriteLine(phen.getGraph().printConnections());
                
                // This Calculates the Path between each room in the map.
                //this.getPathFindingForEachRoom(phen);
                //return this.getPathFinding(phen);

                this.multipath_fit = this.multiPathFind(phen);
                this.monster_fit = monsterFit.evaluate(phen);

                if (Double.IsNaN(multipath_fit))
                {
                    Console.WriteLine(""); // Bug in Multipath Fitness
                }

                if (Double.IsNaN(monster_fit))
                {
                    Console.WriteLine(""); // Bug in Monster Fitness
                }

                phen.setIndividualFitnesses(multipath_fit, monster_fit);

                double totalValue = multipath_fit + monster_fit;
                return totalValue;

                // Wow! Thats some complicated fitness function! *Sarcasm* --- Replace this afterwards... (for testing purposes only!)
                //return 0.0;
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
            monsterFit.setLogger(log);
        }

        public void getPathFindingForEachRoom(Phenotype phen)
        {
            // Initialize the algorithms
            PredecessorRecorder p = new PredecessorRecorder(phen.getGraph());
            BreadthFirstSearchAlgorithm bfs = new BreadthFirstSearchAlgorithm(phen.getGraph());


            // Add the Event Handlers --- this sucks by the way...
            bfs.DiscoverVertex += new VertexHandler(p.findVertex);
            bfs.TreeEdge += new EdgeHandler(p.RecordPredecessor);

            // Create a List with one random tile from each room
            List<int> room_tile_ids = new List<int>();

            // The List of all Paths
            paths = new List<List<IEdge>>();
            foreach (Room r in phen.getMap().getRooms())
            {
                room_tile_ids.Add(r.getTiles()[MyRandom.getRandom().random().Next(r.getTiles().Count - 1)].getID());
            }

            List<int> path_visited = new List<int>();
            foreach (int id_start in room_tile_ids)
            {

                p.setStartDestination(id_start, room_tile_ids, path_visited);
                bfs.Compute(phen.getGraph().getVertex(id_start).getIVertex());

                Console.WriteLine("----------------- FINISH COMPUTING --------------------");


                path_visited.Add(id_start);
            }

            foreach (List<IEdge> i in p.getPath())
            {
                paths.Add(i);
            }

            // Compute on the starting IVertex!
            Console.WriteLine(" --------------------- FITNESS CALCULATION START ! ---------------------");
            int path_num = 0;
            foreach (List<IEdge> path in paths)
            {
                Console.WriteLine("Path " + path_num);
                foreach (IEdge edge in path)
                {
                    Console.WriteLine(phen.getGraph().getEdge(edge).getVertexSource().getID() + " is Connected to " + phen.getGraph().getEdge(edge).getVertexTarget().getID());
                }
                path_num++;
            }
        }

        public double getPathFinding(Phenotype phen)
        {
            // Initialize the algorithms
            PredecessorRecorder p = new PredecessorRecorder(phen.getGraph());
            BreadthFirstSearchAlgorithm bfs = new BreadthFirstSearchAlgorithm(phen.getGraph());


            // Add the Event Handlers --- this sucks by the way...
            bfs.DiscoverVertex += new VertexHandler(p.findVertex);
            bfs.TreeEdge += new EdgeHandler(p.RecordPredecessor);

            // Create a List with one random tile from each room
            List<int> room_tile_ids = phen.getMap().getAllRoomIDs();
            
            List<int[]> start_destination_tuple = new List<int[]>();
            List<Tile> t1 = phen.getMap().getRoomByID(room_tile_ids.Min()).getTiles();
            List<Tile> t2 = phen.getMap().getRoomByID(room_tile_ids.Max()).getTiles();
            int[] tup = { t1[MyRandom.getRandom().random().Next(t1.Count - 1)].getID(), t2[MyRandom.getRandom().random().Next(t2.Count - 1)].getID() };
            start_destination_tuple.Add(tup);
            
            //for (int i = 0; i < room_tile_ids.Count - 1; i++)
            //{
            //    List<Tile> t1 = phen.getMap().getRoomByID(room_tile_ids[i]).getTiles();
            //    List<Tile> t2 = phen.getMap().getRoomByID(room_tile_ids[i+1]).getTiles();
            //    int[] tuple = { t1[MyRandom.getRandom().random().Next(t1.Count-1)].getID(), t2[MyRandom.getRandom().random().Next(t2.Count-1)].getID() };
            //    start_destination_tuple.Add(tuple);
            //}

            foreach(int[] tuple in start_destination_tuple)
            {

                p.setStartDestination(tuple[0], tuple[1]);
                bfs.Compute(phen.getGraph().getVertex(tuple[0]).getIVertex());

                //Console.WriteLine("----------------- FINISH COMPUTING --------------------");

            }

            paths = new List<List<IEdge>>();
            List<IEdge> complete_path = new List<IEdge>();

            // For Fitness Calculation my gentleman friend!
            double fit_value = 0.0;
            List<int> fitnessAvailableRooms = phen.getMap().getAllRoomIDs();
            List<int> fitnessVisitedRooms = new List<int>();
            foreach (List<IEdge> i in p.getPath())
            {
                foreach (IEdge edge in i)
                {
                    // Specific for Path Finding
                    complete_path.Add(edge);

                    // Specific for Fitness -- Remove if fitness does not make sense
                    Edge iedge = phen.getGraph().getEdge(edge);
                    
                    // Special cases its important to know the Source Vertex as well!
                    int room_id_source = phen.getMap().getTileByID(iedge.getVertexSource().getID()).getRoom().getID();
                    if (!fitnessVisitedRooms.Contains(room_id_source))
                    {
                        fitnessVisitedRooms.Add(room_id_source);
                        fit_value += REWARD_VALUE;
                    }

                    // Now analyse the target Vertex
                    int room_id = phen.getMap().getTileByID(iedge.getVertexTarget().getID()).getRoom().getID();
                    if (!fitnessVisitedRooms.Contains(room_id))
                    {
                        fitnessVisitedRooms.Add(room_id);
                        fit_value += REWARD_VALUE;
                    }
                }
            }

            // PENALTY CALCULATION - Rooms not Visited, non exploration penalty
            if ((fitnessAvailableRooms.Count - fitnessVisitedRooms.Count) > 0)
            {
                foreach (int id in fitnessAvailableRooms)
                {
                    if (!fitnessVisitedRooms.Contains(id))
                    {
                        fit_value -= PENALTY_VALUE;
                    }
                }
            }

            // PENALTY CALCULATION - If num Rooms is greater than the TOTAL ROOM VALUE Remove it from population
            if (fitnessAvailableRooms.Count > TOTAL_ROOMS)
            {
                fit_value = 0;
            }

            //Console.WriteLine(" --------------------- FITNESS CALCULATION START ! ---------------------");

            // If the Fitness value is negative, just give a fitness value of zero.
            if (fit_value < 0)
            {
                fit_value = 0;
            }

            paths.Add(complete_path);
            phen.setPath(paths);

            // Compute on the starting IVertex!
            if (logger != null)
            {
                logger.writeLogTimeStamp(" ------- " + name_ID + " ------- \n");
                int path_num = 0;
                foreach (List<IEdge> path in paths)
                {
                    //Console.WriteLine("Path " + path_num);
                    foreach (IEdge edge in path)
                    {
                        //Console.WriteLine(phen.getGraph().getEdge(edge).getVertexSource().getID() + " is Connected to " + phen.getGraph().getEdge(edge).getVertexTarget().getID());
                        logger.writeLog(phen.getGraph().getEdge(edge).getVertexSource().getID() + " is Connected to " + phen.getGraph().getEdge(edge).getVertexTarget().getID() + "\n");
                    }
                    path_num++;
                }
                logger.writeLog("Fitness Value: " + fit_value + "\n");
            }

            // Normalization of fitness value
            fit_value = fit_value / (TOTAL_ROOMS * 10);

            return fit_value;
        }

        public double multiPathFind(Phenotype phen)
        {
            // Initialize the algorithms
            PredecessorRecorder p = new PredecessorRecorder(phen.getGraph());
            BreadthFirstSearchAlgorithm bfs = new BreadthFirstSearchAlgorithm(phen.getGraph());


            // Add the Event Handlers --- this sucks by the way...
            bfs.DiscoverVertex += new VertexHandler(p.findVertex);
            bfs.TreeEdge += new EdgeHandler(p.RecordPredecessor);

            // Create a List with one random tile from each room
            List<int> room_tile_ids = phen.getMap().getAllRoomIDs();

            List<int[]> start_destination_tuple = new List<int[]>();
            List<Tile> t1 = phen.getMap().getRoomByID(room_tile_ids.Min()).getTiles(); // Start Node

            foreach (SpawnPoint sp in phen.getMap().getSpawnPoints())
            {
                if (sp.getType() == 0)
                {
                    List<Tile> t2 = phen.getMap().getRoomByID(sp.getRoom()).getTiles();
                    int[] tup = { t1[MyRandom.getRandom().random().Next(t1.Count - 1)].getID(), t2[MyRandom.getRandom().random().Next(t2.Count - 1)].getID() };
                    start_destination_tuple.Add(tup);
                }
            }

            paths = new List<List<IEdge>>();
            foreach (int[] tuple in start_destination_tuple)
            {
                p.setStartDestination(tuple[0], tuple[1]);

                bfs.Compute(phen.getGraph().getVertex(tuple[0]).getIVertex());
            }

            paths = new List<List<IEdge>>();
            List<IEdge> complete_path = new List<IEdge>();

            // For Fitness Calculation my gentleman friend!
            double fit_value = 0.0;
            List<int> fitnessAvailableRooms = phen.getMap().getAllRoomIDs();
            List<int> fitnessVisitedRooms = new List<int>();

            List<List<int>> roomIdPath = new List<List<int>>();

            foreach (List<IEdge> i in p.getPath())
            {
                List<int> idPath = new List<int>();
                foreach (IEdge edge in i)
                {
                    // Specific for Path Finding
                    complete_path.Add(edge);

                    // Specific for Fitness -- Remove if fitness does not make sense
                    Edge iedge = phen.getGraph().getEdge(edge);

                    // Special cases its important to know the Source Vertex as well!
                    int room_id_source = phen.getMap().getTileByID(iedge.getVertexSource().getID()).getRoom().getID();
                    if (!fitnessVisitedRooms.Contains(room_id_source))
                    {
                        fitnessVisitedRooms.Add(room_id_source);
                        fit_value += REWARD_VALUE;
                    }

                    // Now analyse the target Vertex
                    int room_id = phen.getMap().getTileByID(iedge.getVertexTarget().getID()).getRoom().getID();
                    if (!fitnessVisitedRooms.Contains(room_id))
                    {
                        fitnessVisitedRooms.Add(room_id);
                        fit_value += REWARD_VALUE;
                    }


                    // For ROOM ID Path
                    if (!idPath.Contains(room_id_source))
                    {
                        idPath.Add(room_id_source);
                    }
                    if (!idPath.Contains(room_id))
                    {
                        idPath.Add(room_id);
                    }
                }
                roomIdPath.Add(idPath);
            }
            phen.setRoomIDPath(roomIdPath);

            // PENALTY CALCULATION - Rooms not Visited, non exploration penalty
            if ((fitnessAvailableRooms.Count - fitnessVisitedRooms.Count) > 0)
            {
                foreach (int id in fitnessAvailableRooms)
                {
                    if (!fitnessVisitedRooms.Contains(id))
                    {
                        fit_value -= PENALTY_VALUE;
                    }
                }
            }

            // PENALTY CALCULATION - If num Rooms is greater than the TOTAL ROOM VALUE Remove it from population
            if (fitnessAvailableRooms.Count > TOTAL_ROOMS)
            {
                fit_value = 0;
            }
            else
            {
                // Just testing a small weight for the total number of rooms created
                //double numRoomReward = (fitnessAvailableRooms.Count * 10);

                //fit_value += numRoomReward;
            }

            //Console.WriteLine(" --------------------- FITNESS CALCULATION START ! ---------------------");

            // If the Fitness value is negative, just give a fitness value of zero.
            if (fit_value < 0)
            {
                fit_value = 0;
            }

            paths.Add(complete_path);
            phen.setPath(paths);

            // Compute on the starting IVertex! -- Logging Path Calculation
            if (logger != null)
            {
                logger.writeLogTimeStamp(" ------- " + name_ID + " ------- \n");
                int path_num = 0;
                foreach (List<IEdge> path in paths)
                {
                    //Console.WriteLine("Path " + path_num);
                    foreach (IEdge edge in path)
                    {
                        //Console.WriteLine(phen.getGraph().getEdge(edge).getVertexSource().getID() + " is Connected to " + phen.getGraph().getEdge(edge).getVertexTarget().getID());
                        logger.writeLog(phen.getGraph().getEdge(edge).getVertexSource().getID() + " is Connected to " + phen.getGraph().getEdge(edge).getVertexTarget().getID() + "\n");
                    }
                    path_num++;
                }
            }

            // Normalization of fitness value
            fit_value = fit_value / (TOTAL_ROOMS*10);

            if (logger != null)
            {
                logger.writeLog("Structure Fitness Value: " + fit_value + "\n"); // Logging Fitness Calculation
            }

            return fit_value;
        }

        public double getMultiPathFitness()
        {
            return multipath_fit;
        }

        public double getMonsterFitness()
        {
            return monster_fit;
        }
    }
}
