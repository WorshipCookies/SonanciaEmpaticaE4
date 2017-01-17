using ProjectMaze.GeneticInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMaze.LogSystem.Interfaces;

namespace ProjectMaze.GeneticAlgorithm
{
    public class PlaceSoundFXMutation : IMutation
    {
        private double mutationRate;
        private ILogger logger;

        private static string name_ID = "SoundFX Place Spawn Mutation";

        private static int MAX_SOUNDFX_PER_ROOM = 1; // To avoid cacophony, don't over-exagerate on the sounds!

        public PlaceSoundFXMutation(double mutation_rate)
        {
            this.mutationRate = mutation_rate;
            this.logger = null;
        }

        public double getMutationRate()
        {
            return this.mutationRate;
        }

        public void mutate(IGenotype geno)
        {
            // This mutation adds or removes a soundfx from a room (i.e. Max Number of sound fx per Room = MAX_SOUNDFX_PER_ROOM).

            // Choose a random room. Randomly add or remove a soundfx from a room, obviously if no soundfx exist add soundfx only, or if the max number of soundfx 
            // has been achieved remove only.
            try
            {
                Genotype gen = (Genotype)geno;

                // Get All Room Id's currently Available.
                List<int> roomIds = gen.getAllRoomIDs();

                // Pick a room Id at random
                int pickedRoom = roomIds[Util.MyRandom.getRandom().random().Next(roomIds.Count)];

                // If a SoundFX is already in that room remove it; if not add it!
                List<int> pos_soundfx = isSoundInRoom(gen, pickedRoom);

                if (pos_soundfx.Count >= MAX_SOUNDFX_PER_ROOM)
                {
                    // If a soundfx exists remove it!
                    if (MAX_SOUNDFX_PER_ROOM > 1)
                    {
                        //Choose randomly
                        gen.getSpawnPoints().RemoveAt(pos_soundfx[Util.MyRandom.getRandom().random().Next(pos_soundfx.Count)]);
                    }
                    else
                    {
                        gen.getSpawnPoints().RemoveAt(pos_soundfx[0]); // Remove the soundfx!
                    }
                }
                else
                {
                    if (MAX_SOUNDFX_PER_ROOM > 1 && pos_soundfx.Count > 0)
                    {
                        // 50-50 chance of adding one more soundfx or removing it
                        if (Util.MyRandom.getRandom().random().NextDouble() < 0.5)
                        {
                            // Add SoundFX
                            int[] new_soundfx = { 3, pickedRoom };
                            gen.getSpawnPoints().Add(new_soundfx);
                        }
                        else
                        {
                            // Remove SoundFX by choosing randomly
                            gen.getSpawnPoints().RemoveAt(pos_soundfx[Util.MyRandom.getRandom().random().Next(pos_soundfx.Count)]);
                        }

                    }
                    else
                    {
                        // Can only add soundfx!
                        int[] new_soundfx = { 3, pickedRoom };
                        gen.getSpawnPoints().Add(new_soundfx);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void setLogger(ILogger log)
        {
            this.logger = log;
        }

        public void setMutationRate(double mutation_rate)
        {
            this.mutationRate = mutation_rate;
        }

        public List<int> isSoundInRoom(Genotype gen, int roomID)
        {
            List<int> pos = new List<int>();

            for (int i = 0; i < gen.getSpawnPoints().Count; i++)
            {
                if (gen.getSpawnPoints()[i][0] == 3 && gen.getSpawnPoints()[i][1] == roomID)
                {
                    pos.Add(i);
                }
            }
            return pos;
        }
    }
}
