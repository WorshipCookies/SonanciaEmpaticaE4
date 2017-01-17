using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Util
{
    public class MyTuple : IComparable<MyTuple>
    {

        private int id;
        private double value;

        public MyTuple(int id, double value)
        {
            this.id = id;
            this.value = value;
        }

        public int getID()
        {
            return this.id;
        }

        public double getValue()
        {
            return this.value;
        }

        public void setID(int id)
        {
            this.id = id;
        }

        public void setValue(double value)
        {
            this.value = value;
        }

        public MyTuple copy()
        {
            return new MyTuple(id, value);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj.GetType() != typeof(MyTuple))
            {
                return false;
            }
            else
            {
                MyTuple tup = (MyTuple)obj;
                return (this.id == tup.id && this.value == tup.value);
            }

        }

        public int CompareTo(MyTuple other)
        {
            if (other.value > this.value)
            {
                return -1;
            }
            else if (other.value == this.value)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
