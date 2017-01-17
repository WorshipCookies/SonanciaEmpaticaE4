using UnityEngine;
using System.Collections.Generic;

public class LevelInfo {

    private List<TileBuilder> tiles;
    private List<RoomBuilder> rooms;
    private List<DoorBuilder> doors;

    public LevelInfo(bool static_level)
    {
        if (static_level)
        {
            // Build a level without GA's
            buildStaticLevel();
        }
        else
        {
            // This is empty for now GA goes here.
        }
    }


    private void buildStaticLevel()
    {
        // Create 14x14 Tile
        int idCount = 0;
        tiles = new List<TileBuilder>();
        rooms = new List<RoomBuilder>();
        
        //int totalTiles = LevelBuilder.HEIGHT_TILE_NUM * LevelBuilder.WIDTH_TILE_NUM;

        foreach (int i in LevelLoader.myStaticLevel())
        {
            RoomBuilder currentRoom = getRoom(i);

            if (currentRoom == null)
            {
                rooms.Add(new RoomBuilder(i));
                currentRoom = getRoom(i);
            }

            TileBuilder tile = new TileBuilder(idCount, false, i);
            tiles.Add(tile);
            currentRoom.addTile(tile);
            idCount++;
        }

        doors = LevelLoader.getDoors();

        foreach (DoorBuilder d in doors)
        {
            TileBuilder tile1 = getTile(d.getTile1ID());
            tile1.setDoor(true);

            TileBuilder tile2 = getTile(d.getTile2ID());
            tile2.setDoor(true);
        }
    }


    private RoomBuilder getRoom(int ID)
    {
        foreach (RoomBuilder r in rooms)
        {
            if (r.getID() == ID)
            {
                return r;
            }
        }
        return null;
    }

    private TileBuilder getTile(int ID)
    {
        foreach (TileBuilder t in tiles)
        {
            if (t.getID() == ID)
            {
                return t;
            }
        }

        return null;
    }
}
