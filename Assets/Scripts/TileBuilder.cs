using UnityEngine;
using System.Collections;

public class TileBuilder {

    private int ID;
    private int roomID;
    private bool isDoor;

    public TileBuilder(int ID, bool isDoor)
    {
        this.ID = ID;
        this.isDoor = isDoor;
    }

    public TileBuilder(int ID, bool isDoor, int roomID)
    {
        this.ID = ID;
        this.isDoor = isDoor;
        this.roomID = roomID;
    }

    public int getID()
    {
        return this.ID;
    }

    public bool isTileDoor()
    {
        return this.isDoor;
    }

    public int getRoomID()
    {
        return this.roomID;
    }

    public void setRoom(int roomID)
    {
        this.roomID = roomID;
    }

    public void setDoor(bool isDoor)
    {
        this.isDoor = isDoor;
    }

}
