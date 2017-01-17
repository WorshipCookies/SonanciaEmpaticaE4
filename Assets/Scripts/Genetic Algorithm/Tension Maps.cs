using ProjectMaze.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.GeneticAlgorithm
{
    public class Tension_Maps
    {

        public static double[] U_Shape = { 2.0, 1.6, 1.2, 0.8, 0.4, 0.0, 0.4, 0.8, 1.2, 1.6, 2.0 };
        public static double[] Inverse_U = { 0.0, 0.4, 0.8, 1.2, 1.6, 2.0, 1.6, 1.2, 0.8, 0.4, 0.0 };
        public static double[] Linear = { 0.0, 0.2, 0.4, 0.6, 0.8, 1.0, 1.2, 1.4, 1.6, 1.8, 2.0 };
        public static double[] U_Wave = { 2.0, 1.4, 0.8, 0.0, 0.8, 1.4, 2.0, 1.4, 0.8, 0.0 };
        public static double[] inverse_U_Wave = { 0.0, 0.8, 1.4, 2.0, 1.4, 0.8, 0.0, 0.8, 1.4, 2.0 };
        public static double[] Zig_Zag = { 0.0, 0.5, 1.0, 1.5, 2.0, 1.0, 1.5, 1.0, 0.5, 0.0 };
        public static double[] No_Tension = { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
        public static double[] High_Tension = { 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0, 2.0 };
        public static double[] Extreme = { 5.0, 0.5, 1.0, 3.5, 4.0, 4.0, 1.5, 5.5, 0.5, 2.0 };


        public static double[] Linear_Short = { 0.0, 0.5, 1.0, 1.5, 2.0 };
        public static double[] Zig_Zag_Short = { 0.0, 1.0, 0.0, 1.0, 2.0 };
        public static double[] U_Wave_Short = { 1.0, 0.5, 0.0, 0.5, 1.0 };

        // Translates a map into a saw type map shape.
        public static List<MyTuple> translateMap(int roomNumber, double[] tension_shape)
        {
            int sample_rate = 0;
            if(roomNumber <= tension_shape.Length)
            {
                sample_rate = (int)Math.Round((double)tension_shape.Length / roomNumber);
            }
            else
            {
                return null;
            }

            List<MyTuple> sampled_map = new List<MyTuple>();

            int roomID = 0;
            int i = 0;

            if (sample_rate * roomNumber > tension_shape.Length+1)
            {
                sample_rate -= 1;
            }

            while (roomID < roomNumber)
            {
                if (i > tension_shape.Length)
                {
                    i = tension_shape.Length - 1;
                }

                sampled_map.Add(new MyTuple(roomID, tension_shape[i]));
                roomID++;
                i += sample_rate;
            }
            return sampled_map;
        }

    }
}
