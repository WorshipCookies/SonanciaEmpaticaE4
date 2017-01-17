using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Visual.Spawn
{
    public class MonsterSpawnElement : ISpawnElement
    {
        private int type;
        private double tension_val;

        public MonsterSpawnElement()
        {
            this.type = 1;
            this.tension_val = 1.0;
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
