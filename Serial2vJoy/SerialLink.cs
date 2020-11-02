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
            mSerialPort.ReadTimeout = 100;
            mSerialPort.WriteTimeout = 100;

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
            
            while (mContinue)
            {
                try
                {

                    if(mSerialPort.BytesToRead > 255)
                    {
                        Loger.Warn($"Serial ling overfloded by {mSerialPort.BytesToRead} bytes. Is cleaning all buffer!");
                        mSerialPort.ReadExisting();
                        mSerialPort.ReadLine();
                    }

                    string line = mSerialPort.ReadLine();
                    ResolveLine(line);
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

        }

    }
}
