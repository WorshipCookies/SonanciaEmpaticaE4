using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DMap {

    Dictionary<int, DRoom> roomList;
    Dictionary<int, DTile> tileList;

    int TotalRooms;

    int xsize;
    int ysize;

    int tilesize;

    public DMap(int[] level, int xsize, int ysize, int tilesize)
    {
        this.roomList = new Dictionary<int, DRoom>();
        this.tileList = new Dictionary<int,DTile>();

        this.xsize = xsize;
        this.ysize = ysize;

        this.tilesize = tilesize;


        int currentY_Pos = -1;
        // Lets build the data structure for the levels.
        for (int i = 0; i < level.Length; i++)
        {
            int roomID = Mathf.Abs(level[i]); // We do this because values who have doors are tagged with -RoomID.
            
            int currentX_Pos = i % xsize; // This determines the current X position.

            // Keep track of the current y position.
            if (currentX_Pos == 0)
            {
                currentY_Pos++;
            }

            // Determine if the current tile has a door.
            bool is_connected = false;
            if (level[i] < 0)
            {
                is_connected = true;
            }

            // The X, Y of the tile is the current X position and current Y postion.
            DTile tile = new DTile(currentX_Pos, currentY_Pos, i, roomID, is_connected);
            tileList.Add(tile.getID(), tile);

            if (roomList.ContainsKey(roomID))
            {
                roomList[roomID].addTile(tile.getID(), tile);
            }
            else
            {
                roomList.Add(roomID, new DRoom(roomID));
                roomList[roomID].addTile(tile.getID(), tile);
            }

        }
    }

    public DRoom getRoom(int id)
    {
        return roomList[id];
    }

    public DTile getTile(int id)
    {
        return tileList[id];
    }

    public int getXSize()
    {
        return xsize;
    }

    public int getYSize()
    {
        return ysize;
    }

    public int getTileSize()
    {
        return tilesize;
    }
}
