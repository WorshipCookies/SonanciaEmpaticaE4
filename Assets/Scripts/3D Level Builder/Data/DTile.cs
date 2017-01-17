using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DTile {

    // The (x,y) coordinates of the tile.
    int x;
    int y;

    // Tile id + Room id
    int id;
    int room_id;

    // Bool states if this tile is connected to an adjacent room through a door.
    bool is_connected;

    List<DWall> walls;

    public DTile(int x, int y, int id, int room_id, bool is_connected)
    {
        this.x = x;
        this.y = y;

        this.id = id;
        this.room_id = room_id;

        this.is_connected = is_connected;

        this.walls = new List<DWall>();
    }

    // Returns an array of int with the lower and top coordinates of the tile (lowerX, lowerY, topX, topY).
    public int[] getTileCoordinates()
    {
        int[] coord = { x, y };
        return coord;
    }

    public int getX()
    {
        return x;
    }

    public int getY()
    {
        return y;
    }

    // Returns the ID of the tile.
    public int getID()
    {
        return id;
    }

    // Returns the ID of the room this tile is associated to.
    public int getRoomID()
    {
        return room_id;
    }
    
    // Returns true if this tile is connected to another adjacent room. False if it isn't.
    public bool isConnected()
    {
        return is_connected;
    }

    // Adds a new wall associated to this tile. This is useful for the 3D building phase.
    public void addWall(DWall wall)
    {
        walls.Add(wall);
    }

    // Gets all the walls that are associated to this tile.
    public List<DWall> getWalls()
    {
        return walls;
    }
}
