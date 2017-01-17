using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Util
{
    public class Tuple<T1,T2>
    {

        private T1 val1 { set; get; }
        private T2 val2 { set; get; }


        public Tuple(T1 val1, T2 val2)
        {
            this.val1 = val1;
            this.val2 = val2;
        }

        public T1 getFirst()
        {
            return val1;
        }

        public T2 getSecond()
        {
            return val2;
        }
    }
}
