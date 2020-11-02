using System;

namespace FeederDemoCS
{
    class SerialStates
    {

        public static int[] Axes = { 0, 0, 0, 0 };
        public static bool[] Buttons = { false, false, false, false };

        private static readonly int SerialMaxVal = 255;// 1023;

        public static long AxeMaxVal = 0;

        public static int ConverAxeValue(int b)
        {
            double perCent = ((double)b) / SerialMaxVal;
            return (int) Math.Round(perCent * AxeMaxVal);
        }



    }
}
