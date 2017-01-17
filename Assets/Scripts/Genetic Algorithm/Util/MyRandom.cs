using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Util
{
    public class MyRandom
    {

        private static MyRandom myrand = null;
        private Random rand;

        private MyRandom()
        {
            rand = new Random();
        }

        public static MyRandom getRandom()
        {
            if (myrand == null)
                myrand = new MyRandom();

            return myrand;
        }

        public Random random()
        {
            return this.rand;
        }
    }
}
