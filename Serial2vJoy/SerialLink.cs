using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Linq;
using System.Diagnostics;

namespace FeederDemoCS
{
    class SerialLink
    {

        private int mComNum;
        static SerialPort mSerialPort;
        private bool mContinue;
        private Thread mReadThread;


        public SerialLink(int comNum)
        {
            mComNum = comNum;


        }

        public static string[] PortNames()
        {
            return SerialPort.GetPortNames();
        }

        public bool OpenPort()
        {
            string portName = new List<string>(PortNames()).FirstOrDefault(n => n.ToLower().Equals("com" + mComNum));
            if (portName == null) return false;

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
            

            if(mReadThread == null) mReadThread = new Thread(Read);
            if (!mContinue)
            {
                mContinue = true;
                mReadThread.Start();
            }



            return true;
        }


        public void Read()
        {
            int samplesPerSecond = 100;
            int cycleTime = 1000 / samplesPerSecond;

            while (mContinue)
            {
                try
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    if (mSerialPort.BytesToRead > 255)
                    {
                        Loger.Warn($"Serial ling overfloded by {mSerialPort.BytesToRead} bytes. Is cleaning all buffer!");
                        mSerialPort.ReadExisting();
                        mSerialPort.ReadLine();
                    }

                    string line = mSerialPort.ReadLine();
                    ResolveLine(line);
                    Thread.Yield();

                    sw.Stop();
                    int sleepTime = cycleTime - (int)sw.ElapsedMilliseconds;
                    if (sleepTime > 0) Thread.Sleep(sleepTime);
                    else
                    {
                        Loger.Warn($"Getter loop sleep time {sleepTime}ms");
                        Thread.Yield();
                    }

                }
                catch (Exception e)
                {

                    Loger.Error($"Read exception, wait 1s and try again");
                    Thread.Sleep(1000);
                    OpenPort();
                }
            }

            mSerialPort.Close();
        }

        private void ResolveLine(string line)
        {
            string[] messages = line.Split(';');
            foreach(string message in messages)
            {
                if (String.IsNullOrEmpty(message) || String.IsNullOrWhiteSpace(message)) continue;
                string[] keyVal = message.Split(':');
                ResolveKeyValue(keyVal[0], keyVal[1]);
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
                    if(!Byte.TryParse(value, out byte byteValue))
                    {
                        Loger.Error($"Cannot convert string {value} to byte.");
                        return;
                    }
                    SerialStates.Axes[index] = byteValue;
                    break;
                case 'b':
                    bool boolValue = false;

                    if (value.Equals("1")) boolValue = true;
                    else if (value.Equals("0")) boolValue = false;
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
