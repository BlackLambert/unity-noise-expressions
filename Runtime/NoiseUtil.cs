using UnityEngine;
using Random = System.Random;

namespace SBaier.Expressions
{
    public static class NoiseUtil
    {
        public static int[] CreatePermutations(Random random)
        {
            int[] result = new int[512];
            int[] tempPerm = new int[256];

            for (int i = 0; i < 256; i++)
            {
                tempPerm[i] = i;
            }

            for (int i = 0; i < 256; i++)
            {
                int swapIndex = random.Next(256);
                (tempPerm[i], tempPerm[swapIndex]) = (tempPerm[swapIndex], tempPerm[i]);
            }

            for (int i = 0; i < 512; i++)
            {
                result[i] = tempPerm[i % 256];
            }

            return result;
        }

        public static Vector2 CreateOffset(Random random, int maxOffset)
        {
            float x = ((float)random.NextDouble() - 0.5f) * maxOffset;
            float y = ((float)random.NextDouble() - 0.5f) * maxOffset;
            return new Vector2(x, y);
        }
    }
}