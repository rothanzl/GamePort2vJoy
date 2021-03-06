﻿/////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// This project demonstrates how to write a simple vJoy feeder in C#
//
// You can compile it with either #define ROBUST OR #define EFFICIENT
// The fuctionality is similar - 
// The ROBUST section demonstrate the usage of functions that are easy and safe to use but are less efficient
// The EFFICIENT ection demonstrate the usage of functions that are more efficient
//
// Functionality:
//	The program starts with creating one joystick object. 
//	Then it petches the device id from the command-line and makes sure that it is within range
//	After testing that the driver is enabled it gets information about the driver
//	Gets information about the specified virtual device
//	This feeder uses only a few axes. It checks their existence and 
//	checks the number of buttons and POV Hat switches.
//	Then the feeder acquires the virtual device
//	Here starts and endless loop that feedes data into the virtual device
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

// Don't forget to add this
using vJoyInterfaceWrap;

namespace FeederDemoCS
{
    class Program
    {
        static public bool Verbose { get; set; }
        

        // Declaring one joystick (Device id 1) and a position structure. 
        static public vJoy joystick;
        static public vJoy.JoystickState iReport;
        static public uint id = 0;

        static public SerialLink serialLink;
        public static SerialStates serialStates = new SerialStates();

        static void Main(string[] args)
        {
            // Create one joystick object and a position structure.
            joystick = new vJoy();
            iReport = new vJoy.JoystickState();

            Verbose = args.Any(l => l.ToLower().Equals("v"));
            SerialStates.CenterInsensibility = args.Any(l => l.ToLower().Equals("i"));

            // Device ID can only be in the range 1-16
            if (args.Length>1 && !String.IsNullOrEmpty(args[0]) && !String.IsNullOrEmpty(args[1]))
            {
                id = Convert.ToUInt32(args[0]);
                serialLink = new SerialLink(Convert.ToInt32(args[1]));
            }
            else
            {
                Console.WriteLine($"Input parameter vJoy id and com port!");
                return;
            }
            if (id <= 0 || id > 16)
            {
                Console.WriteLine("Illegal device ID {0}\nExit!",id); 
                return;
            }


            // Get the driver attributes (Vendor ID, Product ID, Version Number)
            if (!joystick.vJoyEnabled())
            {
                Console.WriteLine("vJoy driver not enabled: Failed Getting vJoy attributes.\n");
                return;
            }
            else
                Console.WriteLine("Vendor: {0}\nProduct :{1}\nVersion Number:{2}\n", joystick.GetvJoyManufacturerString(), joystick.GetvJoyProductString(), joystick.GetvJoySerialNumberString());

            // Get the state of the requested device
            VjdStat status = joystick.GetVJDStatus(id);
            switch (status)
            {
                case VjdStat.VJD_STAT_OWN:
                    Console.WriteLine("vJoy Device {0} is already owned by this feeder\n", id);
                    break;
                case VjdStat.VJD_STAT_FREE:
                    Console.WriteLine("vJoy Device {0} is free\n", id);
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    Console.WriteLine("vJoy Device {0} is already owned by another feeder\nCannot continue\n", id);
                    return;
                case VjdStat.VJD_STAT_MISS:
                    Console.WriteLine("vJoy Device {0} is not installed or disabled\nCannot continue\n", id);
                    return;
                default:
                    Console.WriteLine("vJoy Device {0} general error\nCannot continue\n", id);
                    return;
            };

            // Check which axes are supported
            bool AxisX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X);
            bool AxisY = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y);
            bool AxisZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Z);
            bool AxisRX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RX);
            bool AxisRY = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RY);
            bool AxisRZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RZ);
            // Get the number of buttons and POV Hat switchessupported by this vJoy device
            int nButtons = joystick.GetVJDButtonNumber(id);
            int ContPovNumber = joystick.GetVJDContPovNumber(id);
            int DiscPovNumber = joystick.GetVJDDiscPovNumber(id);

            // Print results
            Console.WriteLine("\nvJoy Device {0} capabilities:\n", id);
            Console.WriteLine("Numner of buttons\t\t{0}\n", nButtons);
            Console.WriteLine("Numner of Continuous POVs\t{0}\n", ContPovNumber);
            Console.WriteLine("Numner of Descrete POVs\t\t{0}\n", DiscPovNumber);
            Console.WriteLine("Axis X\t\t{0}\n", AxisX ? "Yes" : "No");
            Console.WriteLine("Axis Y\t\t{0}\n", AxisY ? "Yes" : "No");
            Console.WriteLine("Axis Z\t\t{0}\n", AxisZ ? "Yes" : "No");
            Console.WriteLine("Axis Rx\t\t{0}\n", AxisRX ? "Yes" : "No");
            Console.WriteLine("Axis Ry\t\t{0}\n", AxisRY ? "Yes" : "No");
            Console.WriteLine("Axis Rz\t\t{0}\n", AxisRZ ? "Yes" : "No");

            // Test if DLL matches the driver
            UInt32 DllVer = 0, DrvVer = 0;
            bool match = joystick.DriverMatch(ref DllVer, ref DrvVer);
            if (match)
                Console.WriteLine("Version of Driver Matches DLL Version ({0:X})\n", DllVer);
            else
                Console.WriteLine("Version of Driver ({0:X}) does NOT match DLL Version ({1:X})\n", DrvVer, DllVer);


            // Acquire the target
            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id))))
            {
                Console.WriteLine("Failed to acquire vJoy device number {0}.\n", id);
                return ;
            }
            else
                Console.WriteLine("Acquired: vJoy device number {0}.\n", id);



            
            joystick.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_X, ref SerialStates.AxeMaxVal);

            serialLink.OpenPortAndRead();
            



        }




        internal static void UpdateState()
        {
            if (Verbose)
                Loger.InfoContinue($"Axe1X {SerialStates.Axes[0]} Axe1Y {SerialStates.Axes[1]} Axe2X {SerialStates.Axes[2]} Axe2y {SerialStates.Axes[3]}");
            

            iReport.bDevice = (byte)id;

            iReport.AxisX = SerialStates.ConverAxeValue(0);
            iReport.AxisY = SerialStates.ConverAxeValue(1);

            iReport.AxisXRot = SerialStates.ConverAxeValue(2);
            iReport.AxisYRot = SerialStates.ConverAxeValue(3);


            iReport.bHats = SerialStates.HatStates;

            iReport.Buttons = 0;
            if (!(SerialStates.Buttons[0] && SerialStates.Buttons[1]))
            {
                for (int i = 0; i < SerialStates.Buttons.Length; i++)
                {
                    if (SerialStates.Buttons[i])
                        iReport.Buttons = iReport.Buttons | ((uint)0b1 << i);
                }
            }



            if (!joystick.UpdateVJD(id, ref iReport))
            {
                Loger.Error($"Feeding vJoy device number {id} failed - wait 1s");
                Thread.Sleep(1000);
                joystick.AcquireVJD(id);
            }
        }
    } // class Program
} // namespace FeederDemoCS
