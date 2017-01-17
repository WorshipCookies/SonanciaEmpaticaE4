using UnityEngine;
using System.Collections;

public class DWall {

    static int wallHeight = 5; // Height of all the walls in the map
    wallTypes type;
    bool is_door;

    public enum wallTypes { TOP, LEFT, RIGHT, LOWER };

    public DWall(wallTypes walltype, bool is_door)
    {
        this.type = walltype;
        this.is_door = is_door;
    }

    // The type of the wall is defined by where that wall is according to the tile.
    public wallTypes getType()
    {
        return type;
    }

    // States if this wall is actually a door or not!
    public bool isDoor()
    {
        return is_door;
    }
}
