using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class EmpaticaTimeConverter {

    public static Stopwatch gameTime; // Keep a reference of the game time

    public double currentTime;
    public double lastReference;

    public EmpaticaTimeConverter()
    {
        currentTime = 0;
        lastReference = 0;
    }

    public double updateTime(double timeOfWriting, double newReference)
    {
        // If there are no past references, it means that this stream has started now (we assume).
        // Kickoff by using the current game time and saving the reference for later use.
        if(lastReference == 0)
        {
            currentTime = timeOfWriting;
            lastReference = newReference;
            return currentTime;
        }
        else
        {
            double diff = newReference - lastReference;
            currentTime += diff;
            lastReference = newReference;
            return currentTime;
        }
    }

    public void resetTime()
    {
        currentTime = 0;
        lastReference = 0;
    }

}
