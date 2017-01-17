using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.GeneticInterfaces;
using ProjectMaze.Util;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.GeneticAlgorithm
{
    public class AddRoomMutation : IMutation
    {

        private double mutation_rate;

        private ILogger logger;

        private static string name_ID = "Add Room Mutation";

        public AddRoomMutation(double mutation_rate)
        {
            this.mutation_rate = mutation_rate;
            this.logger = null;
        }

        public void mutate(IGenotype geno)
        {
            try
            {
                //Console.WriteLine("Add Room Mutation Function");

                // Cast to from IGenotype to Genotype 
                Genotype gen = (Genotype)geno;

                // Data Structure has this format [ID_Room, Height, Width]
                List<int[]> room_sizes = new List<int[]>();
                List<int> room_ids = new List<int>();

                // Get the Size of each room. The biggest room will mutate.
                for (int i = 0; i < gen.getGeno().GetLength(0); i++)
                {
                    for (int j = 0; j < gen.getGeno().GetLength(1); j++)
                    {
                        bool exists = false;
                        foreach (int[] s in room_sizes)
                        {
                            if ( s[0] == gen.getGeno()[i,j] )
                            {
                               exists = true;
                               s[1]++; 
                            }
                        }
                        if (!exists)
                        {
                            int[] new_entry = { gen.getGeno()[i,j], 1};
                            room_sizes.Add(new_entry);
                            room_ids.Add(gen.getGeno()[i,j]);
                        }
                    }
                }

                // Get the largest room
                int[] largest_room = getLargestRoom(room_sizes);

                // Divide the largest room into two smaller rooms
                int[] newDoor = divideLargestRoom(largest_room, room_ids, gen);

                gen.addDoors(newDoor[0], newDoor[1]);

                // Door mutation goes here
                // Now Handle the Doors ... (For now lets just add a door for each adjacent room) - Not using this at the moment!!
                //List<int[]> roomIDs = new List<int[]>();
                //for (int i = 0; i < gen.getGeno().GetLength(0) - 1; i++)
                //{
                //    for (int j = 0; j < gen.getGeno().GetLength(1) - 1; j++)
                //    {
                //        // Check Right
                //        if (gen.getGeno()[i, j] != gen.getGeno()[i, j + 1])
                //        {
                //            // Lowest ID value is ALWAYS in the first position 
                //            int[] temp = new int[2];
                //            if (gen.getGeno()[i, j] < gen.getGeno()[i, j + 1])
                //            {
                //                temp[0] = gen.getGeno()[i, j];
                //                temp[1] = gen.getGeno()[i, j + 1];
                //            }
                //            else
                //            {
                //                temp[0] = gen.getGeno()[i, j + 1];
                //                temp[1] = gen.getGeno()[i, j];
                //            }

                //            bool flag = true;

                //            foreach (int[] r in roomIDs)
                //            {
                //                if (r[0] == temp[0] && r[1] == temp[1])
                //                {
                //                    flag = false;
                //                }
                //            }

                //            if (flag)
                //            {
                //                roomIDs.Add(temp);
                //            }
                //        }

                //        // Check Down
                //        if (gen.getGeno()[i, j] != gen.getGeno()[i + 1, j])
                //        {
                //            // Lowest ID value is ALWAYS in the first position 
                //            int[] temp = new int[2];
                //            if (gen.getGeno()[i, j] < gen.getGeno()[i + 1, j])
                //            {
                //                temp[0] = gen.getGeno()[i, j];
                //                temp[1] = gen.getGeno()[i + 1, j];
                //            }
                //            else
                //            {
                //                temp[0] = gen.getGeno()[i + 1, j];
                //                temp[1] = gen.getGeno()[i, j];
                //            }

                //            bool flag = true;

                //            foreach (int[] r in roomIDs)
                //            {
                //                if (r[0] == temp[0] && r[1] == temp[1])
                //                {
                //                    flag = false;
                //                }
                //            }

                //            if (flag)
                //            {
                //                roomIDs.Add(temp);
                //            }
                //        }
                //    }

                //    gen.getDoors().Clear();
                //    foreach (int[] r in roomIDs)
                //    {
                //        gen.addDoors(r[0], r[1]);
                //    }
                //}

                if (gen.getDoors().Count < 1)
                {
                    //Console.WriteLine("We Have a Problem! In ADD ROOM MUTATION!");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void setMutationRate(double mutation_rate)
        {
            this.mutation_rate = mutation_rate;
        }

        public double getMutationRate()
        {
            return mutation_rate;
        }

        public void setLogger(LogSystem.Interfaces.ILogger log)
        {
            this.logger = log;
        }

        private int[] getLargestRoom(List<int[]> room_sizes)
        {
            int max_size = 0;
            int biggest_room = -1;
            
            // Get the biggest Room
            int[] gotcha = null;
            foreach (int[] r in room_sizes)
            {
                if(biggest_room == -1)
                {
                    max_size = r[1];
                    biggest_room = r[0];
                    gotcha = r;

                } 
                else if(max_size < r[1] ) 
                {
                    max_size = r[1];
                    biggest_room = r[0];
                    gotcha = r;
                }
            }
            return gotcha;
        }

        private int[] divideLargestRoom(int[] largestRoom, List<int> room_ids, Genotype geno)
        {
            double slice;

            List<int> width = new List<int>();
            List<int> height = new List<int>();

            // Width Calc
            for (int i = 0; i < geno.getGeno().GetLength(0); i++)
            {
                int line_width = 0;
                for (int j = 0; j < geno.getGeno().GetLength(1); j++)
                {
                    if (largestRoom[0] == geno.getGeno()[i, j])
                        line_width++;
                }
                width.Add(line_width);
            }

            // Height Calc
            for (int i = 0; i < geno.getGeno().GetLength(1); i++)
            {
                int line_height = 0;
                for (int j = 0; j < geno.getGeno().GetLength(0); j++)
                {
                    if (largestRoom[0] == geno.getGeno()[j, i])
                    {
                        line_height++;
                    }
                }
                height.Add(line_height);
            }

            int coord_x = 0;
            int max_value_x = 0;
            for (int i = 0; i < height.Count; i++)
            {
                if (max_value_x < height[i])
                {
                    max_value_x = height[i];
                    coord_x = i;
                }
            }

            int coord_y = 0;
            int max_value_y = 0;
            for (int i = 0; i < width.Count; i++)
            {
                if (max_value_y < width[i])
                {
                    max_value_y = width[i];
                    coord_y = i;
                }
            }

            // Slice Horizontally or Vertically
            if (max_value_x == max_value_y)
            {
                slice = MyRandom.getRandom().random().NextDouble(); // If its a draw, choose randomly.
            }
            else if (max_value_x < max_value_y) // If Height Lower than Width - Cut Vertically
            {
                slice = 0.0;
            }
            else // If Height is above the Width -- Cut Horizontally
            {
                slice = 1.0;
            }

            // Get new Room ID
            room_ids.Sort();
            bool is_added = false;
            int chosen_id = -1;
            for (int i = 0; i < room_ids.Count-1; i++)
            {
                if(i != room_ids[i])
                {
                    // Add at the beginning
                    if (i == 0 && i < room_ids[i])
                    {
                        room_ids.Add(i);
                        chosen_id = i;
                        is_added = true;
                        break;
                    }

                    // Add in the middle
                    if (room_ids[i] < i && i < room_ids[i + 1])
                    {
                        room_ids.Add(i);
                        chosen_id = i;
                        is_added = true;
                        break;
                    }
                }
            }

            if (!is_added)
            {
                chosen_id = room_ids[room_ids.Count - 1] + 1;
                room_ids.Add(chosen_id);
            }

            if (slice < 0.5)
            {
                // Vertical Cut
                for (int i = 0; i < geno.getGeno().GetLength(0); i++)
                {
                    int counter_slice = max_value_y / 2;
                    for (int j = 0; j < geno.getGeno().GetLength(1); j++)
                    {
                        if (geno.getGeno()[i, j] == largestRoom[0] && counter_slice > 0)
                        {
                            geno.getGeno()[i, j] = chosen_id;
                            counter_slice--;
                        }
                    }
                }
            }
            else
            {
                // Horizontal Cut
                for (int i = 0; i < geno.getGeno().GetLength(1); i++)
                {
                    int counter_slice = max_value_x / 2;
                    for (int j = 0; j < geno.getGeno().GetLength(0); j++)
                    {
                        if (geno.getGeno()[j, i] == largestRoom[0] && counter_slice > 0)
                        {
                            geno.getGeno()[j, i] = chosen_id;
                            counter_slice--;
                        }
                    }
                }
            }

            int[] splitRooms = { largestRoom[0], chosen_id };

            // Logging System
            if (logger != null)
            {
                logger.writeLogTimeStamp(" ------- " + name_ID + " ------- \n");
                logger.writeLog("New Room: " + chosen_id + " was created from dividing Room: " + largestRoom[0] +"\n");
            }

            return splitRooms;
        }
    }
}
