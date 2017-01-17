using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Visual.Spawn
{
    public class SpawnElementFactory
    {
        public static ISpawnElement createSpawnElement(SpawnElementTypes type)
        {
            switch (type)
            {
                case SpawnElementTypes.ITEM:
                    return new ItemSpawnElement();

                case SpawnElementTypes.MONSTER:
                    return new MonsterSpawnElement();

                case SpawnElementTypes.LIGHT:
                    return new LightSpawnElement();

                case SpawnElementTypes.SOUNDFX:
                    return new SoundFXSpawnElement();
            }
            return null;
        }

        public static SpawnElementTypes getTypeOnID(int typeID)
        {
            switch (typeID)
            {
                case 0:
                    return SpawnElementTypes.ITEM;

                case 1:
                    return SpawnElementTypes.MONSTER;

                case 2:
                    return SpawnElementTypes.SUBITEM;

                case 3:
                    return SpawnElementTypes.SOUNDFX;

                case 4:
                    return SpawnElementTypes.LIGHT;
            }
            return SpawnElementTypes.UNDEFINED;
        }

    }
    
    public enum SpawnElementTypes
    {
        ITEM,
        SUBITEM,
        MONSTER,
        LIGHT,
        SOUNDFX,
        UNDEFINED    
    };

    


}
