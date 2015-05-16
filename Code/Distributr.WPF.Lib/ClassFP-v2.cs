using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Threading;
using System.IO.Ports;
//using System.Runtime.InteropServices.Automation;

namespace Distributr.WPF.Lib
{
    // Sample syntax how to use the FP class:
    // 
    // try

    // {
    //     using (FP test = new FP())
    //     {
    //         test.OpenPort("COM1", 115200);
    //         test.PrintLogo();
    //         test.PrintText("Test 1...");
    //     }
    // }
    // catch (Exception ex)
    // {
    //     if (ex.GetType() == typeof(FPException))
    //     {
    //         int err = ((FPException)ex).ErrorCode;
    //         MessageBox.Show(ex.Message, "Error " + err.ToString());
    //     }
    //     else
    //     {
    //         MessageBox.Show(ex.Message, "Error");
    //     }
    // }
    //
    // NOTE: If you need to extend or modify FP class, define a new class and set FP as base class.


    /// <summary>
    /// Tremol Fiscal Printer library (Freeware)
    /// Technical Support: software@tremol.bg
    /// </summary>
    public class FP : IDisposable
    {
        public bool WaitAfterSlowCommands = true;

        private string _country;
        public string Country
        {
            get
            {
                return _country;
            }
            set
            {
                _country = value.ToLower();

                switch (_country)
                {
                    case "bg":
                        TextEncoding = Encoding.GetEncoding("windows-1251");
                        TaxGroupCount = 8;
                        DeviceParamsCount = 8;
                        TaxGroupBase = '0';
                        FPException.lang = "bg";
                        break;

                    case "gr":
                        TextEncoding = Encoding.GetEncoding("windows-1253");
                        TaxGroupCount = 5;
                        DeviceParamsCount = 8;
                        TaxGroupBase = '1';
                        FPException.lang = "en";
                        break;

                    case "ke":
                        TextEncoding = Encoding.GetEncoding("UTF-8");
                        TaxGroupCount = 5;
                        DeviceParamsCount = 9;
                        TaxGroupBase = '1';
                        FPException.lang = "en";
                        break;

                    default:
                        throw new FPException(FPLibError.BAD_INPUT_DATA);
                }
            }
        }

        public Encoding TextEncoding;

        static int DeviceParamsCount;
        static int TaxGroupCount;
        const int PayTypesCount = 4;
        public int TextLineWidth = 38;
        public char TaxGroupBase;
        public int GSEndByte = -1;

        const byte STX = 2, ETX = 10, ANTIECHO = 3, PING = 4, BUSY = 5, ACK = 6, OUTOFPAPER = 7, PING_NEW = 9, RETRY = 0x0E, NACK = 0x15;
        const int ping_retries = 10, cmd_retries = 3;
        int rw_timeout = 2000;
        int gs_timeout = 200;
        int ping_timeout = 200;
        int busy_timeout = 20000;
        public byte[] last_response_raw;
        static byte next_cmd_id;
        bool? new_ping = null;

        #region FP Commands

        #region General commands

        public Status GetStatus()
        {
            byte[] res = SendCommand(true, " ");

            return new Status(res);
        }

        public string GetVersion()
        {
            return SendCommandGetString("!");
        }

        public void Diagnostic()
        {
            SendCommand(true, "\"");
            WaitSlowCommand();
        }

        public void ClearDisplay()
        {
            SendCommand(true, "{0}", (char)0x24);
        }

        public void DisplayLine1(string text)
        {
            SendCommand(true, "%{0}", fix_len(text, 20));
        }

        public void DisplayLine2(string text)
        {
            SendCommand(true, "&{0}", fix_len(text, 20));
        }

        public void Display(string text)
        {
            SendCommand(true, "{0}{1}", (char)0x27, fix_len(text, 40));
        }

        public void DisplayDateTime()
        {
            SendCommand(true, "(");
        }

        public void OpenCashDrawer()
        {
            SendCommand(true, "*");
        }

        public void PaperFeed()
        {
            SendCommand(true, "+");
        }

        public void PaperCut()
        {
            SendCommand(true, ")");
        }

        #endregion

        #region Fiscal commands

        public void SetSerialNumber(string password, string serialNum)
        {
            if (password.Length != 6 || serialNum.Length != 9)
                throw new FPException(FPLibError.BAD_INPUT_DATA);

            SendCommand(true, "{0}{1};{2}", (char)0x40, password, serialNum);
        }

        public void SetTaxNumber(string password, string taxNum, string fiscalNum)
        {
            if (password.Length != 6 || taxNum.Length != 15 || fiscalNum.Length != 12)
                throw new FPException(FPLibError.BAD_INPUT_DATA);

            SendCommand(true, "{0}{1};1;{2};{3}", (char)0x41, password, taxNum, fiscalNum);
        }

        public void Fiscalize(string password)
        {
            if (password.Length != 6)
                throw new FPException(FPLibError.BAD_INPUT_DATA);

            SendCommand(true, "{0}{1};2", (char)0x41, password);
        }

        public void SetTaxPecents(string password, decimal[] taxRates)
        {
            if (password.Length != 6 || taxRates.Length != TaxGroupCount)
                throw new FPException(FPLibError.BAD_INPUT_DATA);

            string cmd = string.Format("{0}{1}", (char)0x42, password);

            foreach (decimal tr in taxRates)
                cmd += string.Format(";{0}%", d2s(tr, 2));

            SendCommand(true, cmd);
        }

        public void SetDecimalPoint(string password, bool useFractions)
        {
            if (password.Length != 6)
                throw new FPException(FPLibError.BAD_INPUT_DATA);

            SendCommand(true, "{0}{1}:{2}", (char)0x43, password, Flag(useFractions, '2', '0'));
        }

        public void UpdateHeader(string password, char comCode)
        {
            if (password.Length != 6 || comCode < '0' || comCode > '3')
                throw new FPException(FPLibError.BAD_INPUT_DATA);

            SendCommand(true, "{0}{1};{2}", (char)0x57, comCode, password);
        }

        #endregion

        #region Programming commands

        public void SetPayType(FPPaymentType payType, string name)
        {
            if (payType == FPPaymentType.ForeignCurrency)
                throw new FPException(FPLibError.BAD_INPUT_DATA);

            SendCommand(true, "{0}{1};{2}", (char)0x44, (int)payType, fix_len(name, 10));
        }

        public void SetCurrency(string name, decimal rate)
        {
            SendCommand(true, "{0}{1};{2};{3}", (char)0x44, (int)FPPaymentType.ForeignCurrency, fix_len(name, 10), rate.ToString("0000.00000", CultureInfo.InvariantCulture));
        }

        public void SetParameters(Parameters pas)
        {
            SendCommand(true, "{0}{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}",
                (char)0x45, pas.POSNumber.ToString("####"), OzFlag(pas.PrintLogo), OzFlag(pas.OpenCashDrawer), OzFlag(pas.AutoCut),
                    OzFlag(pas.TransparentDisplay), OzFlag(pas.ShortEJ), OzFlag(pas.TotalInForeignCurrency),
                    OzFlag(pas.SmallFontEJ), OzFlag(pas.FreeTextInEJ), OzFlag(pas.SingleOperator));
        }

        public void SetDateTime(DateTime dt)
        {
            SendCommand(true, "{0}{1}", (char)0x48, dt.ToString("dd-MM-yy HH:mm:ss"));
            WaitSlowCommand();
        }

        public void SetHeaderLine(char index, string text)
        {
            if (index != ':' && (index < '0' && index > '9'))
                throw new FPException(FPLibError.BAD_INPUT_DATA);

            string fx;

            switch (index)
            {
                case '0': fx = fix_len(text, 20); break;
                case '9': fx = fix_len(text, 14); break;
                default: fx = fix_len(text, 48); break;
            }

            SendCommand(true, "{0}{1};{2}", (char)0x49, index, fx);
        }

        public void SetOperatorInfo(OperatorInfo opInfo)
        {
            string pwd = fix_len(opInfo.Password, 4);
            string cmd = string.Format("{0}{1};{2}", (char)0x4A, opInfo.Number, fix_len(opInfo.Name, 20));
            cmd += ";" + pwd;
            SendCommand(true, "{0}", cmd);
        }

        public class ArticleInfo
        {
            public int Number { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public char TaxGroup { get; set; }
            public int Subgroup { get; set; }

            // when reading
            public decimal Turnover { get; set; }
            public decimal Sells { get; set; }
            public int Counter { get; set; }
            public DateTime LastZeroing { get; set; }
            // ---

            public ArticleInfo() { }

            public ArticleInfo(int num, string name, decimal price, char taxGroup)
            {
                Number = num;
                Name = name;
                Price = price;
                TaxGroup = taxGroup;
            }
        }

        public void SetArticleInfo(ArticleInfo ai)
        {
            if (ai.Number > 99999 || ai.Number < 0)
                throw new FPException(FPLibError.BAD_INPUT_DATA);

            string dep = "";
            if (Country == "bg") dep = ";0";
            byte[] data_cmd = TextEncoding.GetBytes(string.Format("{0}{1:00000};{2};{3};{4}{5}", (char)0x4B, ai.Number, fix_len(ai.Name, 20), d2s(ai.Price), ai.TaxGroup, dep));
            if (dep.Length > 0)
                data_cmd[data_cmd.Length - 1] = (byte)(ai.Subgroup + 0x80); //i2b(ai.Subgroup);

            SendCommand(true, data_cmd);
        }

        public void SetSubgroupInfo(int num, string name, char tax_group)
        {
            // 0x47
            SendCommand(true, "G{0:00};{1};{2}", num, fix_len(name, 20), tax_group);
        }

        /// <summary>
        /// Upload a logo image
        /// </summary>
        /// <param name="logoIndex">when -1 whitout setting a number</param>
        /// <param name="data"></param>
        public void SetLogoFile(int logoIndex, byte[] data)
        {
            byte[] cmd;

            if (logoIndex == -1)
            {
                cmd = new byte[] { 0x4C };
            }
            else
            {
                cmd = new byte[] { 0x4D, (byte)('0' + logoIndex) };
            }
            byte[] logo = new byte[cmd.Length + data.Length];
            Array.Copy(cmd, 0, logo, 0, cmd.Length);
            Array.Copy(data, 0, logo, cmd.Length, data.Length);

            SendCommand(true, logo);
        }

        #endregion

        #region Data reading command

        public string GetFactoryNumber()
        {
            string[] pa = SendCommandGetArray("{0}", (char)0x60);
            if (pa.Length < 1)
                throw new FPException(FPLibError.BAD_RESPONSE);
            return pa[0];
        }

        public string GetFiscalNumber()
        {
            string[] pa = SendCommandGetArray("{0}", (char)0x60);
            if (pa.Length < 2)
                throw new FPException(FPLibError.BAD_RESPONSE);
            return pa[1];
        }

        public string GetTaxNumber()
        {
            return SendCommandGetString("{0}", (char)0x61);
        }

        public decimal[] GetTaxGroups()
        {
            string[] pa = SendCommandGetArray("b");

            if (pa.Length != TaxGroupCount)
                throw new FPException(FPLibError.BAD_RESPONSE);

            decimal[] da = new decimal[pa.Length];

            int ix = 0;
            foreach (string a in pa)
                da[ix++] = decimal.Parse(a.TrimEnd('%'), CultureInfo.InvariantCulture);

            return da;
        }

        // COMMAND: 63H – READING THE DECIMAL POINT
        public int GetDecimalPoint()
        {
            string s = SendCommandGetString("c");
            if (s.Length != 1 || !Char.IsDigit(s[0]))
                throw new FPException(FPLibError.BAD_RESPONSE);

            return Int32.Parse(s);
        }

        public class PayTypes
        {
            public PayTypes(string[] vals)
            {
                if (vals.Length != PayTypesCount + 2)
                    throw new FPException(FPLibError.BAD_INPUT_DATA);

                Names = new string[PayTypesCount];
                for (int i = 0; i < PayTypesCount; i++)
                    Names[i] = vals[i];

                CurrencyName = vals[PayTypesCount]; // zero index;
                CurrencyExchRate = decimal.Parse(vals[PayTypesCount + 1], CultureInfo.InvariantCulture);
            }

            public string[] Names { get; set; }
            public string CurrencyName { get; set; }
            public decimal CurrencyExchRate { get; set; }
        }

        public PayTypes GetPayTypes()
        {
            string[] ss = SendCommandGetArray("d");
            return new PayTypes(ss);
        }

        public class Parameters
        {
            string[] raw;

            public Parameters(string[] values)
            {
                if (values.Length < DeviceParamsCount)
                    throw new FPException(FPLibError.BAD_INPUT_DATA);
                raw = values;
            }

            public int POSNumber
            {
                get { return Int32.Parse(raw[0]); }
            }
            public bool PrintLogo
            {
                get { return raw[1][0] == '1'; }
            }
            public bool OpenCashDrawer
            {
                get { return raw[2][0] == '1'; }
            }
            public bool AutoCut
            {
                get { return raw[3][0] == '1'; }
            }
            public bool TransparentDisplay
            {
                get { return raw[4][0] == '1'; }
            }
            public bool ShortEJ
            {
                get { return raw[5][0] == '1'; }
            }
            public bool TotalInForeignCurrency
            {
                get { return raw[6][0] == '1'; }
            }
            public bool SmallFontEJ
            {
                get { return raw.Length > 7 && raw[7][0] == '1'; }
            }
            public bool FreeTextInEJ
            {
                get { return raw.Length > 8 && raw[8][0] == '1'; }
            }
            public bool SingleOperator
            {
                get { return raw.Length > 9 && raw[9][0] == '1'; }
            }
        }

        /// <summary>
        /// 0x65
        /// </summary>
        /// <returns></returns>
        public Parameters GetParameters()
        {
            string[] ss = SendCommandGetArray("e");
            return new Parameters(ss);
        }

        /// <summary>
        /// 0x68
        /// </summary>
        /// <returns></returns>
        public DateTime GetDateTime()
        {
            string s = SendCommandGetString("h");
            return s2dt(s);
        }

        public string GetHeaderLine(char index)
        {
            string s = SendCommandGetString("i{0}", index);

            if (s.Length < 16 || s[0] != index)
                throw new FPException(FPLibError.BAD_RESPONSE);

            return s.Substring(1).Trim();
        }

        public class OperatorInfo
        {
            public OperatorInfo(string[] raw)
            {
                if (raw.Length < 3)
                    throw new FPException(FPLibError.BAD_RESPONSE);
                Number = Int32.Parse(raw[0]);
                Name = raw[1].Trim();
                Password = raw[2];
            }

            public OperatorInfo(int num, string name, string pwd)
            {
                Number = num;
                Name = name;
                Password = pwd;
            }

            public int Number { get; set; }
            public string Name { get; set; }
            public string Password { get; set; }
        }

        public OperatorInfo GetOperatorInfo(int opNum)
        {
            CheckOperatorNumber(opNum);

            string[] ss = SendCommandGetArray(new int[] { -1, 20, 4 }, "j{0}", opNum);
            return new OperatorInfo(ss);
        }

        /// <summary>
        /// Print active logo
        /// </summary>
        public void PrintLogo()
        {
            SendCommand(true, "l");
        }

        public void PrintLogo(int logoNum)
        {
            if (logoNum < 0 || logoNum > 9)
                throw new FPException(FPLibError.BAD_INPUT_DATA);

            SendCommand(true, "l{0}", logoNum);
            WaitSlowCommand();
        }

        public class LogosInfo
        {
            const int MaxLogos = 10;

            string[] raw;
            public LogosInfo(string[] vals)
            {
                if (vals.Length != 2 || vals[0].Length != 1 || vals[1].Length != MaxLogos)
                    throw new FPException(FPLibError.BAD_RESPONSE);

                raw = vals;
            }

            public int ActiveLogo
            {
                get { return Int32.Parse(raw[0]); }
            }
            public bool IsLoaded(int index)
            {
                if (index >= MaxLogos)
                    throw new FPException(FPLibError.BAD_INPUT_DATA);
                return raw[1][index] == '1';
            }
        }

        public LogosInfo GetLogosInfo()
        {
            string[] ss = SendCommandGetArray("{0}?", (char)0x23);
            return new LogosInfo(ss);
        }

        #endregion

        #region Receipt operations commands

        public void OpenNonFiscalReceipt(int opNum, string password, bool defer_print)
        {
            CheckOperatorNumber(opNum);
            SendCommand(true, ".{0};{1};0" + (defer_print ? ";1" : ""), opNum, fix_len(password, 4));
            WaitSlowCommand();
        }

        public void CloseNonFiscalReceipt()
        {
            SendCommand(true, "/");
            WaitSlowCommand();
        }

        public void OpenFiscalReceipt(int opNum, string password, bool detailed, bool show_vat, bool defer_print)
        {
            CheckOperatorNumber(opNum);
            SendCommand(true, "0{0};{1};{2};{3};{4}", opNum, fix_len(password, 4), OzFlag(detailed), OzFlag(show_vat), Flag(defer_print, '2', '0'));
            WaitSlowCommand();
        }

        public void SellItem(string name, char tax_group, decimal price, decimal quantity, decimal discount, bool discount_in_percent)
        {
            if (price < -99999999M || price > 99999999M || quantity > 999999.999M || quantity < -999999.999M || (discount_in_percent && (discount < -999 || discount > 999)))
                throw new FPException(FPLibError.BAD_INPUT_DATA);

            int name_len = 36;
            string cmd = string.Format("1{0};{1};{2}", fix_len(name, name_len), tax_group, d2s(price));
            if (quantity != 1) cmd += "*" + d2s(quantity, 3);
            if (discount != 0)
            {
                if (discount_in_percent)
                    cmd += "," + d2s(discount) + "%";
                else
                    cmd += ":" + d2s(discount);
            }
            SendCommand(true, cmd);
            WaitSlowCommand();
        }

        public void SellItemDB(bool correction, int articleNum, decimal quantity, decimal discount, bool discount_in_percent)
        {
            if (articleNum < 0 || articleNum > 99999 || quantity > 999999.999M || quantity < -999999.999M || (discount_in_percent && (discount < -999 || discount > 999)))
                throw new FPException(FPLibError.BAD_INPUT_DATA);

            string cmd = string.Format("{0}{1};{2}", (char)0x32, Flag(correction, '-', '+'), articleNum);
            if (quantity != 1) cmd += "*" + d2s(quantity, 3);
            if (discount != 0)
            {
                if (discount_in_percent)
                    cmd += "," + d2s(discount) + "%";
                else
                    cmd += ":" + d2s(discount);
            }
            SendCommand(true, cmd);
            WaitSlowCommand();
        }

        /// <summary>
        /// 0x33
        /// </summary>
        public decimal Subtotal(decimal discount, bool inPercents, bool print, bool onDisplay)
        {
            string cmd = string.Format("3{0};{1}", OzFlag(print), OzFlag(onDisplay));

            if (discount != 0)
            {
                if (inPercents)
                {
                    if (discount < -99.99M || discount > 999.99M)
                        throw new FPException(FPLibError.BAD_INPUT_DATA);
                    cmd += "," + d2s(discount) + "%";
                }
                else
                {
                    if (discount < -999999999M || discount > 9999999999M)
                        throw new FPException(FPLibError.BAD_INPUT_DATA);
                    cmd += ";" + d2s(discount);
                }
            }

            string s = SendCommandGetString(cmd);

            try
            {
                return s2d(s);
            }
            catch { }

            throw new FPException(FPLibError.BAD_RESPONSE);
        }

        public void Payment(FPPaymentType type, decimal amount, bool no_change, FPChangeType change_type)
        {
            if (amount != -1 && (amount < 0 || amount > 9999999999M))
                throw new FPException(FPLibError.BAD_INPUT_DATA);
            SendCommand(true, "5{0};{1};{2};{3}", (byte)type, no_change ? 1 : 0, amount == -1 ? "\"" : d2s(amount), (byte)change_type);
            WaitSlowCommand();
        }

        public void CloseReceiptInCash()
        {
            SendCommand(true, "6");
            WaitSlowCommand();
        }

        public void PrintText(string text, FPTextAlign align)
        {
            string nt = string.Empty;

            if (align == FPTextAlign.Center)
            {
                int t = text.Length + ((TextLineWidth - text.Length) / 2);
                nt = text.PadLeft(t, ' ');
            }
            else if (align == FPTextAlign.Right)
            {
                nt = text.PadLeft(TextLineWidth, ' ');
            }
            else if (align == FPTextAlign.Left)
            {
                nt = text;
            }

            PrintText(nt);
        }

        public void PrintText(string text)
        {
            if (text.Length > TextLineWidth)
                throw new FPException(FPLibError.BAD_INPUT_DATA);

            SendCommand(true, "7{0}", fix_len(text, TextLineWidth));
            WaitSlowCommand();
        }

        public void CloseFiscalReceipt()
        {
            SendCommand(true, "8");
            WaitSlowCommand();
        }

        public void PrintDuplicate()
        {
            SendCommand(true, ":");
            WaitSlowCommand();
        }

        /// <summary>
        /// 3Bh
        /// </summary>
        public void OfficialSums(int opNum, string pwd, char payType, decimal amount, string expl)
        {
            CheckOperatorNumber(opNum);
            SendCommand(true, ";{0};{1};{2};{3};{4};@{5}", opNum, fix_len(pwd, 4), payType, d2s(amount), fix_len(expl, 38));
        }

        /// <summary>
        /// Not supported in all devices.
        /// Use TerminateReceipt.
        /// </summary>
        public void CancelFiscalReceipt()
        {
            SendCommand(true, "9");
            WaitSlowCommand();
        }

        /// <summary>
        /// Terminates open fiscal and non-fiscal receipts.
        /// Use this method only for service purposes - after unexpected errors after confirmation from the user or in special menu.
        /// Do not use TerminateReceipt for regular fiscal receipt closures.
        /// </summary>
        /// <param name="FinishPayment">When false the receipt balance will be cancelled.</param>
        public void TerminateReceipt(bool FinishPayment)
        {
            Status s = GetStatus();

            if (s.OpenNonFiscalReceipt)
            {
                CloseNonFiscalReceipt();
                return;
            }
            if (s.OpenFiscalReceipt)
            {
                decimal pay = 0;

                if (FinishPayment)
                {
                    try
                    {
                        CloseReceiptInCash();
                        return;
                    }
                    catch { } // assume command is not supported
                    pay = Subtotal(0, true, false, false);
                }
                else
                {
                    try
                    {
                        CancelFiscalReceipt();
                        return;
                    }
                    catch { }
                    FiscalReceiptInfo fri = GetFiscalReceiptInfo();

                    if (!fri.IsOpen)
                        return;

                    decimal[] tgsts = fri.TaxGroupSubtotal;

                    for (int i = 0; i < tgsts.Length; i++)
                    {
                        if (tgsts[i] != 0)
                        {
                            SellItem("[void]", (char)(TaxGroupBase + i), -tgsts[i], 1, 0, false);
                        }
                    }
                }

                if (pay == 0) pay = 0.01M;
                Payment(FPPaymentType.Cash, pay, true, FPChangeType.SameAsPayment);
                CloseFiscalReceipt();
            }
        }

        #endregion

        #region Commands for reading the data from FPR's registers

        public ArticleInfo GetArticleInfo(int num)
        {
            string[] ss = SendCommandGetArray(new int[] { -1, 20 }, "{0}{1}", (char)0x6B, num.ToString("00000"));

            if (ss.Length < 8)
                throw new FPException(FPLibError.BAD_RESPONSE);

            try
            {
                int ix = 0;
                ArticleInfo ai = new ArticleInfo();
                ai.Number = Int32.Parse(ss[ix++]);
                ai.Name = ss[ix++].Trim();
                ai.Price = s2d(ss[ix++]);
                ai.TaxGroup = ss[ix++][0];
                ai.Turnover = s2d(ss[ix++]);
                ai.Sells = s2d(ss[ix++]);
                ai.Counter = Int32.Parse(ss[ix++]);
                ai.LastZeroing = s2dt(ss[ix++]);

                if (ss.Length > 8 && last_response_raw.Length > 89)
                    ai.Subgroup = b2i(last_response_raw[89]);

                return ai;
            }
            catch (Exception)
            {
            }

            throw new FPException(FPLibError.BAD_RESPONSE);
        }

        public ArticleInfo GetSubgroupInfo(int num)
        {
            string[] ss = SendCommandGetArray(new int[] { -1, 20 }, "{0}{1}", (char)0x67, num.ToString("00"));

            if (ss.Length >= 5)
            {
                try
                {
                    ArticleInfo ai = new ArticleInfo();

                    ai.Name = ss[1].TrimEnd(' ');
                    ai.TaxGroup = ss[2][0];
                    ai.Turnover = s2d(ss[3]);
                    ai.Sells = s2d(ss[4]);

                    return ai;
                }
                catch
                {
                }
            }
            throw new FPException(FPLibError.BAD_RESPONSE);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public decimal[] GetAmountsByTaxGroup()
        {
            string[] ss = SendCommandGetArray("{0}", (char)0x6D);
            if (ss.Length != TaxGroupCount)
                throw new FPException(FPLibError.BAD_RESPONSE);
            return sa2da(ss, 0, TaxGroupCount);
        }

        /// <summary>
        /// Use for:
        /// 2.7.3. COMMAND: 6ЕH / N – READING OF REGISTERS – 0 (ON HAND)
        /// 2.7.7. COMMAND: 6ЕH / N – READING OF REGISTERS – 4 (RECEIVED)
        /// </summary>
        /// <param name="type">'0' or '4'</param>
        /// <returns>the amounts</returns>
        public decimal[] GetAmounts(char type)
        {
            string[] ss = SendCommandGetArray("{0}{1}", (char)0x6E, type);

            if (ss.Length != 6 || ss[0][0] != type)
                throw new FPException(FPLibError.BAD_RESPONSE);

            return sa2da(ss, 1, 5);
        }

        /// <summary>
        /// Use for:
        /// 2.7.5. COMMAND: 6ЕH / N – READING OF REGISTERS – 2 (RA)
        /// 2.7.6. COMMAND: 6ЕH / N – READING OF REGISTERS – 3 (PO)
        /// </summary>
        /// <param name="type">'2' or '3'</param>
        /// <param name="numOps">the returned value of number of operations</param>
        /// <returns>the amounts</returns>
        public decimal[] GetAmountsAndNumOps(char type, out int numOps)
        {
            string[] ss = SendCommandGetArray("{0}{1}", (char)0x6E, type);

            if (ss.Length != 7 || ss[0][0] != type)
                throw new FPException(FPLibError.BAD_RESPONSE);

            numOps = Int32.Parse(ss[6]);
            return sa2da(ss, 1, 5);
        }

        public class RegistersInfo
        {
            public int NumCustomers { get; set; }
            public int NumDiscounts { get; set; }
            public decimal DiscountsTotal { get; set; }
            public int NumAdditions { get; set; }
            public decimal AdditionsTotal { get; set; }
            public int NumVoids { get; set; }
            public decimal VoidsTotal { get; set; }

            public RegistersInfo(string[] raw, int ix)
            {
                if (raw.Length < ix + 7)
                    throw new FPException(FPLibError.BAD_RESPONSE);

                NumCustomers = Int32.Parse(raw[ix++]);
                NumDiscounts = Int32.Parse(raw[ix++]);
                DiscountsTotal = s2d(raw[ix++]);
                NumAdditions = Int32.Parse(raw[ix++]);
                AdditionsTotal = s2d(raw[ix++]);
                NumVoids = Int32.Parse(raw[ix++]);
                VoidsTotal = s2d(raw[ix++]);
            }
        }

        /// <summary>
        /// Provides information about the number of customers (number of fiscal
        /// receipt issued), number of discounts, additions and corrections
        /// made and the accumulated amounts.
        /// </summary>
        /// <returns></returns>
        public RegistersInfo GetRegistersInfo()
        {
            string[] ss = SendCommandGetArray("{0}1", (char)0x6E);
            if (ss.Length > 1 && ss[0] == "1")
                return new RegistersInfo(ss, 1);
            else
                throw new FPException(FPLibError.BAD_RESPONSE);
        }

        public class DailyReportInfo
        {
            public int LastReport { get; set; }
            public int LastFiscMemBlock { get; set; }
            public int NumEJ { get; set; }
            public DateTime LastSaveTime { get; set; }

            public DailyReportInfo(string[] raw)
            {
                if (raw.Length < 5 || raw[0] != "5")
                    throw new FPException(FPLibError.BAD_RESPONSE);

                int ix = 1;
                LastReport = Int32.Parse(raw[ix++]);
                LastFiscMemBlock = Int32.Parse(raw[ix++]);
                NumEJ = Int32.Parse(raw[ix++]);
                LastSaveTime = s2dt(raw[ix++]);
            }
        }

        public DailyReportInfo GetDailyReportInfo()
        {
            string[] ss = SendCommandGetArray("{0}5", (char)0x6E);
            return new DailyReportInfo(ss);
        }

        public class OperatorReportInfo
        {
            public int OpNum { get; set; }
            public RegistersInfo RInfo { get; set; }

            public OperatorReportInfo(string[] raw)
            {
                OpNum = Int32.Parse(raw[1]);
                RInfo = new RegistersInfo(raw, 2);
            }
        }

        public OperatorReportInfo GetOperatorReportInfo(int opNum)
        {
            CheckOperatorNumber(opNum);
            string[] ss = SendCommandGetArray("{0}1;{1}", (char)0x6F, opNum);

            if (ss.Length != 9 || ss[0] != "1")
                throw new FPException(FPLibError.BAD_RESPONSE);
            return new OperatorReportInfo(ss);
        }

        public decimal[] GetOperatorAmountsAndNumOps(int opNum, char type, out int numOps)
        {
            CheckOperatorNumber(opNum);
            string[] ss = SendCommandGetArray("{0}{1};{2}", (char)0x6F, type, opNum);

            if (ss.Length != 8 || ss[0][0] != type)
                throw new FPException(FPLibError.BAD_RESPONSE);
            numOps = Int32.Parse(ss[7]);
            return sa2da(ss, 2, 5);
        }

        public decimal[] GetOperatorAmounts(int opNum, char type)
        {
            CheckOperatorNumber(opNum);
            string[] ss = SendCommandGetArray("{0}{1};{2}", (char)0x6F, type, opNum);

            if (ss.Length != 7 || ss[0][0] != type)
                throw new FPException(FPLibError.BAD_RESPONSE);
            return sa2da(ss, 2, 5);
        }

        public void GetOperatorLastReport(int opNum, out int num, out DateTime time)
        {
            string[] ss = SendCommandGetArray("{0}5;{1}", (char)0x6F, opNum);

            if (ss.Length < 4 || ss[0][0] != '5')
                throw new FPException(FPLibError.BAD_RESPONSE);

            try
            {
                num = Int32.Parse(ss[2]);
                time = s2dt(ss[3]);
                return;
            }
            catch { }

            throw new FPException(FPLibError.BAD_RESPONSE);
        }

        public int GetLastReceiptNumber()
        {
            string s = SendCommandGetString("{0}", (char)0x71);
            try
            {
                return Int32.Parse(s);
            }
            catch { }

            throw new FPException(FPLibError.BAD_RESPONSE);
        }

        public FiscalReceiptInfo GetFiscalReceiptInfo()
        {
            string[] ss = SendCommandGetArray("r");
            return new FiscalReceiptInfo(ss);
        }

        public class LastDailyReportInfo
        {
            public DateTime LastDate { get; set; }
            public int LastDailyReport { get; set; }
            public int LastRamReset { get; set; }

            public LastDailyReportInfo(string[] raw)
            {
                if (raw.Length < 3)
                    throw new FPException(FPLibError.BAD_RESPONSE);

                int ix = 0;
                LastDate = s2dt(raw[ix++]);
                LastDailyReport = Int32.Parse(raw[ix++]);
                LastRamReset = Int32.Parse(raw[ix++]);
            }
        }

        public LastDailyReportInfo GetLastDailyReportInfo()
        {
            string[] ss = SendCommandGetArray("{0}", (char)0x73);
            return new LastDailyReportInfo(ss);
        }

        public int GetFreeFiscMemBlocks()
        {
            string s = SendCommandGetString("{0}", (char)0x74);
            try
            {
                return Int32.Parse(s);
            }
            catch { }

            throw new FPException(FPLibError.BAD_RESPONSE);
        }

        public byte[] ReadFiscalMemory()
        {
            List<byte> re = new List<byte>();

            SendCommand(false, new byte[] { 0x75 });

            byte b= new byte();
            do
            {
                try
                {
                    b = (byte)port.ReadByte();
                }
                catch (TimeoutException ex) { throw new FPException(FPLibError.TIMEOUT, ex); }
                catch (Exception ex) { throw new FPException(FPLibError.UNDEFINED, ex); }

                re.Add(b);

            } while (b != (byte)'@');

            return re.ToArray();
        }

        #endregion

        #region Reports printing commands

        public void ReportSpecialFiscal()
        {
            SendCommand(true, "w");
            WaitSlowCommand();
        }

        public void ReportFiscalByBlock(bool detailed, bool payments, int startBlock, int endBlock)
        {
            if (startBlock < 0 || startBlock > 9999 || endBlock < startBlock || endBlock > 9999)
                throw new FPException(FPLibError.BAD_INPUT_DATA);

            SendCommand(true, "{2}{0};{1}{3}", startBlock.ToString("D4"), endBlock.ToString("D4"), detailed ? "x" : "y", payments ? ";P" : "");

            WaitSlowCommand();
        }

        public void ReportFiscalByBlock(bool detailed, int startBlock, int endBlock)
        {
            ReportFiscalByBlock(detailed, startBlock, endBlock);
        }

        public void ReportFiscalByDate(bool detailed, bool payments, DateTime start, DateTime end)
        {
            if (Int32.Parse(start.ToString("yyyyMMdd")) > Int32.Parse(end.ToString("yyyyMMdd")))
                throw new FPException(FPLibError.BAD_INPUT_DATA);

            string startStr = start.ToString("ddMMyy");
            string endStr = end.ToString("ddMMyy");

            SendCommand(true, "{2}{0};{1}{3}", startStr, endStr, detailed ? "z" : "{", payments ? ";P" : "");

            WaitSlowCommand();
        }

        public void ReportFiscalByDate(bool detailed, DateTime start, DateTime end)
        {
            ReportFiscalByDate(detailed, false, start, end);
        }

        public void ReportOperator(bool zero, int opNum)
        {
            if (opNum != 0)
                CheckOperatorNumber(opNum);

            SendCommand(true, "}}{0};{1}", Flag(zero, 'Z', 'X'), opNum);
            WaitSlowCommand();
        }

        public void ReportArticles(bool zero)
        {
            SendCommand(true, "~{0}", zero ? 'Z' : 'X');
            WaitSlowCommand();
        }

        public void ReportSubgroup(bool zero)
        {
            SendCommand(true, zero ? "vZ" : "vX");
            WaitSlowCommand();
        }

        public void ReportDaily(bool zero, bool extended)
        {
            SendCommand(true, "{0}{1}", extended ? (char)0x7F : '|', zero ? 'Z' : 'X');
            WaitSlowCommand();
        }

        public void ReportEJ()
        {
            SendCommand(true, "{0}E", (char)0x7C);
            WaitSlowCommand();
        }

        #endregion

        #region Auxiliary commands

        public class PrinterModuleStatus
        {
            byte[] raw;

            public PrinterModuleStatus(byte[] raw)
            {
                if (raw.Length < 4)
                    throw new FPException(FPLibError.BAD_RESPONSE);

                this.raw = raw;
            }

            private bool IsHigh(int byteIndex, int bitIndex)
            {
                return (raw[byteIndex] & (1 << bitIndex)) != 0;
            }

            public bool DrawerSignalLevel
            {
                get { return IsHigh(1, 2); }
            }
            public bool Offline
            {
                get { return IsHigh(1, 3); }
            }
            public bool CoverOpen
            {
                get { return IsHigh(1, 5); }
            }
            public bool PaperFeedStatus
            {
                get { return IsHigh(1, 6); }
            }
            public bool AutoCutterError
            {
                get { return IsHigh(2, 3); }
            }
            public bool FatalError
            {
                get { return IsHigh(2, 5); }
            }
            public bool PaperNearEnd
            {
                get { return IsHigh(3, 1); }
            }
            public bool OutOfPaper
            {
                get { return IsHigh(3, 3); }
            }
            public bool LineDisplay
            {
                get { return raw[0] == 'Y'; }
            }
            public bool ServiceJumper
            {
                get { return raw.Length >= 6 && raw[5] == 'J'; }
            }
        }

        public PrinterModuleStatus GetPrinterModuleStatus()
        {
            byte[] r = SendCommand(true, "f");

            if (r.Length < 5)
                throw new FPException(FPLibError.BAD_RESPONSE);

            byte[] daon = new byte[r.Length - 1];
            Array.Copy(r, 1, daon, 0, daon.Length);
            if (daon.Length >= 6 && daon[1] < 128)
                daon[1] = daon[2] = daon[3] = daon[4] = 0;
            return new PrinterModuleStatus(daon);
        }

        public void SetPrintBarcodeInReceipt(bool enable)
        {
            SendCommand(true, "{0}{1}", (char)0x51, Flag(enable, 'E', 'D'));
        }

        public void SetBarcodeFormat(string format)
        {
            if (format.Length != 12)
                throw new FPException(FPLibError.BAD_RESPONSE);

            SendCommand(true, "{0}F;{1}", (char)0x51, format);
        }

        public void PrintBarcode(char codeType, int codeLen, string data)
        {
            if (data.Length > 255)
                throw new FPException(FPLibError.BAD_RESPONSE);

            SendCommand(true, "{0}P;{1};{2};{3}", (char)0x51, codeType, codeLen, data);
        }

        #endregion

        #endregion

        #region GS commands


        public string GSSendCommandGetString(string cmd)
        {
            byte[] r = GSSendCommand(cmd);
            return TextEncoding.GetString(r, 0, r.Length);
        }

        /// <summary>
        /// Read max values for device database categories.
        /// </summary>
        public int[] GetDatabaseRange()
        {
            int save = GSEndByte;
            GSEndByte = 10;
            try
            {
                string[] r = GSSendCommandGetString("I").Substring(1).Split(';');
                int[] rr = new int[r.Length];
                for (int i = 0; i < r.Length; i++)
                {
                    try
                    {
                        rr[i] = Int32.Parse(r[i], CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        rr[i] = 0;
                    }
                }
                return rr;
            }
            catch
            {
            }
            finally
            {
                GSEndByte = save;
            }
            return null;
        }

        public byte[] GSSendCommand(string cmd)
        {
            Ping(BUSY, FPLibError.PRINTER_BUSY, ping_retries, false);

            byte[] data = TextEncoding.GetBytes(cmd);
            try
            {
                port.DiscardInBuffer();
                port.Write(new byte[] { 0x1D }, 0, 1);
                port.Write(data, 0, data.Length);
            }
            catch (TimeoutException tex)
            {
                throw new FPException(FPLibError.TIMEOUT, tex);
            }
            catch (Exception ex)
            {
                throw new FPException(FPLibError.UNDEFINED, ex);
            }

            List<byte> rep = new List<byte>();
            int save_timeout = port.ReadTimeout;

            while (true)
            {
                try
                {
                    byte b = (byte)port.ReadByte();
                    if (GSEndByte != -1 && b == (byte)GSEndByte)
                        break;
                    rep.Add(b);
                    if (rep.Count == 1)
                    {
                        port.ReadTimeout = gs_timeout;
                    }
                }
                catch (TimeoutException)
                {
                    break; // end on timeout
                }
                catch (Exception ex)
                {
                    throw new FPException(FPLibError.UNDEFINED, ex);
                }
                finally
                {
                    port.ReadTimeout = save_timeout;
                }
            }

            return rep.ToArray();
        }

        public string GSGetVersion()
        {
            byte[] bu = GSSendCommand("?");
            return TextEncoding.GetString(bu, 3, bu.Length - 3);
        }

        /// <summary>
        /// Enable/Disable communication with the specified device
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="devNo"></param>
        public void GSCommunicationControl(bool enable, int devNo)
        {
            string ack = GSSendCommandGetString(string.Format("={0}F{1}", OzFlag(!enable), devNo.ToString().PadLeft(4, '0')));

            if (ack != "ACK")
                throw new FPException(FPLibError.BAD_RESPONSE);
        }

        public void GSSetSpeed(int speedStep)
        {
            if (speedStep > 4)
                throw new FPException(FPLibError.BAD_RESPONSE);

            GSSendCommand("S" + speedStep);
        }
        #endregion

        #region System Members

        /// <summary>
        /// Send raw command to the FP.
        /// </summary>
        /// <param name="get_reply">Read command reply or receipt.</param>
        /// <param name="format">String format of the command and parameters.</param>
        /// <param name="arg0">Optional list of one or more parameters.</param>
        /// <returns>If get_reply parameter is true, contains a byte array of return values.</returns>
        public byte[] SendCommand(bool get_reply, string format, params object[] arg0)
        {
            byte[] data_cmd = TextEncoding.GetBytes(string.Format(format, arg0));

            return SendCommand(get_reply, data_cmd);
        }

        public byte[] SendCommand(bool get_reply, byte[] data_cmd)
        {
            int data_len = data_cmd.Length + 2;
            if (data_len > 127)
                throw new FPException(FPLibError.BAD_INPUT_DATA);
            byte[] data = new byte[data_cmd.Length + 6];
            cmd_id = (byte)(((++next_cmd_id) % 0x7F) + 0x20);
            data[0] = STX;
            data[1] = (byte)(data_len + 0x20);
            data[2] = cmd_id;
            Array.Copy(data_cmd, 0, data, 3, data_cmd.Length);
            MakeCRC(data, 1, data_len);
            data[data.Length - 1] = ETX;
            last_response_raw = null;

            Ping(BUSY, FPLibError.PRINTER_BUSY, ping_retries, true);
            try
            {
                port.DiscardInBuffer();
                port.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(TimeoutException))
                    throw new FPException(FPLibError.TIMEOUT, ex);
                else
                    throw new FPException(FPLibError.UNDEFINED, ex);
            }
            if (get_reply)
            {
                last_response_raw = GetResponse(data);
                return last_response_raw;
            }

            return null;
        }

        public string[] SendCommandGetArray(int[] fixed_size, string format, params object[] arg0)
        {
            string str = SendCommandGetString(format, arg0);
            List<string> res = new List<string>();

            for (int i = 0; i < fixed_size.Length; i++)
            {
                if (str.Length >= fixed_size[i])
                {
                    int j = fixed_size[i];

                    if (j == -1) // to next ;
                    {
                        j = str.IndexOf(';');
                        if (j == -1) j = str.Length;
                    }
                    res.Add(str.Substring(0, j));
                    str = str.Substring(j);
                    if (str.Length > 0 && str[0] == ';')
                        str = str.Substring(1);
                }
            }
            if (str.Length > 0)
            {
                foreach (string last in str.Split(';'))
                    res.Add(last.Trim());
            }

            return (string[])res.ToArray();
        }

        public string[] SendCommandGetArray(string format, params object[] arg0)
        {
            string s = SendCommandGetString(format, arg0);
            if (s == null) return new string[0];
            List<string> re = new List<string>();
            foreach (string o in s.Split(';'))
            {
                string d = o.Trim(' ', ';');
                re.Add(d);
            }
            return re.ToArray();
        }

        public string SendCommandGetString(string format, params object[] arg0)
        {
            byte[] r = SendCommand(true, format, arg0);
            if (r == null) return "";
            string s = TextEncoding.GetString(r, 0, r.Length);
            if (s.Length > 1)
                return s.Substring(1).Trim(';');
            throw new FPException(FPLibError.BAD_RESPONSE);
        }

        public byte[] GetResponse(byte[] cmd)
        {
            List<byte> res = new List<byte>();
            int r = 0, retry = cmd_retries;

            do
            {
                try
                {
                    r = port.ReadByte();
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(TimeoutException))
                        throw new FPException(FPLibError.TIMEOUT, ex);
                    else
                        throw new FPException(FPLibError.UNDEFINED, ex);
                }
                if (res.Count == 0)
                {
                    if (r == ANTIECHO)
                        throw new FPException(FPLibError.UNKNOWN_DEVICE);
                    if (r == NACK)
                    {
                        if (retry > 0)
                        {
                            retry--;
                            continue;
                        }
                        throw new FPException(FPLibError.NACK);
                    }
                    if (r == RETRY)
                    {
                        if (retry > 0 && cmd != null)
                        {
                            retry--;
                            res.Clear();
                            Thread.Sleep(50);
                            port.Write(cmd, 0, cmd.Length);
                            continue;
                        }
                        throw new FPException(FPLibError.RETRIED);
                    }
                }
                else
                {
                    if (res.Count == 1 && (byte)res[0] == STX)
                    {
                        int len = (r - 0x20) & 0x7F;
                        if (len > 1)
                        {
                            byte[] msg = new byte[len - 1];
                            int read = port.Read(msg, 0, msg.Length);

                            res.Add((byte)r);
                            for (int i = 0; i < read; i++)
                            {
                                res.Add(msg[i]);
                            }
                            continue;
                        }
                    }
                    if (r == ETX)
                    {
                        if (res.Count < 5)
                            throw new FPException(FPLibError.BAD_RESPONSE);
                        byte[] rcp = (byte[])res.ToArray();
                        byte c1, c2;
                        c1 = rcp[rcp.Length - 2];
                        c2 = rcp[rcp.Length - 1];
                        MakeCRC(rcp, 1, rcp.Length - 3);
                        if (rcp[rcp.Length - 2] != c1 || rcp[rcp.Length - 1] != c2)
                            throw new FPException(FPLibError.CRC);
                        if (rcp[0] == ACK)
                        {
                            byte s1 = rcp[rcp.Length - 4];
                            byte s2 = rcp[rcp.Length - 3];
                            byte ste = (byte)(((s1 <= (byte)'9' ? s1 : s1 - 55) << 4) | ((s2 <= (byte)'9' ? s2 : s2 - 55) & 15));
                            if (ste != 0)
                                throw new FPException(ste);

                            return null;
                        }
                        if (rcp[2] != cmd_id)
                            throw new FPException(FPLibError.NBL_NOT_SAME);
                        if (rcp[1] - 0x20 != rcp.Length - 3)
                            throw new FPException(FPLibError.BAD_RESPONSE);
                        rcp = new byte[res.Count - 5];
                        res.CopyTo(3, rcp, 0, rcp.Length);

                        return rcp;
                    }
                }

                res.Add((byte)r);
                if (res.Count > 6 && (byte)res[0] == ACK)
                    throw new FPException(FPLibError.BAD_RECEIPT);
                if (res.Count > 255)
                    throw new FPException(FPLibError.BAD_RESPONSE);
            }
            while (true);
        }

        private FPLibError Ping(byte ping_code, FPLibError res_default, int retries, bool throw_exception)
        {
            if (port == null)
            {
                if (throw_exception) throw new FPException(FPLibError.PORT_NOT_OPEN);

                return FPLibError.PORT_NOT_OPEN;
            }
            else
            {
                byte anti = ANTIECHO;
                int save_timeout = port.ReadTimeout;

                byte[] data;

                if (new_ping == true)
                {
                    data = new byte[1];
                    data[0] = PING_NEW;
                    anti = PING_NEW;
                }
                else
                {
                    data = new byte[2];

                    data[0] = ANTIECHO;
                    data[1] = ping_code;

                    if (new_ping == null)
                    {
                        data[0] = PING_NEW;
                    }
                }
                 port.DiscardInBuffer();
                for (int i = 0; i < retries; i++)
                {
                    int r = 0;

                    port.Write(data, 0, data.Length);
                    port.ReadTimeout = ping_timeout;
                    try
                    {
                        r = port.ReadByte();
                        if (new_ping == null)
                        {
                            if ((r & 0xC0) == 0x40)
                            {
                                new_ping = true;
                                r = port.ReadByte();
                            }
                            else
                            {
                                new_ping = false;
                            }
                        }
                    }
                    catch
                    {
                    }
                    port.ReadTimeout = save_timeout;
                    if (r == ping_code)
                    {
                        return FPLibError.SUCCESS;
                    }
                    if (new_ping == true && (r & 0xC0) == 0x40)
                    {
                        if ((r & 1) != 0 && ping_code == BUSY)
                        {
                            res_default = FPLibError.PRINTER_BUSY;
                        }
                        else
                            if ((r & 2) != 0)
                            {
                                res_default = FPLibError.PAPER_EMPTY;
                            }
                            else
                            {
                                return FPLibError.SUCCESS;
                            }
                    }
                    if (r == OUTOFPAPER)
                        res_default = FPLibError.PAPER_EMPTY;
                    if (r == anti)
                    {
                        if (throw_exception) throw new FPException(FPLibError.UNKNOWN_DEVICE);

                        return FPLibError.UNKNOWN_DEVICE;
                    }
                }
                if (throw_exception) throw new FPException(res_default);

                return res_default;
            }
        }

        private void MakeCRC(byte[] data, int start, int len)
        {
            byte crc = 0;

            for (int i = 0; i < len; i++)
            {
                crc ^= data[start + i];
            }
            data[start + len] = (byte)((crc >> 4) | '0');
            data[start + len + 1] = (byte)((crc & 15) | '0');
        }

        /// <summary>
        /// Waits until the the last command is finished.
        /// </summary>
        /// <param name="timeout">Timeout in milliseconds. Infinite timeout = -1.</param>
        /// <param name="throw_exception">True to throw BUSY_TIMEOUT exception, otherwise the return value is true.</param>
        /// <returns>True when timeout is expired and throw_exception=false</returns>
        public bool WaitIfBusy(int timeout, bool throw_exception)
        {
            DateTime limit = DateTime.Now.AddMilliseconds(timeout);

            do
            {
                if (IsPrinterBusy == false) return false;
                Thread.Sleep(100);
            }
            while (timeout < 0 || DateTime.Now.CompareTo(limit) < 0);

            if (throw_exception)
                throw new FPException(FPLibError.BUSY_TIMEOUT);

            return true;
        }

        public bool IsPrinterBusy
        {
            get
            {
                return Ping(BUSY, FPLibError.PRINTER_BUSY, ping_retries, false) == FPLibError.PRINTER_BUSY;
            }
        }

        public void OpenPort(string port_name, int baud_rate, int num_ping_retries)
        {
            ClosePort();
            port = new SerialPort(port_name, baud_rate);
            port.ReadTimeout = rw_timeout;
            port.WriteTimeout = rw_timeout;
            port.Open();
            if (port != null)
            {
                Ping(PING, FPLibError.NO_PRINTER, ping_retries, true);
            }
            else
            {
                throw new FPException(FPLibError.NO_PRINTER);
            }
        }

        public void OpenPort(SerialPort p, int num_ping_retries)
        {
            port = p;
            port.ReadTimeout = rw_timeout;
            port.WriteTimeout = rw_timeout;

            Ping(PING, FPLibError.NO_PRINTER, num_ping_retries, true);
        }

        public void OpenPort(string port_name, int baud_rate)
        {
            OpenPort(port_name, baud_rate, ping_retries);
        }

        public void ClosePort()
        {
            if (port != null)
            {
                if (port.IsOpen)
                    port.Close();
                port = null;
            }
            new_ping = null;
        }

        public FP()
        {
            Country = "ke";
        }

        public void Dispose()
        {
            ClosePort();
            if (port != null) port.Close();
        }

        private byte cmd_id;
        public SerialPort port;

        /// <summary>
        /// Current version of Tremol.FP class
        /// </summary>
        public const double LibraryVersion = 2.6;

        #region Helpers

        private static string fix_len(string text, int len, char padding)
        {
            return text.PadRight(len, padding).Substring(0, len);
        }

        private static string fix_len(string text, int len)
        {
            return fix_len(text, len, ' ');
        }

        private static DateTime s2dt(string str)
        {
            DateTime res;

            res = DateTime.ParseExact(str, new string[] { "dd-MM-yyyy", "dd-MM-yyyy HH:mm" }, CultureInfo.InvariantCulture, DateTimeStyles.AllowTrailingWhite);

            return res;
        }

        private static string d2s(decimal value, int decimals)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}", decimal.Round(value, decimals));
        }

        private static string d2s(decimal value)
        {
            return d2s(value, 2);
        }

        private static decimal s2d(string str)
        {
            return decimal.Parse(str.Trim(' ', ';'), CultureInfo.InvariantCulture);
        }

        private int b2i(byte val)
        {
            if (val < 0x30) return val;
            if (val < 0x80) return val - 0x30;

            return val - 0x80;
        }

        private byte i2b(int val)
        {
            if (val < 10) return (byte)(val + 0x30);

            return (byte)(val + 0x80);
        }

        private static decimal[] sa2da(string[] ss, int startIndex, int count)
        {
            if (ss.Length < startIndex + count)
                throw new FPException(FPLibError.BAD_RESPONSE);

            decimal[] re = new decimal[count];

            for (int i = 0; i < count; i++)
                re[i] = s2d(ss[startIndex + i]);

            return re;
        }

        private static void CheckOperatorNumber(int opNum)
        {
            if (opNum < 1 || opNum > 99)
                throw new FPException(FPLibError.BAD_INPUT_DATA);
        }

        private static char Flag(bool val, char forTrue, char forFalse)
        {
            return (val ? forTrue : forFalse);
        }

        private static char OzFlag(bool val)
        {
            return Flag(val, '1', '0');
        }

        private void WaitSlowCommand()
        {
            if (WaitAfterSlowCommands)
                WaitIfBusy(busy_timeout, false);
        }

        #endregion

        #endregion

        #region FP Commands return values (structures)
        /// <summary>
        /// Current status of the Fiscal Printer.
        /// Raw data is supplied after GetStatus command (0x20 in the communication protocol).
        /// </summary>
        public class Status
        {
            /// <summary>
            /// Raw status data received from the FP
            /// </summary>
            public byte[] raw_data;

            public void Load(byte[] data)
            {
                if (data == null || data.Length < 6)
                    throw new FPException(FPLibError.BAD_RESPONSE);
                raw_data = new byte[data.Length - 1];
                Array.Copy(data, 1, raw_data, 0, raw_data.Length);
            }

            public Status()
            {
                raw_data = new byte[5];
            }

            public Status(byte[] data)
            {
                Load(data);
            }

            private bool IsHigh(int byteIndex, int bitIndex)
            {
                return (raw_data[byteIndex] & (1 << bitIndex)) != 0;
            }

            public bool ReadOnlyFiscalMemory { get { return IsHigh(0, 0); } }
            public bool PowerDownWhileFiscalReceiptOpen { get { return IsHigh(0, 1); } }
            public bool PrinterOverheat { get { return IsHigh(0, 2); } }
            public bool IncorectClock { get { return IsHigh(0, 3); } }
            public bool IncorectDate { get { return IsHigh(0, 4); } }
            public bool RAMError { get { return IsHigh(0, 5); } }
            public bool ClockFailure { get { return IsHigh(0, 6); } }

            public bool PaperOut { get { return IsHigh(1, 0); } }
            public bool ReportsAccumulationOverflow { get { return IsHigh(1, 1); } }

            public bool NonZeroDailyReport { get { return IsHigh(1, 3); } }
            public bool NonZeroArticleReport { get { return IsHigh(1, 4); } }
            public bool NonZeroOperatorReport { get { return IsHigh(1, 5); } }
            public bool NonPrintedCopy { get { return IsHigh(1, 6); } }

            public bool OpenNonFiscalReceipt { get { return IsHigh(2, 0); } }
            public bool OpenFiscalReceipt { get { return IsHigh(2, 1); } }
            public bool StandardCashReceipt { get { return IsHigh(2, 2); } }
            public bool VATIncludedInReceipt { get { return IsHigh(2, 3); } }

            public bool NoFiscalMemory { get { return IsHigh(3, 0); } }
            public bool FiscalMemoryFailure { get { return IsHigh(3, 1); } }
            public bool FiscalMemoryOverflow { get { return IsHigh(3, 2); } }
            public bool FiscalMemoryAlmostFull { get { return IsHigh(3, 3); } }
            public bool FractionAmmounts { get { return IsHigh(3, 4); } }
            public bool Fiscalized { get { return IsHigh(3, 5); } }
            public bool FactoryNumberSet { get { return IsHigh(3, 6); } }

            public bool AutoCut { get { return IsHigh(4, 0); } }
            public bool TransparentDisplay { get { return IsHigh(4, 1); } }
            public int CommunicationSpeed
            {
                get
                {
                    if (IsHigh(4, 2))
                        return 9600;
                    else
                        return 19200;
                }
            }
            public bool AutoOpenCashDrawer { get { return IsHigh(4, 4); } }
            public bool IncludeLogoInReceipt { get { return IsHigh(4, 5); } }
        }

        /// <summary>
        /// Information for the current fiscal receipt (if open).
        /// </summary>
        public class FiscalReceiptInfo
        {
            public FiscalReceiptInfo(string[] raw_str)
            {
                if (raw_str.Length > 1)
                {
                    int ix = 0;

                    IsOpen = raw_str[ix++] == "1";
                    TaxGroupSubtotal = new decimal[TaxGroupCount];

                    if (!IsOpen)
                        return;

                    int nvs = raw_str.Length - 1;

                    int numGrInFirstBatch;

                    switch (nvs)
                    {
                        case 18:
                        case 11:
                            numGrInFirstBatch = 3;
                            break;
                        case 16:
                        case 15:
                            numGrInFirstBatch = 5;
                            break;
                        default:
                            throw new FPException(FPLibError.BAD_RESPONSE);
                    }

                    SalesCount = Int32.Parse(raw_str[ix++]);

                    int tgx = 0; // tax group index

                    for (; tgx < numGrInFirstBatch; tgx++)
                        TaxGroupSubtotal[tgx] = s2d(raw_str[ix++]);

                    ForbiddenVoid = raw_str[ix++] == "1";
                    PrintVAT = raw_str[ix++] == "1";
                    PrintDetailedReceipt = raw_str[ix++] == "1";
                    PaymentInitiated = raw_str[ix++] == "1";
                    PaymentCompleted = raw_str[ix++] == "1";
                    PowerDown = raw_str[ix++] == "1";
                    IsInvoice = raw_str[ix++] == "1";
                    Change = s2d(raw_str[ix++]);
                    ChangeType = (FPPaymentType)Int32.Parse(raw_str[ix++]);

                    while (ix < raw_str.Length && tgx < TaxGroupCount)
                        TaxGroupSubtotal[tgx++] = s2d(raw_str[ix++]);
                }
                else
                {
                    throw new FPException(FPLibError.BAD_RESPONSE);
                }
            }

            public decimal Change { get; set; }
            public FPPaymentType ChangeType { get; set; }

            public bool IsOpen { get; set; }
            public bool ForbiddenVoid { get; set; }
            public bool PrintVAT { get; set; }
            public bool PrintDetailedReceipt { get; set; }
            public bool PaymentInitiated { get; set; }
            public bool PaymentCompleted { get; set; }
            public bool IsInvoice { get; set; }
            public bool PowerDown { get; set; }
            public int SalesCount { get; set; }
            public decimal[] TaxGroupSubtotal { get; set; }
        }

        #endregion
    }

    /// <summary>
    /// Represents errors that occur during execution of fiscal printer commands.
    /// Error codes between 1 and 255 are received from the FP.
    /// Error codes above 255 are data and communication errors generated by the library.
    /// 
    /// To identify an error use ErrorCode public member instead of string message comparison.
    /// </summary>
    public class FPException : Exception
    {
        public static string lang = "en";
        private Exception base_exception;

        public int ErrorCode;

        public FPException(int error)
        {
            ErrorCode = error;
            base_exception = null;
        }

        public FPException(FPLibError error)
        {
            ErrorCode = (int)error;
            base_exception = null;
        }

        public FPException(int error, Exception innerException)
        {
            ErrorCode = error;
            base_exception = innerException;
        }

        public FPException(FPLibError error, Exception innerException)
        {
            ErrorCode = (int)error;
            base_exception = innerException;
        }

        public override Exception GetBaseException()
        {
            return base_exception;
        }

        public bool IsFPError
        {
            get
            {
                return ErrorCode > 0 && ErrorCode < 256;
            }
        }

        public override string Message
        {
            get
            {
                if (lang == "bg")
                {
                    if (ErrorCode == (int)FPLibError.SUCCESS)
                        return "Операцията завърши успешно.";

                    if (ErrorCode < 256)
                    {
                        string[] fp_err_bg = new string[16] { "OK", "Няма хартия", "Препълване в общите регистри", "Несверен / грешен часовник", "Отворен фискален бон", "Сметка с остатък за плащане (отворен бон)", "Отворен нефискален бон", "Сметка с приключено плащане (отворен бон)", "Фискална памет в режим само за четене", "Грешна парола или непозволена команда", "Липсващ външен дисплей", "24 часа без дневен отчет (блокировка)", "Прегрят принтер", "Спад на напрежение във фискален бон", "Препълване в електронната контролна лента", "Недостатъчни условия" };
                        string[] cmd_err_bg = new string[16] { "OK", "Невалидна", "Непозволена", "Непозволена поради ненулев отчет", "Синтактична грешка", "Синтактична грешка / препълване на входните регистри", "Синтактична грешка / нулев входен регистър", "Липсва транзакция, която да се войдира", "Недостатъчна налична сума", "Конфликт в данъчните групи", "?", "?", "?", "?", "?", "?" };

                        return string.Format("ФП: {0}; Команда: {1}.", fp_err_bg[ErrorCode >> 4], cmd_err_bg[ErrorCode & 15]);
                    }
                    switch ((FPLibError)ErrorCode)
                    {
                        case FPLibError.CRC:
                            return "Грешна контролна сума.";

                        case FPLibError.BAD_INPUT_DATA:
                            return "Некоректни входни данни.";

                        case FPLibError.NACK:
                            return "Отрицателен (NACK) отговор от фискалния принтер.";

                        case FPLibError.TIMEOUT:
                            return "Просрочено време за отговор от фискалния принтер.";

                        case FPLibError.RETRIED:
                            return "Фискалният принтер не може да изпълни операцията; Опитайте по-късно.";

                        case FPLibError.BAD_RECEIPT:
                            return "Получен е грешен отговор от фискалния принтер; Грешка при комуникация.";

                        case FPLibError.BAD_RESPONSE:
                            return "Получени са грешни данни от фискалния принтер; Грешка при комуникация.";

                        case FPLibError.NO_PRINTER:
                            return "Фискалният принтер не може да бъде открит.";

                        case FPLibError.PRINTER_BUSY:
                            return "Фискалният принтер е зает; Опитайте по-късно.";

                        case FPLibError.NBL_NOT_SAME:
                            return "Различен номер на блок на данните; Грешка при комуникация.";

                        case FPLibError.BUSY_TIMEOUT:
                            return "Фискалният принтер е зает; Просрочено време за отговор.";

                        case FPLibError.UNKNOWN_DEVICE:
                            return "Непознато устройство.";

                        case FPLibError.PORT_NOT_OPEN:
                            return "Серийният порт не е отворен.";

                        case FPLibError.PAPER_EMPTY:
                            return "Няма хартия.";
                    }
                    return "Непозната грешка.";
                }
                else
                {
                    if (ErrorCode == (int)FPLibError.SUCCESS)
                        return "Operation completed successfully.";

                    if (ErrorCode < 256)
                    {
                        string[] fp_err = new string[16] { "OK", "Paper out", "Daily registers overflow", "Invalid RTC date/time", "Open fiscal receipt", "Bill remainder not paid; Open receipt", "Open non-fiscal receipt", "Bill payment finished; Open receipt", "Fiscal memory is read-only", "Wrong password or command not allowed", "Missing display", "24 hours without daily report", "Printer overheat", "Power down", "Electronic journal is full", "Not enough conditions met" };
                        string[] cmd_err = new string[16] { "OK", "Invalid", "Illegal", "Denied because of uncommited report", "Syntax error", "Syntax error / Input register overflow", "Syntax error / Input register is zero", "Missing transaction for void", "Insufficient subtotal", "Tax groups conflict", "?", "?", "?", "?", "?", "?" };

                        return string.Format("FP: {0}; Command: {1}.", fp_err[ErrorCode >> 4], cmd_err[ErrorCode & 15]);
                    }
                    switch ((FPLibError)ErrorCode)
                    {
                        case FPLibError.CRC:
                            return "CRC error.";

                        case FPLibError.BAD_INPUT_DATA:
                            return "Incorrect input data.";

                        case FPLibError.NACK:
                            return "Negative response from FP.";

                        case FPLibError.TIMEOUT:
                            return "Timeout while waiting for fiscal printer response.";

                        case FPLibError.RETRIED:
                            return "The Fiscal Printer cannot complete the operation; Try again later.";

                        case FPLibError.BAD_RECEIPT:
                            return "Wrong FP receipt; Communication error.";

                        case FPLibError.BAD_RESPONSE:
                            return "Wrong FP response content; Communication error.";

                        case FPLibError.NO_PRINTER:
                            return "Fiscal Printer device cannot be found.";

                        case FPLibError.PRINTER_BUSY:
                            return "The Fiscal Printer is busy; Try again later.";

                        case FPLibError.NBL_NOT_SAME:
                            return "Wrong data block number; Communication error.";

                        case FPLibError.BUSY_TIMEOUT:
                            return "The Fiscal Printer is busy more than expected.";

                        case FPLibError.UNKNOWN_DEVICE:
                            return "Incompatible device; Not a Fiscal Printer.";

                        case FPLibError.PORT_NOT_OPEN:
                            return "The serial port is not open.";

                        case FPLibError.PAPER_EMPTY:
                            return "Out of paper or open cover.";
                    }
                    return "Unknown error.";
                }
            }
        }
    }

    #region Enumerations
    public enum FPLibError
    {
        SUCCESS = 0,
        UNDEFINED = 0x100,
        BAD_INPUT_DATA = 0x101,
        TIMEOUT = 0x102,
        NACK = 0x103,
        CRC = 0x104,
        BAD_RECEIPT = 0x105,
        BAD_RESPONSE = 0x106,
        RETRIED = 0x107,
        NO_PRINTER = 0x109,
        PRINTER_BUSY = 0x10A,
        NBL_NOT_SAME = 0x10B,
        BUSY_TIMEOUT = 0x10D,
        UNKNOWN_DEVICE = 0x10E,
        PORT_NOT_OPEN = 0x10F,
        PAPER_EMPTY = 0x110
    }

    public enum FPPaymentType
    {
        Cash = 0,
        Card = 1,
        Cheque = 2,
        Coupon = 3,
        ForeignCurrency = 4
    }

    public enum FPChangeType
    {
        Cash = 0,
        SameAsPayment = 1,
        ForeignCurrency = 2
    }

    public enum FPTextAlign
    {
        Center,
        Right,
        Left
    }
    #endregion
}
