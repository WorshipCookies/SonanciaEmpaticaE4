using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class WallCorner
    {
        private int tileID;
        private CornerType type;
        private float angle;

        public static Dictionary<int,bool> cornerIDs = new Dictionary<int,bool>();

        public WallCorner(int tileID, CornerType type, float angle)
        {
            this.tileID = tileID;
            this.type = type;
            this.angle = angle;

            cornerIDs.Add(tileID, false); // Keep the IDs in a list so it is easier to look up later.
        }

        public int getTileID()
        {
            return tileID;
        }

        public CornerType getType()
        {
            return type;
        }

        public float getAngle()
        {
            return angle;
        }

        public static void resetDictionary()
        {
            cornerIDs.Clear();
        }
    }

    

    public enum CornerType
    {
        TWOWALL,
        THREEWALL
    }
}
