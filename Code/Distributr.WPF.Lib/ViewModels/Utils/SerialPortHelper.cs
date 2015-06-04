using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Utility;

namespace Distributr.WPF.Lib.ViewModels.Utils
{
    public static class SerialPortHelper
    {

        private static WeighScaleType scaletype;
        static SerialPort _serialPort = new SerialPort();
        private static string _portname;
        private static int _baudRate;
        private static Parity _parity = Parity.None;
        private static StopBits _stopBits;
        private static Handshake _handshake;
        private static SerialData _serialData;
        private static SerialError _serialError;
        private static string _portError;
        private static int _dataBits;
        private static bool _isDeviceReady;


        public static bool IsDeviceReady
        {
            get { return _isDeviceReady && _serialPort.IsOpen; }
        }


        public static decimal Read()
        {
            try
            {
                //  var weig1 = Weigh();//GO TODO:HAs error =>let go and weigh a second time.
                //Thread.Sleep(1000);


                var weight = Task.Run(delegate
                {

                    return Weigh();
                });
                return weight.Result;

            }
            catch (Exception ex)
            {
                // MessageBox.Show("Error reading from device,ver1`ify configuration\nDetails:" + ex.Message, "Agrimanagr Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        private static decimal Weigh()
        {


            try
            {
                string line = "";


                int buffersize = _serialPort.ReadBufferSize;
                byte[] buffer = new byte[buffersize];

                for (int i = 0; i < 3; i++)
                {
                    _serialPort.Read(buffer, 0, buffersize);
                }

                line = System.Text.Encoding.UTF8.GetString(buffer);

                //split string based on the = sign which is used by the weighing scale Display to separate readings
                string[] values;
                switch (scaletype)
                {

                    case WeighScaleType.Octagon:
                    case WeighScaleType.HangingScale:
                    case WeighScaleType.EndelDR150:
                        values = Regex.Split(line, @"\r\nW");
                        break;
                    case WeighScaleType.GSC:
                        values = Regex.Split(line, @"\nS");
                        break;
                    case WeighScaleType.EndelBWS:
                        values = Regex.Split(line, @"\r\n");
                        break;
                    case WeighScaleType.BAYKONAS:
                        values = Regex.Split(line, @"\n\r\n");
                        break;
                    case WeighScaleType.Crane:
                        //Regex digitsOnly=new Regex(@"[^\d]");
                        //values = digitsOnly.Replace(line, "").ToString();
                        values = Regex.Split(line, @"\r");
                        break;
                    default:
                        values = line.Split('=');
                        break;

                }

                //Pick the send array value which will always be accurate since it will alwys be between two = signs
                line = values.Length < 2 ? values[0] : values[values.Length - 3];
                //Remove any unwanted characters to leave decimals only
                line = Regex.Replace(line, @"[^\d\.]", "");
                //reverse string to get the correct reading. The display sends the readings in the reverse order
                switch (scaletype)
                {
                    case WeighScaleType.Octagon:
                    case WeighScaleType.GSC:
                    case WeighScaleType.EndelDR150:
                    case WeighScaleType.HangingScale:
                    case WeighScaleType.EndelBWS:
                    case WeighScaleType.BAYKONAS:
                    case WeighScaleType.Crane:
                        line = line;
                        break;
                    default:
                        line = ReverseString(line);
                        break;

                }

                //Remove leading zeros
                var converted = line.Split(new char[] { '.' });
                if (converted.Length > 2)
                {
                    line = converted[0] + '.' + converted[1];
                }
                line = Convert.ToDecimal(line).ToString("#0.00");
                _serialPort.DiscardOutBuffer();
                _serialPort.DiscardInBuffer();


                //  line = Filter(line);

                decimal res;
                if (decimal.TryParse(line, out res))
                {
                    return res;
                }


            }
            catch (IOException ioEx)
            {
                Thread.CurrentThread.Abort();
                throw;
            }
            catch (ObjectDisposedException odEx) // This catch is optional
            {

                Thread.CurrentThread.Abort();
            }
            catch (Exception ex)
            {
                Thread.CurrentThread.Abort();
                throw;
            }
            return 0;
        }


        public static bool Close()
        {
            if (_serialPort == null)
                return true;
            _serialPort.Close();
            if (_serialPort != null)
            {
                _serialPort.Dispose();
                _serialPort = null;

            }
            _isDeviceReady = false;
            return true;
        }


        private static string ReverseString(string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }



        public static bool Init(out string message, WeighScaleType weighScaleType, string port, int baudRate, int dataBits = 8)
        {
            message = "";
            scaletype = weighScaleType;
            _serialPort = new SerialPort();
            if (!_serialPort.IsOpen)
            {
                _serialPort.PortName = port;
                _serialPort.BaudRate = baudRate;
                // _serialPort.Handshake = HandShake;
                _serialPort.DataBits = dataBits;
                _serialPort.Parity = Parity.None;
                _serialPort.StopBits = StopBits.One;
                _serialPort.ReadTimeout = 50000;
                try
                {

                    _serialPort.Open();
                    _isDeviceReady = true;
                }
                catch (IOException ioEx)
                {

                    _isDeviceReady = false;

                    _isDeviceReady = false;

                    message = "Serial Port Error\nDetails:" + ioEx.Message;
                }
                catch (ObjectDisposedException odEx) // This catch is optional
                {

                    _isDeviceReady = false;

                    message = "Serial Port Error\nDetails:" + odEx.Message;


                }
                catch (Exception exception)
                {
                    _isDeviceReady = false;

                    message = "Serial Port Error\nDetails:" + exception.Message;

                }
            }

            return _isDeviceReady;
        }
    }
}