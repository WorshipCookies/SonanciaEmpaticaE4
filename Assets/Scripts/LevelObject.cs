using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProjectMaze.Util;
using ProjectMaze.GeneticAlgorithm;
using ProjectMaze.Visual;
using Assets.Scripts;

public class LevelObject {


    private int numTileHeight;
    private int numTileWidth;
    private int[] level;
    private List<GameObject> tiles;
    private List<DoorObject> doors;
    private List<int> items;
    private List<int> monsters;
    private List<int> sub_items;
    private List<int> lights;
    private List<int> soundFX;


    private int totalRooms;
    private Dictionary<int, List<int>> roomTiles;
    public Dictionary<int, List<GameObject>> roomMeshList; // Room Meshes by Room ID
    public Dictionary<int, GameObject> wallMeshList; // Wall Meshes by Room ID

    private Phenotype genetic_map;

    private List<List<Room>> allPossiblePaths;

    public LevelObject(int[] level, List<DoorObject> doors, List<int> items, List<int> monsters, List<int> sub_items, List<int> lights, List<int> soundFX, 
        int numTileHeight, int numTileWidth, Phenotype genetic_map)
    {
        this.level = level;
        this.doors = doors;
        this.items = items;
        this.monsters = monsters;
        this.lights = lights;
        this.soundFX = soundFX;
        this.sub_items = sub_items;
        this.numTileHeight = numTileHeight;
        this.numTileWidth = numTileWidth;

        this.genetic_map = genetic_map;

        tiles = new List<GameObject>();
        buildRoomDictionary();

        allPossiblePaths = genetic_map.getAllPossiblePaths();
    }

    public int[] getLevel()
    {
        return this.level;
    }

    public List<int> getItems()
    {
        return this.items;
    }

    public List<int> getMonsters()
    {
        return this.monsters;
    }

    public List<int> getSubItems()
    {
        return this.sub_items;
    }

    public List<int> getLights()
    {
        return this.lights;
    }

    public List<int> getSoundFX()
    {
        return this.soundFX;
    }

    public int getNumTileHeight()
    {
        return numTileHeight;
    }

    public int getNumTileWidth()
    {
        return numTileWidth;
    }

    public void addTileObject(GameObject Tile)
    {
        tiles.Add(Tile);
    }

    public GameObject getTiles(int index)
    {
        return tiles[index];
    }

    private void buildRoomDictionary()
    {
        totalRooms = 0;
        roomTiles = new Dictionary<int, List<int>>();
        for (int i = 0; i < level.Length; i++)
        {
            int roomId = level[i];
            if (roomId < 0)
                roomId = roomId * -1;
            if (!roomTiles.ContainsKey(roomId))
            {
                List<int> newtiles = new List<int>();
                newtiles.Add(i);
                roomTiles.Add(roomId, newtiles);
                totalRooms++;
            }
            else
            {
                roomTiles[roomId].Add(i);
            }
        }
    }

    public int getTotalRoomNum()
    {
        return totalRooms;
    }

    public List<int> getRoomTileIDs(int roomID)
    {
        return roomTiles[roomID];
    }

    public List<DoorObject> getDoors()
    {
        return doors;
    }

    // Get neighbours of a specific tile. Neighbours returned in a fashion [UP, DOWN, LEFT, RIGHT], if a neighbour doesn't exist, returns null in its place.
    public int[] obtainNeighbours(int tile)
    {
        // if no tiles error.
        if (tiles.Count <= 0)
            return null;

        int[] neighbours = new int[4];

        // First check and see if the tile is in a extremity of the map
        // LEFT Extremity Check
        if ((tile % LevelBuilder.WIDTH_TILE_NUM) == 0)
        {
            neighbours[2] = -1;
        }
        else
        {
            neighbours[2] = tile-1;
        }

        // RIGHT Extremity Check
        if ((tile % LevelBuilder.WIDTH_TILE_NUM) == (LevelBuilder.WIDTH_TILE_NUM - 1))
        {
            neighbours[3] = -1;
        }
        else
        {
            neighbours[3] = tile + 1;
        }

        // UP Extremity Check
        if (tile < LevelBuilder.WIDTH_TILE_NUM)//Map.HEIGHT_TILE_SIZE)
        {
            neighbours[0] = -1;
        }
        else
        {
            neighbours[0] = tile - LevelBuilder.WIDTH_TILE_NUM;
        }

        //DOWN Extremity Check
        if (tile >= ((LevelBuilder.HEIGHT_TILE_NUM - 1) * LevelBuilder.WIDTH_TILE_NUM))
        {
            neighbours[1] = -1;
        }
        else
        {
            neighbours[1] = tile + LevelBuilder.WIDTH_TILE_NUM;
        }

        return neighbours;
    }

    public List<int> getAllAdjacentTiles(int roomID)
    {
        List<int> adjacentTiles = new List<int>();
        roomID = roomID - 1;

        foreach (int i in roomTiles[roomID])
        {
            int[] neighbours = obtainNeighbours(i);

            if(neighbours != null)
            {
                foreach (int j in neighbours)
                {
                    if (j == -1 || level[j] != level[i]) // If the tile is on an extremity than put it in the list.
                    {
                        adjacentTiles.Add(i);
                    }
                }
            }
        }
        return adjacentTiles;
    }

    public Phenotype getGeneticMap()
    {
        return genetic_map;
    }

    public List<int> roomsOutsidePath()
    {
        List<int> outsidePath = new List<int>();
        for (int i = 1; i <= totalRooms; i++)
        {
            outsidePath.Add(i);
        }

        foreach (MyTuple t in genetic_map.getAnxietyMap())
        {
            if (outsidePath.Contains( t.getID()+1) )
            {
                outsidePath.Remove( t.getID()+1 );
            }
        }
        return outsidePath;
    }

    public Dictionary<int, List<GameObject>> getRoomMeshList()
    {
        return roomMeshList;
    }

    public List<int> getAdjacentTileIDs(int roomID)
    {
        List<int[]> adjId = genetic_map.getMap().getAllRoomAdjacentTiles(genetic_map.getMap().getRoomByID(roomID-1));
        List<int> adjList = new List<int>();
        foreach(int[] t in adjId)
        {
            adjList.Add(t[0]); // This is the tile from the current room, the other is the tile from the adjacent room.
        }
        return adjList;
    }

    // Determines the Angle and the corner type.
    public List<WallCorner> detectCorners()
    {
        WallCorner.resetDictionary(); // Always reset the dictionary.
        List<WallCorner> IDisCorner = new List<WallCorner>();

        Map m = genetic_map.getMap();
        List<Tile> tiles = m.getTiles();

        foreach (Tile t in tiles)
        {
            // If the tile has a door -- Ignore it! Its not a corner.

            if(t.getDoor() != null)
            {
                int roomID = t.getRoom().getID(); // Get ID of the current tile.
                Tile[] neighbours = m.obtainNeighbours(t); // Get the tiles neighbours.

                // Remember the array is [UP, DOWN, LEFT, RIGHT]
                bool[] isSameRoom = { true, true, true, true };  // We first assume that the tile is in the same room. If not then it is false.
                int detCorner = 0; // This counts the number of corners to determine which type of corner it is (if any).
                                   // Find the neighbours that aren't of the same room id.
                for (int i = 0; i < neighbours.Length; i++)
                {
                    if (neighbours[i] == null)
                    {
                        isSameRoom[i] = false;
                        detCorner++;
                    }
                    else if (neighbours[i].getRoom().getID() != roomID)
                    {
                        if (t.getDoor() == null || t.getDoor().getOpposite(t).getID() != neighbours[i].getID()) // Make sure that the door does not get included!
                        {
                            isSameRoom[i] = false;
                            detCorner++;
                        }
                    }
                }

                if (detCorner == 2)
                {
                    // Ignore the walls that are oposite of each other -- That means where UP && DOWN == False || LEFT && RIGHT == False
                    if (!((isSameRoom[0] == false && isSameRoom[1] == false) || (isSameRoom[2] == false && isSameRoom[3] == false)))
                    {

                        // This Confirms that we have a corner -- Now we need to determine its positioning.
                        CornerType type = CornerType.TWOWALL;

                        float angle = 0f;

                        // FOR THIS MODEL ROTATE ON THE Y AXIS!
                        // There is a wall UP and LEFT
                        if (isSameRoom[0] == false && isSameRoom[2] == false)
                        {
                            angle = 180f; // -- Old 90
                        }
                        // There is a wall UP and RIGHT.
                        else if (isSameRoom[0] == false && isSameRoom[3] == false)
                        {
                            angle = 90f; // -- Old 0f
                        }
                        // There is a wall DOWN and LEFT
                        else if (isSameRoom[1] == false && isSameRoom[2] == false)
                        {
                            angle = 270f; // -- Old 180f
                        }
                        // There is a wall DOWN and RIGHT
                        else if (isSameRoom[1] == false && isSameRoom[3] == false)
                        {
                            angle = 0f; // -- Old 270f
                        }
                        IDisCorner.Add(new WallCorner(t.getID(), type, angle)); // Add Corner to the List
                    }
                }
                else if (detCorner == 3)
                {
                    // Cul-de-sac or dead end sections will require 3 sided walls
                    // FOR THIS MODEL ROTATE ON THE Y AXIS!

                    // This Confirms that we have a corner -- Now we need to determine its positioning.
                    CornerType type = CornerType.THREEWALL;
                    float angle = 0f;

                    // Wall is UP and LEFT and RIGHT
                    if (isSameRoom[0] == false && isSameRoom[2] == false && isSameRoom[3] == false)
                    {
                        angle = 0f;
                    }
                    // Wall is UP and DOWN and LEFT
                    else if (isSameRoom[0] == false && isSameRoom[1] == false && isSameRoom[2] == false)
                    {
                        angle = 90f;
                    }
                    // Wall is UP and DOWN and RIGHT
                    else if (isSameRoom[0] == false && isSameRoom[1] == false && isSameRoom[3] == false)
                    {
                        angle = 270f;
                    }
                    // Wall is DOWN and LEFT and RIGHT -- This is the default position.
                    else if (isSameRoom[1] == false && isSameRoom[2] == false && isSameRoom[3] == false)
                    {
                        angle = 180f;
                    }

                    IDisCorner.Add(new WallCorner(t.getID(), type, angle));
                }
            }
        }
        return IDisCorner;
    }

    // Returns the Room based on the Tile ID
    public int getRoomByTileID(int tileID)
    {
        return genetic_map.getMap().getTileByID(tileID).getRoom().getID();
    }

    public List<List<Room>> getAllPossiblePaths()
    {
        return allPossiblePaths;
    }
}
