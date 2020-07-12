using System;

namespace Helpers
{
    public static class RandomHelper
    {
        private static Random rand = new Random();

        public static int RandomInt(int min, int max)
        {
            return rand.Next(min, max);
        }
    }
}