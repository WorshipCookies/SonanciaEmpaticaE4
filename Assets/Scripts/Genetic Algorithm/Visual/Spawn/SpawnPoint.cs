using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Visual.Spawn
{
    public class SpawnPoint
    {
        private int spawnID;
        private int[] spawnInRoom;

        private Tile tile_placement;

        private static int id_count = 0;

        private ISpawnElement element;

        public static SpawnPoint SpawnPointFactory(int type)
        {
            SpawnPoint spawn = new SpawnPoint(id_count, type);
            id_count++;
            return spawn;
        }

        public static SpawnPoint SpawnPointFactory(int type, int roomID)
        {
            SpawnPoint spawn = new SpawnPoint(id_count, type, roomID);
            id_count++;
            return spawn;
        }

        public SpawnPoint(int id, Tile t, int type)
        {
            this.spawnID = id;
            this.tile_placement = t;
            this.spawnInRoom = new int[2];
            this.spawnInRoom[0] = type;
            this.spawnInRoom[1] = t.getRoom().getID();

            element = SpawnElementFactory.createSpawnElement(SpawnElementFactory.getTypeOnID(type));
        }

        private SpawnPoint(int id, int type)
        {
            this.spawnID = id;
            spawnInRoom = new int[2];
            spawnInRoom[0] = type;

            element = SpawnElementFactory.createSpawnElement(SpawnElementFactory.getTypeOnID(type));
        }

        private SpawnPoint(int id, int type, int roomID)
        {
            this.spawnID = id;
            spawnInRoom = new int[2];
            spawnInRoom[0] = type;
            spawnInRoom[1] = roomID;

            element = SpawnElementFactory.createSpawnElement(SpawnElementFactory.getTypeOnID(type));
        }

        public void setRoom(int roomID)
        {
            spawnInRoom[1] = roomID;
        }

        public void setType(int type)
        {
            spawnInRoom[0] = type;
        }

        public int getType()
        {
            return spawnInRoom[0];
        }

        public int getRoom()
        {
            return spawnInRoom[1];
        }

        public void setTile(Tile t)
        {
            this.tile_placement = t;
        }

        public Tile getTile()
        {
            return this.tile_placement;
        }

        public int getSpawnID()
        {
            return this.spawnID;
        }

        public ISpawnElement getElement()
        {
            return element;
        }

    }
}
