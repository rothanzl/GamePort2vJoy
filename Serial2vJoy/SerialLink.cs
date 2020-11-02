using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Linq;

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
            byte[] oldMessageBuffer = new byte[0];
            while (mContinue)
            {
                try
                {
                    int size = mSerialPort.BytesToRead;
                    if (size == 0) continue;

                    byte[] bytes = new byte[size];
                    mSerialPort.Read(bytes, 0, size);

                    byte[] messageBuffer = new byte[size + oldMessageBuffer.Length];

                    Buffer.BlockCopy(oldMessageBuffer, 0, messageBuffer, 0, oldMessageBuffer.Length);
                    Buffer.BlockCopy(bytes, 0, messageBuffer, oldMessageBuffer.Length, bytes.Length);

                    int countNewLine = messageBuffer.Count(b => b == 0xA);
                    if (countNewLine == 0) // find new line
                    {
                        oldMessageBuffer = messageBuffer;
                        continue;
                    }



                    int passNewLine = 0;
                    string message = "";
                    for(int i = 0; i < messageBuffer.Length; i++)
                    {
                        byte b = messageBuffer[i];
                        if(b == 0xA)
                        {
                            passNewLine++;

                            if (passNewLine == countNewLine || countNewLine == 0)
                            {
                                oldMessageBuffer = new byte[messageBuffer.Length - i - 1];
                                Buffer.BlockCopy(messageBuffer, i + 1, oldMessageBuffer, 0, messageBuffer.Length - (i + 1));
                                break;
                            }

                            if (message.Contains("ba"))
                            {

                            }

                            Loger.Info($"Message: {message}");
                            message = "";
                            continue;
                        }
                        


                        if ((char)b == 'b')
                        {
                            message += "b";
                            for(int j = 0; j < 1; j++)
                            {
                                if(i+1 < messageBuffer.Length)
                                {
                                    i++;
                                    b = messageBuffer[i];
                                    message += $"'{b}'";
                                }
                            }
                            continue;
                        }
                        if ((char)b == 'a')
                        {
                            message += "a";
                            for (int j = 0; j < 2; j++)
                            {
                                if (i + 1 < messageBuffer.Length)
                                {
                                    i++;
                                    b = messageBuffer[i];
                                    message += $"'{b}'";
                                }
                            }
                            continue;
                        }

                        message += $"'{b}'";
                    
                    }

                }catch(Exception e)
                {

                    Loger.Info($"Read exception, wait 1s and try again");
                    Thread.Sleep(1000);
                    OpenPort();
                }
            }

            mSerialPort.Close();
        }


    }
}
