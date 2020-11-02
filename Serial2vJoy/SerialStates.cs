using System;

namespace FeederDemoCS
{
    class SerialStates
    {

        public static byte[] Axes = { 0, 0, 0, 0 };
        public static bool[] Buttons = { false, false, false, false };


        public static long AxeMaxVal = 0;

        public static int ConverAxeValue(byte b)
        {
            double perCent = ((double)b) / 255;
            return (int) Math.Round(perCent * AxeMaxVal);
        }



    }
}
