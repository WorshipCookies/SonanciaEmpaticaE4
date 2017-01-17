using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomBuilder {

    private int roomID;
    private List<TileBuilder> tiles;

    public RoomBuilder(int roomID)
    {
        this.roomID = roomID;
        tiles = new List<TileBuilder>();
    }

    public int getID()
    {
        return roomID;
    }

    public void addTile(TileBuilder tile)
    {
        tiles.Add(tile);
    }

    public List<TileBuilder> getTiles()
    {
        return tiles;
    }
}
