using UnityEngine;
using System.Collections;

public class DoorObject {

    public int tile_id1;
    public int tile_id2;
    public int room_id1;
    public int room_id2;

    public DoorObject(int tile_id1, int tile_id2, int room_id1, int room_id2)
    {
        this.tile_id1 = tile_id1;
        this.tile_id2 = tile_id2;
        this.room_id1 = room_id1;
        this.room_id2 = room_id2;
    }

    public int getTileID1()
    {
        return this.tile_id1;
    }

    public int getTileID2()
    {
        return this.tile_id2;
    }

    public int getRoomID1()
    {
        return this.room_id1;
    }

    public int getRoomID2()
    {
        return this.room_id2;
    }
}
