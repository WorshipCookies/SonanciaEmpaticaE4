using UnityEngine;
using System.Collections;

public class DoorBuilder {

    private int roomID1;
    private int roomID2;

    private int tileID1;
    private int tileID2;

    public DoorBuilder(int roomID1, int roomID2, int tileID1, int tileID2)
    {
        this.roomID1 = roomID1;
        this.roomID2 = roomID2;
        this.tileID1 = tileID1;
        this.tileID2 = tileID2;
    }

    public int getRoom1ID()
    {
        return roomID1;
    }

    public int getRoom2ID()
    {
        return roomID2;
    }

    public int getTile1ID()
    {
        return tileID1;
    }

    public int getTile2ID()
    {
        return tileID2;
    }
}
