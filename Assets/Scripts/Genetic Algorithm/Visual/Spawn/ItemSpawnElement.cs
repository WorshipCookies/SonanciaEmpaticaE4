using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Visual.Spawn
{
    public class ItemSpawnElement : ISpawnElement
    {
        private int type;
        private double tension_val;

        public ItemSpawnElement()
        {
            this.type = 0;
            this.tension_val = 0;
        }

        public int getTypeID()
        {
            return type;
        }

        public double tensionValue()
        {
            return this.tension_val;
        }
    }
}
