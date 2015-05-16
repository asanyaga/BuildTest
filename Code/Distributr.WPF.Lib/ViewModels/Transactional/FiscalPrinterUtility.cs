using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FP3530;

namespace Distributr.WPF.Lib.ViewModels.Transactional
{
  public  class FiscalPrinterUtility
    {

      public bool IsFiscalPrinterConnected()
      {
          using (var searcher =
            new ManagementObjectSearcher(@"Select * From Win32_USBControllerDevice"))
          {
              using (var collection = searcher.Get())
              {
                  foreach (var device in collection)
                  
                  {
                     
                      var usbDevice = Convert.ToString(device);

                   //   if (usbDevice.Contains(pid) && usbDevice.Contains(vid))
                          //return true;
                  }
              }
          }
          return false;
      }
      public FiscalPrinterUtility(int port = 2, int comSpeed = 115200)
      {
          this.comPort = port;
          this.comSpeed = comSpeed;
      }

        private CS_BGR_FP2000_KLClass FP2000KL;
        private int language = 1;
        int comPort; //todo:read port from settings
        int comSpeed; //todo:read port speed from settings
      
      public bool OpenFiscalPrinter()
        {
            int allRecs, fiscRecs;
            try
            {
                // Default operator code, operaror password and till number are used
                // These values should be set and kept somewhere in your software
                if (OpenFiscalReceipt(1, "0000", 1, out allRecs, out fiscRecs))
                {
                    // Do something with all records and fiscal records
                    // if you wish
                    return true;
                }
                else
                {
                    MessageBox.Show("Open fiscal receipt failed");

                }
            }
            catch(Exception ex)
            {
                
            }
            finally
            {
               
            }
          return false;
        }

      public void ConnectFiscalPrinter()
      {
          try
          {
              Connect(comPort, comSpeed);
          }
          catch (ArgumentException ex)
          {
              MessageBox.Show(ex.Message);
          }
          catch (FormatException ex)
          {
              MessageBox.Show(ex.Message);
          }
          catch (OverflowException ex)
          {
              MessageBox.Show(ex.Message);
          }
          catch (Exception ex)
          {
              MessageBox.Show(ex.Message);
             
          }
      }
      public bool Sell(string line1, string line2, string taxGr, int price, int price_precision, int quan, int quan_precision)
      {
          try
          {
            return  AddProduct(line1, line2, taxGr, price, price_precision, quan, quan_precision);
          }
          catch (Exception ex)
          {
              return false;
          }
      }
      public bool SellWithDiscount(string line1, string line2, string taxGr, int price, int price_precision, int quan, int quan_precision, double percent)
      {
          try
          {
             return SellDiscount(line1, line2, taxGr, price, price_precision, quan, quan_precision, percent);
          }
          catch (Exception ex)
          {
              return false;
          }
      }
      public void GetTotal(string line1, string line2)
      {
          try
          {
              bool fBonOpened = false, sBonOpened = false;
              int salesNum = 0;
              int amt = 0, tender = 0;
              int payamout = 1000;
              if (CheckFiscalTransactionStatus(out fBonOpened, out sBonOpened, out salesNum, out amt, out tender))
              {
                  if (sBonOpened)
                  {
                      MessageBox.Show("Non fiscal receipt opened");
                      throw (new Exception());
                  }
                  if (fBonOpened)
                  {
                      if ((tender < amt) || ((amt == 0) && (tender == 0)))
                      {
                          string strPayCode = "";
                          int returnedAmount = 0;
                          if (Total(line1,line2,"P", payamout, 0, out strPayCode, out returnedAmount))
                          {
                              // Do something if you wish like show the retured amount
                              if (strPayCode == "R")
                                  MessageBox.Show("Change: " + ((double)returnedAmount / 100).ToString());
                              if (strPayCode == "D")
                                  MessageBox.Show("To pay: " + ((double)returnedAmount / 100).ToString());
                          }
                          else
                          {
                              MessageBox.Show("Total failed");
                              throw (new Exception());
                          }
                      }
                      else
                      {
                          MessageBox.Show("No need for total");
                          throw (new Exception());
                      }
                  }
                  else
                  {
                      MessageBox.Show("Fiscal receipt not opened");
                      throw (new Exception());
                  }
              }
              else
              {
                  MessageBox.Show("Check Fiscal Transaction Status failed");
              }
          }
          catch
          {
              // Do exception handling
          }
          finally
          {
              // UpdateStatusBits();
          }
      }
      public void CloseFiscalPrinter()
      {
          int allRecs, fiscRecs;
          try
          {
              if (CloseFiscalReceipt(out allRecs, out fiscRecs))
              {
                  // Do something with all records and fiscal records
                  // if you wish
              }
              else
              {
                  MessageBox.Show("Close fiscal receipt failed");
              }
          }
          catch
          {
              // Do exception handling
          }
          finally
          {
              // UpdateStatusBits();
          }
      }

      #region Helpers
        private void Connect(int port, int speed)
        {
            FP2000KL.INIT_Ex1(port, speed);
        }
        private bool OpenFiscalReceipt(int opCode, string opPWD, int tillNumber, out int allReceipts, out int fiscReceipts)
        {
            allReceipts = 0;
            fiscReceipts = 0;
            try
            {
                int ret = 0;
                string strAllRec = "", strFiscRec = "";
                // FP-2000 KL
                ret = FP2000KL.CMD_48_0_0(opCode.ToString(), opPWD, tillNumber.ToString(), ref strAllRec, ref strFiscRec);
                if (ret != 0)
                {
                    MessageBox.Show(CalcError(ret));
                    // Error handling
                    return false;
                }
                else
                {
                    allReceipts = Convert.ToInt32(strAllRec);
                    fiscReceipts = Convert.ToInt32(strFiscRec);
                }

            }
            catch
            {
                return false;
            }
            return true;
        }
        private bool CloseFiscalReceipt(out int allReceipts, out int fiscReceipts)
        {
            allReceipts = 0;
            fiscReceipts = 0;
            try
            {
                int ret = 0;
                string strAllRec = "", strFiscRec = "";


                ret = FP2000KL.CMD_56_0_0(ref strAllRec, ref strFiscRec);
                if (ret != 0)
                {
                    MessageBox.Show(CalcError(ret));
                    // Error handling
                    return false;
                }
                else
                {
                    allReceipts = Convert.ToInt32(strAllRec);
                    fiscReceipts = Convert.ToInt32(strFiscRec);
                }

            }
            catch
            {
                return false;
            }
            return true;
        }
        private string CalcError(int error)
        {

            return "Last error code: " + error.ToString() + "\n" +
                   "Last error message: " + FP2000KL.GET_LASTERROR(language);


        }
        private bool AddProduct(string line1, string line2, string taxGr, int price, int price_precision, int quan, int quan_precision)
        {
            try
            {
                int ret = 0;
                // Custom formating to ensure maximum precision and avoid culture specific decimal separators.
                string strPrice = string.Format("{0}.{1}", (int) (price/Math.Pow(10, price_precision)),
                                                ((int) (price%Math.Pow(10, price_precision))).ToString(
                                                    string.Format("D{0}", price_precision)));
                string strQuan = string.Format("{0}.{1}", (int) (quan/Math.Pow(10, quan_precision)),
                                               ((int) (quan%Math.Pow(10, quan_precision))).ToString(string.Format(
                                                   "D{0}", quan_precision)));

                ret = FP2000KL.CMD_49_2_0(line1, line2, taxGr, strPrice, strQuan);
                if (ret != 0)
                {
                    MessageBox.Show(CalcError(ret));
                    // Error handling
                    return false;
                }

            }
            catch
            {
                return false;
            }
            return true;
        }
        private bool SellDiscount(string line1, string line2, string taxGr, int price, int price_precision, int quan, int quan_precision, double percent)
        {
            try
            {
                int ret = 0;
                // Custom formating to ensure maximum precision and avoid culture specific decimal separators.
                string strPrice = string.Format("{0}.{1}", (int)(price / Math.Pow(10, price_precision)), ((int)(price % Math.Pow(10, price_precision))).ToString(string.Format("D{0}", price_precision)));
                string strQuan = string.Format("{0}.{1}", (int)(quan / Math.Pow(10, quan_precision)), ((int)(quan % Math.Pow(10, quan_precision))).ToString(string.Format("D{0}", quan_precision)));
                string strPerc = string.Format("{0}.{1:D2}", (int)percent, ((int)(Math.Abs(percent) * 100)) % 100);
               
               ret = FP2000KL.CMD_49_0_0(line1, line2, taxGr, strPrice, strQuan, strPerc);
               if (ret != 0)
               {
                   MessageBox.Show(CalcError(ret));
                   // Error handling
                   return false;
               }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private bool Total(string line1, string line2, string payMode, int paysum, int paysum_precision, out string payCode, out int amount)
        {
            payCode = "";
            amount = 0;
            try
            {
                int ret = 0;
                string strRetAmt = "", strAmt = "";

                strAmt = string.Format("{0}.{1}", (int)(paysum / Math.Pow(10, paysum_precision)),
                                       ((int)(paysum % Math.Pow(10, paysum_precision))).ToString(string.Format("D{0}",
                                                                                                              paysum_precision)));
                ret = FP2000KL.CMD_53_0_0(line1, line2, payMode, strAmt, ref payCode, ref strRetAmt);
                if (ret != 0)
                {
                    MessageBox.Show(CalcError(ret));
                    // Error handling
                    return false;
                }
                else
                {
                    amount = Convert.ToInt32(strRetAmt);
                }

            }
            catch
            {
                return false;
            }
            return true;
        }
        private bool CheckFiscalTransactionStatus(out bool fReceiptOpened, out bool sReceiptOpened, out int salesNumber, out int amount, out int tender)
        {
            fReceiptOpened = false;
            sReceiptOpened = false;
            salesNumber = 0;
            amount = 0;
            tender = 0;
            try
            {
                int ret = 0;
                string strRecOpend = "", strSalesNum = "", strAmount = "", strTender = "";

                ret = FP2000KL.CMD_76_0_0(ref strRecOpend, ref strSalesNum, ref strAmount, ref strTender);
                if (ret != 0)
                {
                    MessageBox.Show(CalcError(ret));
                    // Error handling
                    return false;
                }
                else
                {
                    if (0 == strRecOpend.CompareTo("1"))
                    {
                        fReceiptOpened = FP2000KL.GET_BIT_STATE(2, 3);
                        sReceiptOpened = FP2000KL.GET_BIT_STATE(2, 5);
                    }
                    salesNumber = Convert.ToInt32(strSalesNum);
                    amount = Convert.ToInt32(strAmount);
                    tender = Convert.ToInt32(strTender);
                }

            }
            catch
            {
                return false;
            }
            return true;
        }
       #endregion

        public void Dispose()
        {
            if (this.FP2000KL != null)
                this.FP2000KL = null;
        }
        
        
    }
}
