using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Distributr.WPF.Lib.ViewModels.Utils.WeighScale_Lib
{
    public class DataStreamEventArgs : EventArgs
    {
        #region Defines
        private byte[] _bytes;
        #endregion

        #region Constructors
        public DataStreamEventArgs(byte[] bytes)
        {
            _bytes = bytes;
        }
        #endregion

        #region Properties
        public byte[] Response
        {
            get { return _bytes; }
        }
        #endregion
    }

    public class SerialClient : IDisposable
    {
        #region Defines
        private string _port;
        private int _baudRate;
        private SerialPort _serialPort;
        private Thread serThread;
        private double _PacketsRate;
        private DateTime _lastReceive;
        /*The Critical Frequency of Communication to Avoid Any Lag*/
        private const int freqCriticalLimit = 20;
        #endregion

        #region Constructors
        public SerialClient(string port)
        {
            _port = port;
            _baudRate = 9600;
            _lastReceive = DateTime.MinValue;

            serThread = new Thread(new ThreadStart(SerialReceiving));
            serThread.Priority = ThreadPriority.Normal;
            serThread.Name = "SerialHandle" + serThread.ManagedThreadId;
        }
        public SerialClient(string Port, int baudRate)
            : this(Port)
        {
            _baudRate = baudRate;
        }
        #endregion

        #region Custom Events
        public event EventHandler<DataStreamEventArgs> OnReceiving;
        #endregion

        #region Properties
        public string Port
        {
            get { return _port; }
        }
        public int BaudRate
        {
            get { return _baudRate; }
        }
        public string ConnectionString
        {
            get
            {
                return String.Format("[Serial] Port: {0} | Baudrate: {1}",
                    _serialPort.PortName, _serialPort.BaudRate.ToString());
            }
        }
        #endregion

        #region Methods
        #region Port Control
        public bool OpenConn()
        {
            try
            {
                if (_serialPort == null)
                    _serialPort = new SerialPort(_port, _baudRate, Parity.None);

                if (!_serialPort.IsOpen)
                {
                    _serialPort.ReadTimeout = -1;
                    _serialPort.WriteTimeout = -1;

                    _serialPort.Open();

                    if (_serialPort.IsOpen)
                        serThread.Start(); /*Start The Communication Thread*/
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
        public bool OpenConn(string port, int baudRate)
        {
            _port = port;
            _baudRate = baudRate;

            return OpenConn();
        }
        public void CloseConn()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                serThread.Abort();

                if (serThread.ThreadState == ThreadState.Aborted)
                    _serialPort.Close();
            }
        }
        public bool ResetConn()
        {
            CloseConn();
            return OpenConn();
        }
        #endregion
        #region Transmit/Receive
        public void Transmit(byte[] packet)
        {
            _serialPort.Write(packet, 0, packet.Length);
        }
        public int Receive(byte[] bytes, int offset, int count)
        {
            int readBytes = 0;

            if (count > 0)
            {
                readBytes = _serialPort.Read(bytes, offset, count);
            }

            return readBytes;
        }
        #endregion
        #region IDisposable Methods
        public void Dispose()
        {
            CloseConn();

            if (_serialPort != null)
            {
                _serialPort.Dispose();
                _serialPort = null;
            }
        }
        #endregion
        #endregion

        #region Threading Loops
        private void SerialReceiving()
        {
            while (true)
            {
                int count = _serialPort.BytesToRead;

                /*Get Sleep Inteval*/
                TimeSpan tmpInterval = (DateTime.Now - _lastReceive);

                /*Form The Packet in The Buffer*/
                byte[] buf = new byte[count];
                int readBytes = Receive(buf, 0, count);

                if (readBytes > 0)
                {
                    OnSerialReceiving(buf);
                }

                #region Frequency Control
                _PacketsRate = ((_PacketsRate + readBytes) / 2);

                _lastReceive = DateTime.Now;

                if ((double)(readBytes + _serialPort.BytesToRead) / 2 <= _PacketsRate)
                {
                    if (tmpInterval.Milliseconds > 0)
                        Thread.Sleep(tmpInterval.Milliseconds > freqCriticalLimit ? freqCriticalLimit : tmpInterval.Milliseconds);

                  
                }
                #endregion
            }

        }
        #endregion

        #region Custom Events Invoke Functions
        private void OnSerialReceiving(byte[] res)
        {
            if (OnReceiving != null)
            {
                OnReceiving(this, new DataStreamEventArgs(res));
            }
        }
        #endregion
    }
}
