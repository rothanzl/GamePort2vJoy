using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Linq;
using System.Diagnostics;
using System.Threading;

namespace FeederDemoCS
{
    class SerialLink
    {

        private int mComNum;
        static SerialPort mSerialPort;


        public SerialLink(int comNum)
        {
            mComNum = comNum;


        }

        public static string[] PortNames()
        {
            return SerialPort.GetPortNames();
        }

        public void OpenPortAndRead()
        {
            string portName = new List<string>(PortNames()).FirstOrDefault(n => n.ToLower().Equals("com" + mComNum));
            if (portName == null)
            {
                Loger.Error("Cannot open port");
                return;
            }

            if(mSerialPort != null)
            {
                try
                {
                    mSerialPort.Close();
                }
                catch { }
            }

            mSerialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
            mSerialPort.ReadTimeout = 250;
            mSerialPort.WriteTimeout = 250;

            mSerialPort.Open();


            Read();
        }


        public void Read()
        {
            int samplesPerSecond = 100;
            int cycleTime = 1000 / samplesPerSecond;

            while (true)
            {
                try
                {
                    
                    if (mSerialPort.BytesToRead > 500)
                    {
                        Loger.Warn($"Serial ling overfloded by {mSerialPort.BytesToRead} bytes. Is cleaning all buffer!");
                        mSerialPort.ReadExisting();
                        mSerialPort.ReadLine();
                    }

                    string line = mSerialPort.ReadLine();
                    ResolveLine(line);

                    Program.UpdateState();
                    
                    

                }
                catch (Exception e)
                {

                    Loger.Error($"Read exception, wait 1s and try again");
                    Thread.Sleep(1000);
                    OpenPortAndRead();
                }
            }

            mSerialPort.Close();
        }

        private void ResolveLine(string line)
        {
            try
            {
                string[] messages = line.Split(';');
                foreach (string message in messages)
                {
                    if (String.IsNullOrEmpty(message) || String.IsNullOrWhiteSpace(message)) continue;
                    string[] keyVal = message.Split(':');
                    ResolveKeyValue(keyVal[0], keyVal[1]);
                }
            }catch(Exception e)
            {
                Loger.Error($"Parse error: {line}");
            }
            
        }

        private void ResolveKeyValue(string key, string value)
        {
            if (!Int32.TryParse(key.Substring(1), out int index)){
                Loger.Error($"Cannot convert string {key.Substring(1)} to Int32.");
                return;
            }

            switch (key[0])
            {
                case 'a':
                    if(!Int32.TryParse(value, out int intValue))
                    {
                        Loger.Error($"Cannot convert string {value} to int.");
                        return;
                    }
                    SerialStates.Axes[index] = intValue;
                    break;
                case 'b':
                    bool boolValue;

                    if (value.Equals("0")) boolValue = true;
                    else if (value.Equals("1")) boolValue = false;
                    else
                    {
                        Loger.Error($"Cannot convert string {value} to bool.");
                        return;
                    }

                    SerialStates.Buttons[index] = boolValue;
                    break;
            }
        }

    }
}
