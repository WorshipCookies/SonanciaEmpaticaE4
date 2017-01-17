using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Visual.Spawn
{
    public class LightSpawnElement : ISpawnElement
    {
        private int type;
        private double tension_val;

        public LightSpawnElement()
        {
            this.type = 4;
            this.tension_val = - 0.2;
        }

        public int getTypeID()
        {
            return this.type;
        }

        public double tensionValue()
        {
            return this.tension_val;
        }
    }
}
