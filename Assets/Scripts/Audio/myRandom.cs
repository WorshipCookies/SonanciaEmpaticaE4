using System.Collections;
using System.Security.Cryptography;
using System;

public static class myRandom  {


    private static System.Random _random;

    public static float Next(float inclusiveLowerBound, float inclusiveUpperBound)
    {
        if (_random == null)
        {
            var cryptoResult = new byte[4];
            new RNGCryptoServiceProvider().GetBytes(cryptoResult);

            int seed = BitConverter.ToInt32(cryptoResult, 0);

            _random = new Random(seed);
        }

        return (float)_random.NextDouble() * (inclusiveUpperBound - inclusiveLowerBound) + inclusiveLowerBound;
   }

    public static int Next(int inclusiveLowerBound, int inclusiveUpperBound)
    {
        if (_random == null)
        {
            var cryptoResult = new byte[4];
            new RNGCryptoServiceProvider().GetBytes(cryptoResult);

            int seed = BitConverter.ToInt32(cryptoResult, 0);

            _random = new Random(seed);
        }

        // upper bound of Random.Next is exclusive
        int exclusiveUpperBound = inclusiveUpperBound + 1;
        return _random.Next(inclusiveLowerBound, exclusiveUpperBound);
    }

}
