using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DRoom {

    int id;
    Dictionary<int, DTile> tileList;

    public DRoom(int id)
    {
        this.id = id;
        tileList = new Dictionary<int, DTile>();
    }

    public void addTile(int tileID, DTile tile)
    {
        tileList.Add(tileID, tile);
    }

    public List<int> getAllTileIDs()
    {
        return new List<int>(tileList.Keys);
    }

    public DTile getTile(int id)
    {
        return tileList[id];
    }


}
