using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.Visual;
using ProjectMaze.Util;
using QuickGraph.Representations;
using QuickGraph.Concepts;
using QuickGraph.Providers;
using ProjectMaze.GeneticAlgorithm.GraphRepresentation;
using ProjectMaze.LogSystem.Interfaces;
using ProjectMaze.Visual.Spawn;

namespace ProjectMaze.GeneticAlgorithm
{
    public class Phenotype : IPhenotype
    {

        private Map map;
        private double fitness_value;
        private AdjGraph graph;
        private List<List<IEdge>> paths;
        private List<List<int>> roomIDPath;

        private ILogger logger;

        private double multipath_fit;
        private double monster_fit;

        private SpawnPoint mainItem = null;
        private List<MyTuple> anxiety_map;

        public Phenotype(int height, int width, bool is_static)
        {
            if (is_static)
            {
                this.map = Map.mapFactoryStatic(height, width);
            }
            else
            {
                this.map = Map.mapFactory(height, width);
            }

            // Create the Graph Function -- I Like to call it the Skeleton of the Map
            createGraph();

            this.logger = null;
            this.fitness_value = 0.0;
            this.multipath_fit = 0.0;
            this.monster_fit = 0.0;
            paths = new List<List<IEdge>>();
            anxiety_map = new List<MyTuple>();
        }

        public Phenotype(Map map)
        {
            this.map = map;

            createGraph();

            this.logger = null;
            this.fitness_value = 0.0;
            this.multipath_fit = 0.0;
            this.monster_fit = 0.0;
            paths = new List<List<IEdge>>();
            anxiety_map = new List<MyTuple>();
        }

        // For use ONLY FOR CLONING!!
        private Phenotype(Phenotype to_clone)
        {
            this.map = to_clone.map.cloneMap();
            this.fitness_value = to_clone.fitness_value;
            this.monster_fit = to_clone.monster_fit;
            this.multipath_fit = to_clone.multipath_fit;
            this.anxiety_map = to_clone.anxiety_map;

            // Create Graph based on Phenotype
            createGraph();

            this.paths = new List<List<IEdge>>(to_clone.paths);
            this.logger = to_clone.logger;
        }

        public void setLogger(LogSystem.Interfaces.ILogger log)
        {
            this.logger = log;
        }

        // Transform a genotype into a Phenotype
        public void toPhenotype(Genotype g)
        {
            // First reset map structure
            map.resetValues();
            int counter = 0;
            for (int i = 0; i < g.getGeno().GetLength(0); i++)
            {
                for (int j = 0; j < g.getGeno().GetLength(1); j++)
                {
                    if (map.getRoomByID(g.getGeno()[i, j]) == null)
                    {
                        map.createRoom(g.getGeno()[i, j]);
                    }
                    Room r = map.getRoomByID(g.getGeno()[i, j]);
                    Tile t = map.getTileByID(counter);

                    t.setRoom(r);
                    r.addTile(t);

                    counter++;
                }
            }

            // Repair Function -- Holy shit brace yourself for take off!
            this.repairPhenotype();
            
            //Holy shit update the genotype!
            //g.updateTilesOnly(this);
            
            counter = 0;
            // Lets add the doors to the tiles
            foreach (int[] i in g.getDoors())
            {
                // Get adjacent tiles between 2 rooms
                if (map.getRoomByID(i[0]) != null && map.getRoomByID(i[1]) != null)
                {   
                    List<int[]> list = map.getAdjacentTileFromRoomToRoom(map.getRoomByID(i[0]), map.getRoomByID(i[1]));
                    if (list.Count > 0)
                    {
                        

                        // For Random Door Placement
                        //int[] chosen = list[MyRandom.getRandom().random().Next(list.Count)];

                        // For Deterministic Door Place (Places doors always in the "middle" of a wall
                        int index = list.Count / 2;
                        int[] chosen = list[index];

                        Tile[] door_chosen = { map.getTileByID(chosen[0]), map.getTileByID(chosen[1]) };

                        map.getDoors().Add(new Door(counter, door_chosen));

                        counter++;
                    }
                }
                else
                {
                    //Console.WriteLine("Door Bug!?!!");
                }
            }

            // Lets add the spawn points -- this might be wrong! :(
            foreach (int[] i in g.getSpawnPoints())
            {
                SpawnPoint sp = SpawnPoint.SpawnPointFactory(i[0],i[1]);
                int tiles_count = map.getRoomByID(sp.getRoom()).getTiles().Count;
                Tile t = map.getRoomByID(sp.getRoom()).getTiles()[(int)tiles_count / 2];
                sp.setTile(t);
                map.getSpawnPoints().Add(sp);
            }

            // Create the Graph
            createGraph();

            // BOOM SHAKALAKA! GOT MYSELF A PHENOTYPE!
        }

        public void setFitness(double fitness)
        {
            this.fitness_value = fitness;
        }

        public double getFitness()
        {
            return fitness_value;
        }

        public Map getMap()
        {
            return this.map;
        }

        public void createGraph()
        {
            graph = new AdjGraph(); // New Graph

            // Initialize the Graph
            foreach (Tile t in map.getTiles())
            {
                graph.addVertex(t.getID());
            }

            // Connect Vertexes that are in the same room
            foreach(Tile t in map.getTiles())
            {
                Tile[] neighbours = map.obtainNeighbours(t);
                foreach (Tile neighbour in neighbours)
                {
                    // If Neighbour is the same Room ID then connect them with a normal connection.
                    if (neighbour != null && neighbour.getRoom().getID() == t.getRoom().getID())
                    {
                        graph.addEdge(graph.getVertex(t.getID()), graph.getVertex(neighbour.getID()), 0);
                    }
                }
            }

            // Connect Rooms via doors
            foreach (Door d in map.getDoors())
            {
                Tile[] tiles = d.getConnectingTiles();

                // Connect Doors to each other!
                graph.addEdge(graph.getVertex(tiles[0].getID()), graph.getVertex(tiles[1].getID()), 1);
                graph.addEdge(graph.getVertex(tiles[1].getID()), graph.getVertex(tiles[0].getID()), 1);
            }
        }

        public AdjGraph getGraph()
        {
            return this.graph;
        }

        public void setPath(List<List<IEdge>> paths)
        {
            this.paths = new List<List<IEdge>>(paths);
        }

        public List<List<IEdge>> getPaths()
        {
            return this.paths;
        }

        public void repairPhenotype()
        {
            List<Room> roomIds = map.getRooms();

            //Flood fill each room ID
            foreach (Room r in roomIds)
            {

                List<Tile> visited = floodFillFunction(r);

                // Houston.... we have a problem. - If this happens then a repair must be made on the remaining tile group.
                while (r.getTiles().Count != visited.Count)
                {
                    if (logger != null)
                    {
                        logger.writeLogTimeStamp("Repair needed for Room: " + r.getID() + "\n");
                    }

                    // First lets see if the visited group is actually smaller then the other group.
                    List<Tile> remaining_tiles = new List<Tile>(r.getTiles());
                    foreach (Tile v in visited)
                    {
                        remaining_tiles.Remove(v);
                    }

                    if (remaining_tiles.Count < visited.Count)
                    {
                        // the remaining tiles are assimilated to adjacent rooms
                        foreach (Tile reAssign in remaining_tiles)
                        {
                            Tile[] neighbours = map.obtainNeighbours(reAssign);
                            foreach (Tile t in neighbours)
                            {
                                if (t != null && t.getRoom().getID() != r.getID())
                                {
                                    // Remove from the current room
                                    r.removeTile(map.getTileByID(reAssign.getID()));

                                    // Add to the other room
                                    map.getTileByID(reAssign.getID()).setRoom(map.getRoomByID(t.getRoom().getID()));
                                    map.getRoomByID(t.getRoom().getID()).addTile(map.getTileByID(reAssign.getID()));
                                    break;
                                }
                            }
                        }

                    }
                    else
                    {
                        // the visited tiles are assimilated to adjacent rooms
                        foreach (Tile reAssign in visited)
                        {
                            Tile[] neighbours = map.obtainNeighbours(reAssign);
                            foreach (Tile t in neighbours)
                            {
                                if (t != null && t.getRoom().getID() != r.getID())
                                {
                                    // Remove from the current room
                                    r.removeTile(map.getTileByID(reAssign.getID()));

                                    // Add to the other room
                                    map.getTileByID(reAssign.getID()).setRoom(map.getRoomByID(t.getRoom().getID()));
                                    map.getRoomByID(t.getRoom().getID()).addTile(map.getTileByID(reAssign.getID()));
                                    break;
                                }
                            }
                        }
                    }

                    visited = floodFillFunction(r); // Repeat flood fill to verify if this room is fully fixed.
                }
            }

            //// Fix IDs
            //int id_counter = 0;
            //foreach (Room r in map.getRooms())
            //{
            //    r.setID(id_counter);
            //    id_counter++;
            //}

        }

        // Create Flood Fill Function Here!
        public List<Tile> floodFillFunction(Room r)
        {
            Queue<Tile> toVisit = new Queue<Tile>();
            toVisit.Enqueue(r.getTiles()[0]);

            List<Tile> visited = new List<Tile>();
            while (toVisit.Count > 0)
            {
                Tile t = toVisit.Dequeue();
                Tile[] neighbours = map.obtainNeighbours(t);

                foreach (Tile n in neighbours)
                {
                    if (n != null && !toVisit.Contains(n) && !visited.Contains(n) && n.getRoom().getID() == r.getID())
                    {
                        toVisit.Enqueue(n);
                    }
                }
                visited.Add(t);
            }
            return visited;
        }

        // NEW Code Here! Get Alternative Paths -- More Accurate Sound
        public List<List<Room>> getAllPossiblePaths()
        {
            
            // In the current implementation, the longest path will always be the critical path.
            Queue<List<Room>> currentPaths = new Queue<List<Room>>();
            List<List<Room>> completedPaths = new List<List<Room>>();


            // Flood fill
            List<Room> initPath = new List<Room>();
            initPath.Add(map.getRoomByID(0));
            currentPaths.Enqueue(initPath);

            while (currentPaths.Count > 0)
            {
                List<Room> r = currentPaths.Dequeue();
                List<Room> neighbours = r[r.Count-1].getRoomNeighbours();

                // There should be at least one neighbour!
                
                if(r.Count > 1)
                {
                    neighbours.Remove(r[r.Count - 2]); // Remove the previous as neighbour.
                }
                
                if(neighbours.Count > 0)
                {
                    foreach (Room n in neighbours)
                    {
                        if (n != null && !r.Contains(n))
                        {
                            List<Room> altPath = new List<Room>(r);
                            altPath.Add(n);
                            currentPaths.Enqueue(altPath);
                        }
                    }
                }
                else
                {
                    completedPaths.Add(r);
                }
                
            }
            return completedPaths;
        }

        public Phenotype clonePheno()
        {
            return new Phenotype(this);
        }

        public void setIndividualFitnesses(double multipath_fit, double monster_fit)
        {
            this.multipath_fit = multipath_fit;
            this.monster_fit = monster_fit;
        }

        public double getMultiPathFitness()
        {
            return this.multipath_fit;
        }

        public double getMonsterFitness()
        {
            return this.monster_fit;
        }

        // Returns the item that is further away
        public SpawnPoint getMainItem()
        {
            return mainItem;   
        }

        public void setMainSpawnItem(SpawnPoint sp)
        {
            this.mainItem = sp;
        }

        public void setRoomIDPath(List<List<int>> roomIDPaths)
        {
            this.roomIDPath = roomIDPaths;
        }

        public List<int> getMainPath()
        {
            foreach (List<int> p in getRoomIDPaths())
            {
                if (p[p.Count - 1] == getMainItem().getRoom())
                {
                    return p;
                }
            }
            return null;
        }

        public List<List<int>> getAllSubPaths()
        {
            List<List<int>> subpaths = new List<List<int>>();
            foreach (List<int> p in getRoomIDPaths())
            {
                if (p[p.Count - 1] != getMainItem().getRoom())
                {
                    subpaths.Add(p);
                }
            }
            return subpaths;
        }

        public List<List<int>> getRoomIDPaths()
        {
            return this.roomIDPath;
        }

        public void setAnxietyMap(List<MyTuple> anxiety_map)
        {
            this.anxiety_map = anxiety_map;
        }

        public List<MyTuple> getAnxietyMap()
        {
            return anxiety_map;
        }

        public string unityMap()
        {
            return "";
        }


    }
}
