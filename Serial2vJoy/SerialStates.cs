using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace FeederDemoCS
{
    class SerialStates
    {

        public static int[] Axes = { 0, 0, 0, 0 };
        public static bool[] Buttons = { false, false, false, false };



        public static uint HatStates
        {
            get
            {
                if(!(Buttons[0] && Buttons[1])) return 0b100; // neutral

                uint result = 0b100;

                if(!Buttons[2] && !Buttons[3]) result = 0b11; // left
                if(Buttons[2] && !Buttons[3]) result = 0b10; // down
                if(!Buttons[2] && Buttons[3]) result = 0b1; // right
                if(Buttons[2] && Buttons[3]) result = 0b0; // up

                return result;
            }
        }


        private static readonly int SerialMaxVal = 1023;

        public static long AxeMaxVal = 0;

        public static int ConverAxeValue(int b)
        {
            double perCent = ((double)b) / SerialMaxVal;
            int result = (int)Math.Round(perCent * AxeMaxVal);
            if(result > AxeMaxVal || result < 0)
            {
                throw new Exception($"ConverAxeValue b={b}. Result out of range");
            }
            return result;
        }



    }
}
