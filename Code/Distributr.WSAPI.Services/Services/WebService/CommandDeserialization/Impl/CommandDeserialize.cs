using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.DisbursementNotes;
using Distributr.Core.Commands.DocumentCommands.Discounts;
using Distributr.Core.Commands.DocumentCommands.InventoryReceivedNotes;
using Distributr.Core.Commands.DocumentCommands.Invoices;
using Distributr.Core.Commands.DocumentCommands.Losses;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Commands.DocumentCommands.Receipts;
using Distributr.WSAPI.Lib.API.WebService.CommandValidation;
using Distributr.Core.Commands;
using Distributr.Core.Commands.DocumentCommands.AdjustmentNotes;
using Distributr.Core.Commands.DocumentCommands.InventoryTransferNotes;
using Distributr.Core.Commands.DocumentCommands.DispatchNotes;
using Distributr.Core.Commands.DocumentCommands.ReturnsNotes;
using Distributr.Core.Commands.DocumentCommands.CreditNotes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Distributr.WSAPI.Lib.Services.WebService.CommandDeserialization.Impl
{
    public class CommandDeserialize : ICommandDeserialize
    {
        ICommandValidate _commandValidate;
        public CommandDeserialize(ICommandValidate commandValidate)
        {
            _commandValidate = commandValidate;
        }

        public ICommand DeserializeCommand(string commandType, string jsoncommand)
        {
            CommandType ct = GetCommandType(commandType);
            return DeserializeCommand(ct, jsoncommand);
        }

        public DateTime DeserializeSendDateTime(string sendDateTime)
        {
            DateTime _sendDateTime =DateTime.Now;
            //string d = JsonConvert.SerializeObject(DateTime.Now, new IsoDateTimeConverter()).Replace("\"", "");
            try
            {
                IsoDateTimeConverter converter = new IsoDateTimeConverter();
                string testString = sendDateTime.Replace(" ","+"); // JsonConvert.SerializeObject(_sendDateTime, new IsoDateTimeConverter());

                DateTimeOffset xx = JsonConvert.DeserializeObject<DateTimeOffset>(testString, converter);

                _sendDateTime = xx.DateTime;// DateTime.Parse(sendDateTime);
            }catch
            {
            }
            
            return _sendDateTime;
        }

        CommandType GetCommandType(string commandType)
        {
            CommandType _commandType;
            Enum.TryParse(commandType, out _commandType);
            return _commandType;
        }
      
        ICommand DeserializeCommand(CommandType ct, string jsoncommand)
        {
            switch (ct)
            {
                //--------------- Order 
                case CommandType.CreateOrder:
                    CreateOrderCommand coc = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out coc);
                    return coc;

                case CommandType.AddOrderLineItem:
                    AddOrderLineItemCommand aoli = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out aoli);
                    return aoli;

                case CommandType.ConfirmOrder:
                    ConfirmOrderCommand co = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out co);
                    return co;

                case CommandType.ApproveOrder:
                    ApproveOrderCommand ao = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out ao);
                    return ao;

                case CommandType.ChangeOrderLineItem:
                    ChangeOrderLineItemCommand ccl = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out ccl);
                    return ccl;

                case CommandType.RejectOrder:
                    RejectOrderCommand ro = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out ro);
                    return ro;

                case CommandType.RemoveOrderLineItem:
                    RemoveOrderLineItemCommand rol = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out rol);
                    return rol;

                case CommandType.CloseOrder:
                    CloseOrderCommand coc1 = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out coc1);
                    return coc1;

                case CommandType.BackOrder:
                    BackOrderCommand bo1 = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out bo1);
                    return bo1;

                case CommandType.OrderPendingDispatch:
                    OrderPendingDispatchCommand opd1 = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out opd1);
                    return opd1;

                case CommandType.DispatchToPhone:
                    DispatchToPhoneCommand dpc1 = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out dpc1);
                    return dpc1;

                //-------------- IAN
                case CommandType.CreateInventoryAdjustmentNote:
                    CreateInventoryAdjustmentNoteCommand cianc = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out cianc);
                    return cianc;

                case CommandType.AddInventoryAdjustmentNoteLineItem:
                    AddInventoryAdjustmentNoteLineItemCommand aianli = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out aianli);
                    return aianli;

                case CommandType.ConfirmInventoryAdjustmentNote:
                    ConfirmInventoryAdjustmentNoteCommand cian = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out cian);
                    return cian;

                // ---------------- DN
                case CommandType.CreateDispatchNote:
                    CreateDispatchNoteCommand cdnc = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out cdnc);
                    return cdnc;

                case CommandType.AddDispatchNoteLineItem:
                    AddDispatchNoteLineItemCommand adnli = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out adnli);
                    return adnli;

                case CommandType.ConfirmDispatchNote:
                    ConfirmDispatchNoteCommand cdn = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out cdn);
                    return cdn;

                //------ IRN
                case CommandType.CreateInventoryReceivedNote:
                    CreateInventoryReceivedNoteCommand cirn = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out cirn);
                    return cirn;

                case CommandType.AddInventoryReceivedNoteLineItem:
                    AddInventoryReceivedNoteLineItemCommand airnli = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out airnli);
                    return airnli;

                case CommandType.ConfirmInventoryReceivedNote:
                    ConfirmInventoryReceivedNoteCommand cirn1 = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out cirn1);
                    return cirn1;


                //------------ ITN
                case CommandType.CreateInventoryTransferNote:
                    CreateInventoryTransferNoteCommand citnc = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out citnc);
                    return citnc;

                case CommandType.AddInventoryTransferNoteLineItem:
                    AddInventoryTransferNoteLineItemCommand aitnli = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out aitnli);
                    return aitnli;

                case CommandType.ConfirmInventoryTransferNote:
                    ConfirmInventoryTransferNoteCommand citn = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out citn);
                    return citn;

                //----------- Invoice 
                case CommandType.CreateInvoice:
                    CreateInvoiceCommand ci = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out ci);
                    return ci;

                case CommandType.AddInvoiceLineItem:
                    AddInvoiceLineItemCommand iic = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out iic);
                    return iic;

                case CommandType.ConfirmInvoice:
                    ConfirmInvoiceCommand cic1 = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out cic1);
                    return cic1;

                case CommandType.CloseInvoice:
                    CloseInvoiceCommand cic2 = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out cic2);
                    return cic2;

                //----------- Receipt 
                case CommandType.CreateReceipt:
                    CreateReceiptCommand cr = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out cr);
                    return cr;

                case CommandType.AddReceiptLineItem:
                    AddReceiptLineItemCommand ar = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out ar);
                    return ar;

                case CommandType.ConfirmReceiptLineItem:
                    ConfirmReceiptLineItemCommand crl = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out crl);
                    return crl;

                case CommandType.ConfirmReceipt:
                    ConfirmReceiptCommand crc = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out crc);
                    return crc;

                //----------- Disbursement note 
                case CommandType.CreateDisbursementNote:
                    CreateDisbursementNoteCommand cdbn = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out cdbn);
                    return cdbn;

                case CommandType.AddDisbursementNoteLineItem:
                    AddDisbursementNoteLineItemCommand adbnl = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out adbnl);
                    return adbnl;

                case CommandType.ConfirmDisbursementNote:
                    ConfirmDisbursementNoteCommand codbn = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out codbn);
                    return codbn;

                //----------- Returns note 
                case CommandType.CreateReturnsNote:
                    CreateReturnsNoteCommand srnc = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out srnc);
                    return srnc;

                case CommandType.AddReturnsNoteLineItem:
                    AddReturnsNoteLineItemCommand arnli = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out arnli);
                    return arnli;

                case CommandType.ConfirmReturnsNote:
                    ConfirmReturnsNoteCommand cornc = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out cornc);
                    return cornc;
                case CommandType.CloseReturnsNote:
                    CloseReturnsNoteCommand clornc = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out clornc);
                    return clornc;

                //----------- Loss note 
                case CommandType.CreatePaymentNote:
                    CreatePaymentNoteCommand clc = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out clc);
                    return clc;

                case CommandType.AddPaymentNoteLineItem:
                    AddPaymentNoteLineItemCommand allic = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out allic);
                    return allic;

                case CommandType.ConfirmPaymentNote:
                    ConfirmPaymentNoteCommand colc = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out colc);
                    return colc;




                //----------- Credit note 
                case CommandType.CreateCreditNote:
                    CreateCreditNoteCommand cnc = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out cnc);
                    return cnc;

                case CommandType.AddCreditNoteLineItem:
                    AddCreditNoteLineItemCommand aclic = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out aclic);
                    return aclic;

                case CommandType.ConfirmCreditNote:
                    ConfirmCreditNoteCommand cocc = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out cocc);
                    return cocc;

              //---------------------Discount--------------------
                case CommandType.CreateDiscount:
                    CreateDiscountCommand cdc = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out cdc);
                    return cdc;

                case CommandType.AddDiscountLineItem:
                    AddDiscountLineItemCommand adc = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out adc);
                    return adc;

                case CommandType.ConfirmDiscount:
                    ConfirmDiscountCommand cd = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out cd);
                    return cd;
                    //Retire Document;
                case CommandType.RetireDocument:
                    RetireDocumentCommand rdc = null;
                    _commandValidate.CanDeserializeCommand(jsoncommand, out rdc);
                    return rdc;

                default:
                    throw new Exception("Failed to deserialize command in command deserializer");
            }

            //return null;
        }
    }
}
