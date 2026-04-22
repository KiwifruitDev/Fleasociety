using System;
using System.Collections.Generic;

namespace KMGEngine
{
    /// <summary>
    /// This class stores useful global variables and functions.
    /// </summary>
    public static class Global
    {
        public static string productName = "KMGEngine";
        public static string productVersion = "0.0.0.0";
        public static bool exiting = false;
        public static bool fakeExit = false;
        public static float exitOpacityIncrease = 0.0075f;
        public static bool ready = false;
        public static double readyTime = 0;
        public static string editing = "";
        public static List<string> parameters = new List<string>();
        public static string tooltip = ""; 
        // Aspect ratio functions
        public static (int, int) ConvertToFraction(double aspectRatio, double tolerance = 0.01)
        {
            int numerator = 1;
            int denominator = 1;

            while (Math.Abs((double)numerator / denominator - aspectRatio) > tolerance)
            {
                if ((double)numerator / denominator < aspectRatio)
                    numerator++;
                else
                    denominator++;
            }

            // Simplify the fraction
            int gcd = GCD(numerator, denominator);
            numerator /= gcd;
            denominator /= gcd;

            return (numerator, denominator);
        }
        public static int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }
    }
}
