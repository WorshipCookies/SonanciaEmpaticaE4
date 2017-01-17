using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.TensionMapGeneration.TensionMapFitnesses
{
    public class PeakValleyCalculation
    {
        // Determines if a position is a peak or not.
        public static bool calculateSpike(double[] tension_map, int pos)
        {
            // Make sure that if pos == 0; it is definetely not a peak
            if (pos == 0)
                return false;

            // If it is at the end of the tension_map it is also not a peak!
            if (pos >= tension_map.Length - 1)
                return false;

            return tension_map[pos + 1] < tension_map[pos] && tension_map[pos - 1] < tension_map[pos]; // If the current Position is higher than the previous and next position then it is a peak!
        }

        // Determines if a position is a valley or not.
        public static bool calculateValley(double[] tension_map, int pos)
        {
            // Make sure that if pos == 0; it is definetely not a peak
            if (pos == 0)
                return false;

            // If it is at the end of the tension_map it is also not a peak!
            if (pos >= tension_map.Length - 1)
                return false;

            return tension_map[pos + 1] > tension_map[pos] && tension_map[pos - 1] > tension_map[pos]; // If the current Position is higher than the previous and next position then it is a peak!
        }

        // Calculates the height of the highest peak!
        public static double calculateSpikeHeight(double[] tension_map)
        {
            List<int> spikes = new List<int>();
            for(int i = 0; i < tension_map.Length; i++)
            {
                if (calculateSpike(tension_map, i))
                {
                    spikes.Add(i);
                }
            }

            if (spikes.Count == 0)
            {
                return 0;
            }
            else
            {
                double currentHighestPeak = 0.0;
                foreach(int i in spikes)
                {
                    double leftValue = tension_map[i-1];
                    // Go left until it goes up again
                    for (int j = i-1; j > 0; j--)
                    {
                        if (tension_map[j] > tension_map[j - 1])
                        {
                            leftValue = tension_map[j - 1];
                        }
                        else
                        {
                            break; // Leave the for Loop
                        }
                    }

                    leftValue = Math.Abs(leftValue - tension_map[i]);

                    double rightValue = tension_map[i+1];
                    // Go right until it goes up again
                    for (int j = i+1; j < tension_map.Length-1; j++)
                    {
                        if(tension_map[j] > tension_map[j + 1])
                        {
                            rightValue = tension_map[j + 1];
                        } else
                        {
                            break;
                        }
                    }

                    rightValue = Math.Abs(rightValue - tension_map[i]);

                    if(currentHighestPeak == 0.0)
                    {
                        currentHighestPeak = Math.Max(leftValue, rightValue);
                    }
                    else
                    {
                        double comparePeak = Math.Max(leftValue, rightValue);
                        if(comparePeak > currentHighestPeak)
                        {
                            currentHighestPeak = comparePeak;
                        }
                    }
                }
                return currentHighestPeak;
            }
        }

        // Calculates the deepest valley
        public static double calculateValleyDepth(double[] tension_map)
        {
            List<int> valleys = new List<int>();
            for (int i = 0; i < tension_map.Length; i++)
            {
                if (calculateValley(tension_map, i))
                {
                    valleys.Add(i);
                }
            }

            if (valleys.Count == 0)
            {
                return 0;
            }
            else
            {
                double currentLowestPeak = 0.0;
                foreach (int i in valleys)
                {
                    double leftValue = tension_map[i - 1];
                    // Go left until it goes up again
                    for (int j = i - 1; j > 0; j--)
                    {
                        if (tension_map[j] < tension_map[j - 1])
                        {
                            leftValue = tension_map[j - 1];
                        }
                        else
                        {
                            break; // Leave the for Loop
                        }
                    }

                    leftValue = Math.Abs(leftValue - tension_map[i]);

                    double rightValue = tension_map[i + 1];
                    // Go right until it goes up again
                    for (int j = i + 1; j < tension_map.Length - 1; j++)
                    {
                        if (tension_map[j] < tension_map[j + 1])
                        {
                            rightValue = tension_map[j + 1];
                        }
                        else
                        {
                            break;
                        }
                    }

                    rightValue = Math.Abs(rightValue - tension_map[i]);

                    if (currentLowestPeak == 0.0)
                    {
                        currentLowestPeak = Math.Max(leftValue, rightValue);
                    }
                    else
                    {
                        double comparePeak = Math.Max(leftValue, rightValue);
                        if (comparePeak > currentLowestPeak)
                        {
                            currentLowestPeak = comparePeak;
                        }
                    }
                }
                return currentLowestPeak;
            }
        }

        public static int getHighestPeakIndex(double[] tension_map)
        {
            List<int> spikes = new List<int>();
            for (int i = 0; i < tension_map.Length; i++)
            {
                if (calculateSpike(tension_map, i))
                {
                    spikes.Add(i);
                }
            }

            if (spikes.Count == 0)
            {
                return -1;
            }
            else
            {
                double currentHighestPeak = 0.0;
                int currentIndex = 0;
                foreach (int i in spikes)
                {
                    double leftValue = tension_map[i - 1];
                    // Go left until it goes up again
                    for (int j = i - 1; j > 0; j--)
                    {
                        if (tension_map[j] > tension_map[j - 1])
                        {
                            leftValue = tension_map[j - 1];
                        }
                        else
                        {
                            break; // Leave the for Loop
                        }
                    }

                    leftValue = Math.Abs(leftValue - tension_map[i]);

                    double rightValue = tension_map[i + 1];
                    // Go right until it goes up again
                    for (int j = i + 1; j < tension_map.Length - 1; j++)
                    {
                        if (tension_map[j] > tension_map[j + 1])
                        {
                            rightValue = tension_map[j + 1];
                        }
                        else
                        {
                            break;
                        }
                    }

                    rightValue = Math.Abs(rightValue - tension_map[i]);

                    if (currentHighestPeak == 0.0)
                    {
                        currentHighestPeak = Math.Max(leftValue, rightValue);
                        currentIndex = i;
                    }
                    else
                    {
                        double comparePeak = Math.Max(leftValue, rightValue);
                        if (comparePeak > currentHighestPeak)
                        {
                            currentHighestPeak = comparePeak;
                            currentIndex = i;
                        }
                    }
                }
                return currentIndex;
            }
        }

        public static int totalPeaks(double[] tension_map)
        {
            int peakNum = 0;
            for(int i = 0; i < tension_map.Length; i++)
            {
                if (calculateSpike(tension_map, i))
                {
                    peakNum++;
                }
            }
            return peakNum;
        }

        public static bool anyPeak(double[] tension_map)
        {
            for(int i = 0; i < tension_map.Length; i++)
            {
                if (calculateSpike(tension_map, i))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
