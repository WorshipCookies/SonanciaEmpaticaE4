using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Visual.Spawn
{
    public class SoundFXSpawnElement : ISpawnElement
    {
        private int type;
        private double tension_val;

        public SoundFXSpawnElement()
        {
            this.type = 3;
            this.tension_val = 0.2;
        }

        public int getTypeID()
        {
            return type;
        }

        public double tensionValue()
        {
            return tension_val;
        }
    }
}
