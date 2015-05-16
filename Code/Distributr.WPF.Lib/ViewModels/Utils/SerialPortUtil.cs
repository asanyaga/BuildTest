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
    public class SerialPortUtil : DistributrViewModelBase
    {

        private WeighScaleType scaletype;
        //public  string ReverseString(string s)
        //{
        //    char[] arr = s.ToCharArray();
        //    Array.Reverse(arr);
        //    return new string(arr);
        //}

       
        static SerialPort _serialPort = new SerialPort();

        private string _portname;

        public string PortName { get { return _portname; } set { _portname = value; } }

        private static int _baudRate;

        public int BaudRate { get { return _baudRate; } set { _baudRate = value; } }

        private Parity _parity=Parity.None;

        public Parity Parity
        {
            get
            {
                return _parity;
            }
            set
            {
                _parity = value;
            }
        }

        private StopBits _stopBits;

        public StopBits StopBits
        {
            get { return _stopBits; }
            set { _stopBits = value; }
        }

        private Handshake _handshake;
        public Handshake HandShake
        {
            get { return _handshake; }
            set { _handshake = value; }
        }

        private SerialData _serialData;
        public SerialData SerialData
        {
            get { return _serialData; }
            set { _serialData = value; }
        }

        private SerialError _serialError;
        public SerialError SerialError
        {
            get { return _serialError; }
            set { _serialError = value; }
        }

        private string _portError;

        public string PortError
        {
            get { return _portError; }
            set { _portError = value; }
        }

        private int _dataBits;

        public int DataBits { get { return _dataBits; } set { _dataBits = value; } }

        public bool IsDeviceReady
        {
            get; set;
        }

        public  decimal Read()
          {
            try
            {
              //  var weig1 = Weigh();//GO TODO:HAs error =>let go and weigh a second time.
                //Thread.Sleep(1000);
              

                var weight = Task.Run( delegate
                {
                   
                    return Weigh();
                });
                return weight.Result;

            }catch(Exception ex)
            {
                MessageBox.Show("Error reading from device,verify configuration\nDetails:" + ex.Message, "Agrimanagr Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
          }
        string line = "";
        private  decimal Weigh()
        {
          
          
            try
            {
                
                //line = _serialPort.ReadLine().Trim();


             int buffersize=   _serialPort.ReadBufferSize;
             byte[] buffer = new byte[buffersize];
               // Thread.Sleep(new TimeSpan(0, 0, 0, 0, 5));
             _serialPort.Read(buffer, 0, buffersize);
                line = System.Text.Encoding.UTF8.GetString(buffer);
               
                //split string based on the = sign which is used by the weighing scale Display to separate readings
                string[] values;
                if (scaletype == WeighScaleType.Octagon || scaletype == WeighScaleType.HangingScale || scaletype == WeighScaleType.EndelDR150)
                {
                    values = Regex.Split(line, @"\r\nW"); //tag.Split(@"\r\nW");
                }
                else if (scaletype == WeighScaleType.GSC)
                {
                    values = Regex.Split(line, @"\nS");
                }
                else if (scaletype == WeighScaleType.EndelBWS)
                {
                    values = Regex.Split(line, @"\r\n");
                }
                else
                    values = line.Split('=');
                //Pick the send array value which will always be accurate since it will alwys be between two = signs
                line = values.Length < 2 ? values[0] : values[values.Length-3];
                //Remove any unwanted characters to leave decimals only
                line = Regex.Replace(line, @"[^\d\.]", "");
                //reverse string to get the correct reading. The display sends the readings in the reverse order
                if (scaletype != WeighScaleType.Octagon &&
                    scaletype != WeighScaleType.GSC &&
                    scaletype != WeighScaleType.EndelDR150 &&
                    scaletype != WeighScaleType.HangingScale &&
                     scaletype != WeighScaleType.EndelBWS
                    )
                    line = ReverseString(line);
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
           catch(Exception ex)
           {
               Thread.CurrentThread.Abort();
               throw;
           }
            return 0;
        }

        private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {
            var port=sender as SerialPort;
          line=  port.ReadLine();
        }
        int Receive(byte[] bytes, int offset, int count)
        {
            int readBytes = 0;

            if (count > 0)
            {
                readBytes = _serialPort.Read(bytes, offset, count);
            }

            return readBytes;
        }
        public bool DoClose()
        {
            if(_serialPort ==null)
                return true;
            _serialPort.Close();
            if (_serialPort != null)
            {
                _serialPort.Dispose();
                _serialPort = null;

            }
            IsDeviceReady = false;
            return true;
        }

        #region Filter input string
        public static string ReverseString(string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
       
        private string Filter(string tag)
        {
            //Check if contains KG
            string pat = @"^(?!.*kg).*$";// @"(\w+)\s+(kg)";

            // Instantiate the regular expression object.
            Regex r = new Regex(pat, RegexOptions.IgnoreCase);

            // Match the regular expression pattern against a text string.
            Match m = r.Match(tag);
            if (!m.Success)
            {
                //Remove any unwanted characters to leave decimals only
                tag = Regex.Replace(tag, @"[^\d\.]", "");

            }
            //split string based on the = sign which is used by the weighing scale Display to separate readings
            if (tag.Contains("="))
            {
                string[] values = tag.Split('=');
                //Pick the send array value which will always be accurate since it will alwys be between two = signs
                tag = values[1];
            }
            //Remove any unwanted characters to leave decimals only
            tag = Regex.Replace(tag, @"[^\d\.]", "");
            //reverse string to get the correct reading. The display sends the readings in the reverse order

            /*To be uncommented */
            //tag = ReverseString(tag);
            /*End*/

            //Remove leading zeros

            Regex.Split(tag, @"[^0-9\.]+").Where(c => c != "");
            return tag;
        }

        #endregion
        public bool Write(Guid deliveryId, Document document)
        {
            if (!_serialPort.IsOpen)
                _serialPort.PortName = PortName;
            _serialPort.BaudRate = BaudRate;

            _serialPort.Handshake = HandShake;
            _serialPort.StopBits = StopBits;
            _serialPort.Parity = Parity;
            _serialPort.NewLine = "\n";
           
            _serialPort.WriteTimeout = 50000;
           
                if (_serialPort.IsOpen)
                {
                    try
                    {
                        _serialPort.WriteLine("Test Text");
                     /*   if (document != null)
                        {
                            header = "Farmer No. " + "\t" + document.tblCostCentre2.Code + "\n" + "Name" + "\t" + document.tblCostCentre2.Name + "\n" + "Date\t" + DateTime.Now.ToShortDateString()
                                + "\n" + "Transaction No." + "\t" + " " + document.Ref;
                        }

                        string printOut = _ctx.GetSet(new tblPrintOut()).FirstOrDefault(n => n.DocumentId == deliveryId).PrintOut ?? "";
                        _serialPort.WriteLine(String.Format("{0}", header.Replace("\t", " " + ":" + " ")));
                        _serialPort.Write(new byte[] { 13, 10 }, 0, 2);

                        string toPrint = printOut.Replace("\\n", "");
                        _serialPort.WriteLine(String.Format("{0} ", toPrint.Replace("\t", " " + ":" + " ")));

                        if (document.LineItems != null)
                        {
                            _serialPort.Write(new byte[] { 13, 10 }, 0, 2);
                            _serialPort.Write(String.Format("{0}",
                                                            "Total Weight" + " : " +
                                                            document.tblLineItems.Sum(n => n.MeasuredWeight)));
                        }
                        */
                        _serialPort.Write(new byte[] { 13, 10 }, 0, 2);
                        _serialPort.Write(new byte[] { 13, 10 }, 0, 2);
                        _serialPort.Write(new byte[] { 13, 10 }, 0, 2);
                    }
                    catch (Exception e)
                    {
                        throw e;

                    }
                }
            
            return true;
        }

        public void Init(WeighScaleType weighScaleType, string port, int baudRate,int dataBits=8 )
        {
            scaletype = weighScaleType;
            if(!_serialPort.IsOpen)
            {
                _serialPort.PortName = port;
                _serialPort.BaudRate = baudRate;
                _serialPort.Handshake = HandShake;
                _serialPort.DataBits = dataBits;
                _serialPort.Parity = Parity.None;
                _serialPort.StopBits = StopBits.One;
                _serialPort.ReadTimeout = 50000;
                try
                {

                    _serialPort.Open();
                    IsDeviceReady = true;
                }
                catch (IOException ioEx)
                {
                   
                    IsDeviceReady = false;
                   
                    IsDeviceReady = false;

                    MessageBox.Show("Serial Port Error\nDetails:" + ioEx.Message);
                }
                catch (ObjectDisposedException odEx) // This catch is optional
                {

                    IsDeviceReady = false;

                    MessageBox.Show("Serial Port Error\nDetails:" + odEx.Message);
                    
                   
                }
               catch (Exception exception)
                {
                    IsDeviceReady = false;
                   
                   MessageBox.Show("Serial Port Error\nDetails:"+exception.Message);
                    
                }
            }

            
        }
    }
}

