using System;
using System.Activities.Expressions;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Windows.Forms;
using Distributr.Core.Data.Utility;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Utility;
using Distributr.WPF.Lib.Service.Utility;
using FP3530;
using StructureMap;
using Xceed.Wpf.Toolkit.Core.Converters;

namespace Distributr.WPF.Lib.ViewModels.Transactional.Fiscalprinter
{
  public  class FiscalPrinterUtility
    {


      public FiscalPrinterUtility(int port, int comSpeed)
      {
          this.comPort = port;
          this.comSpeed = comSpeed;
      }

      public bool IsEnabled = false;
      public static bool IsConnected = false;
      private static CS_BGR_FP2000_KLClass FP2000KL;
      
        private int language = 1;
        int comPort; //todo:read port from settings
        int comSpeed; //todo:read port speed from settings
      
       bool OpenFiscalPrinter()
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
                MessageBox.Show("Open fiscal receipt failed");
            }
            catch(Exception ex)
            {
                
            }
          return false;
        }
       bool OpenNonFiscalPrinter()
       {
           int allRecs, fiscRecs;
           try
           {
               // Default operator code, operaror password and till number are used
               // These values should be set and kept somewhere in your software
               if (OpenNonFiscalReceipt(out allRecs, out fiscRecs))
               {
                   // Do something with all records and fiscal records
                   // if you wish
                   return true;
               }
               MessageBox.Show("Open Non fiscal receipt failed");
           }
           catch (Exception ex)
           {

           }
           return false;
       }
       public bool IsFiscalPrinterConnected()
       {
           ManagementObjectCollection ManObjReturn;
           ManagementObjectSearcher ManObjSearch;
           var wmiQuery = string.Format("Select * from Win32_SerialPort where DeviceID='{0}'",comPort);
           ManObjSearch = new ManagementObjectSearcher(wmiQuery);
           ManObjReturn = ManObjSearch.Get();
           var comObjects = (from ManagementObject ManObj in ManObjReturn
                             select new ComObject()
                             {
                                 DeciveId = ManObj["DeviceID"].ToString(),
                                 Name = ManObj["Name"].ToString(),
                                 Caption = ManObj["Caption"].ToString(),
                                 Description = ManObj["Description"].ToString(),
                                 ProviderType = ManObj["ProviderType"].ToString(),
                                 Status = ManObj["Status"].ToString(),
                                 PNPDeviceID = ManObj["PNPDeviceID"].ToString()
                             }).ToList();
           Log(string.Format("Test printer availability"));

           return true; //didn't really work for now...
       }


       public bool TestConnection()
       {
           try
           {
              // Log(string.Format("test printer availability"));
               FP2000KL = null;
               FP2000KL = new CS_BGR_FP2000_KLClass();
               Connect(comPort, comSpeed);
               return true;
           }catch(Exception)
           {
               return false;
           }
       }
      private void Log(string msg)
      {
          string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
          if(!string.IsNullOrEmpty(appPath))
          {
              var task = Path.Combine(appPath, "fiscalprinterlog.txt");
              using (var writer = new StreamWriter(task, true))
              {
                  writer.WriteLine(msg);
                  writer.Close();
              }
          }
      }
      public void PrintNonFiscalOrderReceipt(MainOrder order)
      {
          Log(string.Format("printing non fiscal order id={0}", order.Id));
          var printer = new FiscalPrinterUtility(comPort, comSpeed);

          if (!FiscalPrinterHelper._isConnected)
          {
              printer.ConnectFiscalPrinter();
          }

          if (printer.OpenNonFiscalPrinter())
          {
              int casesQuantity = 0;
              var spaces = string.Empty;
              foreach (var item in order.ItemSummary)
              {
                  if (item.Product.ReturnableType == ReturnableType.Returnable &&item.Value==0)
                  {
                      continue;
                  }

                  double price = (double)Math.Truncate((item.Value + item.VatValue) * 100) / 100;
                  var pricecheck = (item.Value + item.VatValue).GetTruncatedValue();

                  var bulkQuantity = 0.0m;
                  var calculationPrice = 0.0;
                  using (var container = ObjectFactory.Container.GetNestedContainer())
                  {
                      var summaryService = container.GetInstance<IProductPackagingSummaryService>();
                      bulkQuantity = summaryService.GetProductQuantityInBulk(item.Product.Id);
                      var modulo = item.Qty % bulkQuantity;
                      if (modulo != 0)
                      {
                          casesQuantity = (int)item.Qty;
                          calculationPrice = (double)((pricecheck));
                      }
                      else
                      {
                          casesQuantity = (int)item.Qty / (int)bulkQuantity;
                          calculationPrice = (double)(pricecheck).GetTruncatedValue();
                          calculationPrice = (double)((decimal)(calculationPrice * (double)bulkQuantity)).GetTotalGross();
                      }
                      
                  }

                  spaces = GenerateSpaces("\t", item.Product.ProductCode, casesQuantity + " x " + calculationPrice.ToString("0.00"), 48);
                  FP2000KL.CMD_42_0_0(item.Product.ProductCode + spaces + casesQuantity + " x " + calculationPrice.ToString("0.00"));

                  var total = casesQuantity * calculationPrice;
                  spaces = GenerateSpaces("\t", item.Product.Description, total.ToString("0.00"), 48);
                  FP2000KL.CMD_42_0_0(item.Product.Description + spaces + total.ToString("0.00"));


                  
              }

              if (order.SaleDiscount > 0)
              {
                  spaces = GenerateSpaces("\t", "Discount", order.SaleDiscount.ToString("0.00"), 48);
                  FP2000KL.CMD_42_0_0("Discount" + spaces + order.SaleDiscount.ToString("0.00"));
              }

              var paidAmount = order.GetPayments.Sum(n => n.Amount);
              var expectedPayment =order.SaleDiscount>0?(order.TotalGross.GetTotalGross()-order.SaleDiscount):order.TotalGross;
              var amountDue = paidAmount >= expectedPayment ? paidAmount : expectedPayment.GetTotalGross();
              var amountPaid = paidAmount;
              spaces = GenerateSpaces("-", string.Empty, string.Empty, 48);
              FP2000KL.CMD_42_0_0(spaces);
              spaces = GenerateSpaces("\t", "Total", amountDue.ToString("0.00"), 48);

              FP2000KL.CMD_42_0_0("Total" + spaces + amountDue.ToString("0.00"));

              spaces = GenerateSpaces("\t", "Paid", amountPaid.ToString("0.00"), 48);

              FP2000KL.CMD_42_0_0("Paid" + spaces + amountPaid.ToString("0.00"));
              FP2000KL.CMD_42_0_0("\n");
              FP2000KL.CMD_42_0_0("\n");
              
              printer.CloseNonFiscalPrinter();
          }

      }

      private string GenerateSpaces(string character,string initialString,string finalString,int maximumLength)
      {
          var spaces = string.Empty;
          var initialSize = initialString.Length;
          var finalSize = finalString.Length;
          var characterSize = maximumLength - (initialSize + finalSize);
          if (characterSize > 0)
          {
              for (int i = 0; i <= characterSize; i++)
              {
                  spaces = string.Concat(spaces, character);
              }
          }
          return spaces;
      }

      public void PrintReceipt(MainOrder order)
      {
          decimal calculatedTotal = 0m;
          decimal priceCalculatedTotal = 0m;
          foreach (var item in order.ItemSummary)
          {
              var vat = item.VatValue;
              string hasVAT = vat > 0 ? "A" : "B";

              double price = (double) Math.Truncate((item.Value + vat)*100)/100; 
              double price2 = (double) (item.Value + vat + item.ProductDiscount);
              int quantity = (int) item.Qty;

              calculatedTotal +=Convert.ToDecimal((item.Value+vat)*quantity);
              priceCalculatedTotal += Convert.ToDecimal(((double)Math.Truncate((item.Value + vat)*quantity) * 100) / 100);
             
          }
          calculatedTotal = calculatedTotal.GetTotalGross();
          calculatedTotal=(calculatedTotal-order.SaleDiscount).GetTotalGross();
          if (order.PaidAmount != calculatedTotal)
          {
              PrintNonFiscalOrderReceipt(order);
          }
          else
          {
              PrinterOrderReceipt(order);
          }

      }
       public void  PrinterOrderReceipt(MainOrder order)
       {
          Log(string.Format("printing order id={0}", order.Id));
           var printer = new FiscalPrinterUtility(comPort, comSpeed);

           if (!FiscalPrinterHelper._isConnected)
           {
               printer.ConnectFiscalPrinter();
           }
           
           if (printer.OpenFiscalPrinter())
           {
                   int casesQuantity = 0;
                   double calculatedTotal = 0.0;
                   decimal cumulativeTotal = 0m;
                   int pricePrecision = 0;
                   int quantityPrecision = 0;
                   foreach (var item in order.ItemSummary)
                   {
                       var vat = item.VatValue;
                       string hasVAT = vat> 0 ? "A" : "B";

                      
                       if (item.Product.ReturnableType == ReturnableType.Returnable && item.Value==0)
                       {
                           continue;
                       }

                       double price = (double)Math.Truncate((item.Value + vat)*100) / 100;
                       var pricecheck = (item.Value + vat).GetTruncatedValue();

                       cumulativeTotal += Convert.ToDecimal((item.Value + vat) * (int)item.Qty);

                       var bulkQuantity = 0.0m;
                       var calulationPrice = 0.0;
                       using (var container = ObjectFactory.Container.GetNestedContainer())
                       {
                           var summaryService = container.GetInstance<IProductPackagingSummaryService>();
                          bulkQuantity = summaryService.GetProductQuantityInBulk(item.Product.Id);
                           var modulo = item.Qty%bulkQuantity;
                           if (modulo != 0)
                           {
                               casesQuantity =(int) item.Qty;
                               calulationPrice = (double) ((pricecheck));
                           }
                           else
                           {
                               casesQuantity = (int)item.Qty / (int)bulkQuantity;
                               calulationPrice = (double)(pricecheck).GetTruncatedValue();
                               calulationPrice = (double)((decimal)(calulationPrice * (double)bulkQuantity)).GetTotalGross();
                           }
                           
                       }

                       double price2 =(double) (item.Value + vat+item.ProductDiscount);
                       int quantity = (int)item.Qty;
                       
                       var line1 = item.Product.ProductCode.Trim();
                       var line2 = item.Product.Description.Trim();


                       printer.Sell(line1, line2, hasVAT.Trim(), calulationPrice, pricePrecision, casesQuantity, quantityPrecision);

                        calculatedTotal += quantity * price;

                   }

                   

                   var paidAmount = order.GetPayments.Sum(n => n.Amount);
                   var expectedPayment = order.TotalGross;
                  
                  
                   int subtotal, sumA, sumB, sumC, sumD, sumE, sumF, sumG, sumH;

                   double salediscountValue = Convert.ToDouble(order.SaleDiscount);
                   printer.SaleDiscount(1, 0, -salediscountValue, out subtotal, out sumA, out sumB, out sumC, out sumD, out sumE, out sumF, out sumG, out sumH);

                   cumulativeTotal = cumulativeTotal.GetTotalGross();
                   cumulativeTotal = (cumulativeTotal - order.SaleDiscount).GetTotalGross();
                   var value = paidAmount;
                   printer.GetTotalQ((double)value);


                  

                   printer.CloseFiscalPrinter();
               }
           
       }

      public void ConnectFiscalPrinter()
      {
          try
          {
              FP2000KL = null;
              FP2000KL = new CS_BGR_FP2000_KLClass();
              Connect(comPort, comSpeed);
              Log(string.Format("Printer connected"));
              IsConnected = true;
              FiscalPrinterHelper._isConnected = true;
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
      
     bool SellWithDiscount(string line1, string line2, string taxGr, int price, int price_precision, int quan, int quan_precision, double percent)
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
      bool SellQWithDiscount(string line1, string line2, string taxGr, double price, int price_precision, int quan, int quan_precision, double percent)
      {
          try
          {
             return SellQDiscount(line1, line2, taxGr, price, price_precision, quan, quan_precision, percent);
          }
          catch (Exception ex)
          {
              return false;
          }
      }
       void GetTotal(int payamout = 1000,string line1="", string line2="")
      {
          try
          {
              bool fBonOpened = false, sBonOpened = false;
              int salesNum = 0;
              int amt = 0, tender = 0;
              
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
                              
                              //if (strPayCode == "R")
                              //    MessageBox.Show("Change: " + ((double)returnedAmount / 100).ToString());
                              //if (strPayCode == "D")
                              //    MessageBox.Show("To pay: " + ((double)returnedAmount / 100).ToString());
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
      void GetTotalQ(double payamout = 1000,string line1="", string line2="")
      {
          try
          {
              bool fBonOpened = false, sBonOpened = false;
              int salesNum = 0;
              int amt = 0, tender = 0;
              
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
                          if (TotalQ(line1, line2, "P", payamout, 0, out strPayCode, out returnedAmount))
                          {
                              
                              //if (strPayCode == "R")
                              //    MessageBox.Show("Change: " + ((double)returnedAmount / 100).ToString());
                              //if (strPayCode == "D")
                              //    MessageBox.Show("To pay: " + ((double)returnedAmount / 100).ToString());
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

      void SaleDiscount(int print, int display, double perc, out int subtotal, out int sumA, out int sumB,
            out int sumC, out int sumD, out int sumE, out int sumF, out int sumG, out int sumH)
      {
          
          string strPerc = string.Format("{0}.{1:D2}", (int)perc, ((int)(Math.Abs(perc) * 100)) % 100);
          string strSubtotal = "", strA = "", strB = "", strC = "", strD = "", strE = "", strF = "", strG = "", strH = "";


          int ret = 0;
          ret = FP2000KL.CMD_51_0_2(print.ToString(), display.ToString(), strPerc, ref strSubtotal, ref strA, ref strB, ref strC, ref strD, ref strE, ref strF, ref strG, ref strH);
          
          subtotal = Convert.ToInt32(strSubtotal);
          sumA = Convert.ToInt32(strA);
          sumB = Convert.ToInt32(strB);
          sumC = Convert.ToInt32(strC);
          sumD = Convert.ToInt32(strD);
          sumE = Convert.ToInt32(strE);
          sumF = Convert.ToInt32(strF);
          sumG = Convert.ToInt32(strG);
          sumH = Convert.ToInt32(strH);
         
      }
      void CloseFiscalPrinter()
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
      void CloseNonFiscalPrinter()
      {
          string allRecs="";
          try
          {
              if (CloseNonFiscalReceipt(allRecs))
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

      public static class FiscalPrinterHelper
      {

          public static bool _isConnected = false;
          public static void Reinitialize(int port,int comSpeed)
          {
              var fiscalPrinter = new FiscalPrinterUtility(port, comSpeed);
              fiscalPrinter.ConnectFiscalPrinter();
          }

      }
        private void Connect(int port, int speed)
        {
            FP2000KL.INIT_Ex1(port, speed);
           
        }

        private bool OpenNonFiscalReceipt(out int allReceipts, out int fiscReceipts)
        {
            allReceipts = 0;
            fiscReceipts = 0;
            try
            {
                int ret = 0;
                string strAllRec = "", strFiscRec = "";
                // FP-2000 KL
                ret = FP2000KL.CMD_38_0_0( ref strAllRec, ref strFiscRec);
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

        private bool CloseNonFiscalReceipt(string allReceipts)
        {
            try
            {
                int ret = 0;


                ret = FP2000KL.CMD_39_0_0(ref allReceipts);
                if (ret != 0)
                {
                    MessageBox.Show(CalcError(ret));
                    // Error handling
                    return false;
                }
                else
                {
                    allReceipts = ret.ToString();
                    
                }

            }
            catch
            {
                return false;
            }
            return true;
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
        private bool Sell(string line1, string line2, string taxGr, double price, int price_precision, int quan, int quan_precision)
        {
            try
            {
                int ret = 0;
                // Custom formating to ensure maximum precision and avoid culture specific decimal separators.
                

                string strPrice = price.ToString();
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

        private bool SellQDiscount(string line1, string line2, string taxGr, double price, int price_precision, int quan, int quan_precision, double percent)
        {
            try
            {
                int ret = 0;
                // Custom formating to ensure maximum precision and avoid culture specific decimal separators.
                string strPrice = price.ToString();
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
       private bool TotalQ(string line1, string line2, string payMode, double paysum, int paysum_precision, out string payCode, out int amount)
        {
            payCode = "";
            amount = 0;
            try
            {
                int ret = 0;
                string strRetAmt = "", strAmt = "";

                strAmt = paysum.ToString();

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


        class ComObject
        {
            public string DeciveId { get; set; }
            public string PNPDeviceID { get; set; }
            public string Name { get; set; }
            public string Caption { get; set; }
            public string Description { get; set; }

            public string ProviderType { get; set; }
            public string Status { get; set; }
        }
        
    }

 
}
