using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using Distributr.Core.Notifications;
using Distributr.Core.Repository.Master.CostCentreRepositories;

namespace Distributr.BusSubscriber.Service
{
    public class NotificationResolver : INotificationResolver
    {
        private ICostCentreRepository _costCentreRepository;
        private IContactRepository _contactRepository;


        public NotificationResolver(ICostCentreRepository costCentreRepository, IContactRepository contactRepository)
        {
            _costCentreRepository = costCentreRepository;
            _contactRepository = contactRepository;
        }

        public NotificationEnvelope Mail(NotificationBase notification)
        {
            
            if(notification is NotificationOrderSale)
            {
                var envelope = new NotificationEnvelope();
                envelope.MailMessage= FormatOrderSale(notification as NotificationOrderSale);
                envelope.SmsMessage = FormatOrderSaleSms(notification as NotificationOrderSale);
                return envelope;
            }
            if (notification is NotificationInvoice)
            {
                var envelope = new NotificationEnvelope();
                envelope.MailMessage = FormatInvoice(notification as NotificationInvoice);
                envelope.SmsMessage = FormatInvoiceSms(notification as NotificationInvoice);

                return envelope;
            }
            if (notification is NotificationReceipt)
            {
                var envelope = new NotificationEnvelope();
                envelope.MailMessage = FormatReceipt(notification as NotificationReceipt);
                envelope.SmsMessage = FormatReceiptSms(notification as NotificationReceipt);
                return envelope;
            }
            if (notification is NotificationPurchase)
            {
                var envelope = new NotificationEnvelope();
                envelope.MailMessage = FormatPurchase(notification as NotificationPurchase);
                envelope.SmsMessage = FormatPurchaseSms(notification as NotificationPurchase);
                return envelope;
            }
            if (notification is NotificationDispatch)
            {
                var envelope = new NotificationEnvelope();
                envelope.MailMessage = FormatDispatch(notification as NotificationDispatch);
                envelope.SmsMessage = FormatDispatchSms(notification as NotificationDispatch);
                return envelope;
            }
            if (notification is NotificationDelivery)
            {
                var envelope = new NotificationEnvelope();
                envelope.MailMessage = FormatDelivery(notification as NotificationDelivery);
                envelope.SmsMessage = FormatDeliverySms(notification as NotificationDelivery);
                return envelope;
            }
            return null;
        }

        private NotificationSMS FormatPurchaseSms(NotificationPurchase noti)
        {
            var sms = new NotificationSMS();
            var farmer = _costCentreRepository.GetById(noti.FarmerId);
            if (farmer == null)
                throw new Exception("Invalid farmer");
            sms.HubId = noti.HubId;
            sms.Recipitents = GetPhoneNumber(new List<Guid> { noti.HubId, noti.PurchaseClerkId, noti.FarmerId });
            string body = "";

               body = "PurchaseID= " + noti.DocumentRef + "\n"
               + "Farmer=" + farmer.Name + ",\n"
               + "CUM.Weight=" + noti.CummulativeWeightDetail + ",\n"
               + "and Center=" + noti.CenterName + "\n";

            foreach (var item in noti.Items)
            {
                body +="Commodity=" +  item.ItemName + ",\n" 
                     + "Grade=" + item.Grade + ",\n"
                     + "NT.Weight=" + item.Quantity;
            }
            body += "\nStatus= Purchased";

            sms.SmsBody = body;
            return sms;
        }
        
        private NotificationSMS FormatInvoiceSms(NotificationInvoice noti)
        {
            var sms = new NotificationSMS();
            sms.HubId = noti.DistributorId;
            sms.Recipitents = GetPhoneNumber(new List<Guid> { noti.DistributorId, noti.SalemanId, noti.OutletId });// new List<string> { "254722557538" };
            sms.SmsBody = "Invoice " + noti.DocumentRef + "\n"
                + "of Vat AMT=" + noti.TotalVat + ",\n"
                + "Discount  AMT=" + noti.SalevalueDiscount + ",\n"
                + "Gross AMT=" + noti.TotalGross + ",\n"
                + "and  Net AMT=" + noti.TotalNet + "\nStatus= Approved ";

            return sms;
        }
        
        private NotificationSMS FormatReceiptSms(NotificationReceipt noti)
        {
            var sms = new NotificationSMS();
            sms.HubId = noti.DistributorId;
            sms.Recipitents = GetPhoneNumber(new List<Guid> { noti.DistributorId, noti.SalemanId, noti.OutletId });// new List<string> { "254722557538" };
            sms.SmsBody = "Receipt " + noti.DocumentRef + ",\n"
                + "OrderRef" + noti.OrderRef + ",\n"
                + "InvoiceRef=" + noti.InvoiceRef + ",\n"
                + "and Amount Paid=" + noti.TotalAmount.ToString("N2") + "\nStatus= Received ";

            return sms;
        }

        private NotificationSMS FormatOrderSaleSms(NotificationOrderSale noti)
        {
            var sms = new NotificationSMS();
            sms.HubId = noti.DistributorId;
            sms.Recipitents = GetPhoneNumber(new List<Guid> { noti.DistributorId, noti.SalemanId, noti.OutletId });// new List<string> { "254722557538" };
            sms.SmsBody = "Order " + noti.DocumentRef + "\n"
                + "of Vat AMT=" + noti.TotalVat + ",\n"
                + "Discount  AMT=" + noti.SalevalueDiscount + ",\n"
                + "Gross AMT=" + noti.TotalGross + ",\n"
                + "and  Net AMT=" + noti.TotalNet + "\nStatus= Confirmed ";
            return sms;
        }
        

        private NotificationSMS FormatDispatchSms(NotificationDispatch noti)
        {
            var sms = new NotificationSMS();
            sms.HubId = noti.DistributorId;
            sms.Recipitents = GetPhoneNumber(new List<Guid> { noti.DistributorId, noti.SalemanId, noti.OutletId });// new List<string> { "254722557538" };
            sms.SmsBody = "DIspatch " + noti.DocumentRef + ",\n"
                + "OrderRef =" + noti.OrderRef + "\nStatus= Dispatched ";
            return sms;
        }

        private NotificationSMS FormatDeliverySms(NotificationDelivery noti)
        {
            var sms = new NotificationSMS();
            sms.HubId = noti.DistributorId;
            sms.Recipitents = GetPhoneNumber(new List<Guid> { noti.DistributorId, noti.SalemanId, noti.OutletId });// new List<string> { "254722557538" };
            sms.SmsBody = "Delivery " + noti.DocumentRef + ",\n"
               + "OrderRef =" + noti.OrderRef + "\nStatus= Delivered ";
            return sms;
        }
        
        private MailMessage FormatDispatch(NotificationDispatch notification)
        {
            var distributor = _costCentreRepository.GetById(notification.DistributorId);
            var salesman = _costCentreRepository.GetById(notification.SalemanId);
            var outlet = _costCentreRepository.GetById(notification.OutletId);
            if (outlet == null || distributor == null || salesman == null)
                throw new Exception("Invalid distributor , salesman or outlet");
            var msg = new MailMessage();
            GetAddresses(distributor.Id).ForEach(msg.To.Add);
            GetAddresses(salesman.Id).ForEach(msg.To.Add);
            GetAddresses(outlet.Id).ForEach(msg.To.Add);
            // msg.To.Add("juvegitau@yahoo.com");


            if (msg.To.Count == 0) return null;
            msg.Subject = string.Format("Dispatch {0} Notification", notification.DocumentRef);
            msg.IsBodyHtml = true;
           
            string html = "";
            foreach (var item in notification.Items)
            {
                html += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", item.ItemName, item.Quantity);
            }
            string body = string.Empty;
          
            using (StreamReader reader = new StreamReader("service/resources/DispatchEmailTemplate.htm"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{Distributor}", distributor.Name);
            body = body.Replace("{Salesman}", salesman.Name);
            body = body.Replace("{Outlet}", outlet.Name);
          
            body = body.Replace("{OrderRef}", notification.OrderRef);
            body = body.Replace("{DocumentRef}", notification.DocumentRef);


            body = body.Replace("{Items}", html);

            msg.Body = body;
            return msg;
        }
        
        private MailMessage FormatDelivery(NotificationDelivery notification)
        {
            var distributor = _costCentreRepository.GetById(notification.DistributorId);
            var salesman = _costCentreRepository.GetById(notification.SalemanId);
            var outlet = _costCentreRepository.GetById(notification.OutletId);
            if (outlet == null || distributor == null || salesman == null)
                throw new Exception("Invalid distributor , salesman or outlet");
            var msg = new MailMessage();
            GetAddresses(distributor.Id).ForEach(msg.To.Add);
            GetAddresses(salesman.Id).ForEach(msg.To.Add);
            GetAddresses(outlet.Id).ForEach(msg.To.Add);
           


            if (msg.To.Count == 0) return null;
            msg.Subject = string.Format("Delivery {0} Notification", notification.DocumentRef);
            msg.IsBodyHtml = true;

            string html = "";
            foreach (var item in notification.Items)
            {
                html += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", item.ItemName, item.Quantity);
            }
            string body = string.Empty;

            using (StreamReader reader = new StreamReader("service/resources/DeliveryEmailTemplate.htm"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{Distributor}", distributor.Name);
            body = body.Replace("{Salesman}", salesman.Name);
            body = body.Replace("{Outlet}", outlet.Name);

            body = body.Replace("{OrderRef}", notification.OrderRef);
            body = body.Replace("{DocumentRef}", notification.DocumentRef);


            body = body.Replace("{Items}", html);

            msg.Body = body;
            return msg;
        }
        
        private MailMessage FormatPurchase(NotificationPurchase notification)
        {
            var hub = _costCentreRepository.GetById(notification.HubId);
            var clerk = _costCentreRepository.GetById(notification.PurchaseClerkId);
            var farmer = _costCentreRepository.GetById(notification.FarmerId);
            if (farmer == null || hub == null || clerk == null)
                throw new Exception("Invalid Hub , clerk or farmer");
            var msg = new MailMessage();
            GetAddresses(hub.Id).ForEach(msg.To.Add);
            GetAddresses(clerk.Id).ForEach(msg.To.Add);
            GetAddresses(farmer.Id).ForEach(msg.To.Add);
            //msg.To.Add("juvegitau@yahoo.com");


            if (msg.To.Count == 0) return null;
            msg.Subject = string.Format("Purchase {0} Notification", notification.DocumentRef);
            msg.IsBodyHtml = true;
            string html = "";
            foreach (var item in notification.Items)
            {
                html += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", item.ItemName, item.Grade, item.Quantity);
            }
            string body = string.Empty;
            using (StreamReader reader = new StreamReader("service/resources/agrimanagr/PurchaseEmailTemplate.htm"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{Hub}", hub.Name + " (" + hub.CostCentreCode + ")");
            body = body.Replace("{Clerk}", notification.ServedBy);
            body = body.Replace("{Farmer}", farmer.Name+" ("+farmer.CostCentreCode+")");
            body = body.Replace("{DocumentRef}", notification.DocumentRef);
            body = body.Replace("{Items}", html);
            body = body.Replace("{CummulativeWeightDetail}", notification.CummulativeWeightDetail);
            body = body.Replace("{CenterName}", notification.CenterName);

            msg.Body = body;
            return msg;
        }
        
        private MailMessage FormatReceipt(NotificationReceipt notification)
        {
            var distributor = _costCentreRepository.GetById(notification.DistributorId);
            var salesman = _costCentreRepository.GetById(notification.SalemanId);
            var outlet = _costCentreRepository.GetById(notification.OutletId);
            if (outlet == null || distributor == null || salesman == null)
                throw new Exception("Invalid distributor , salesman or outlet");
            var msg = new MailMessage();
            GetAddresses(distributor.Id).ForEach(msg.To.Add);
            GetAddresses(salesman.Id).ForEach(msg.To.Add);
            GetAddresses(outlet.Id).ForEach(msg.To.Add);
            //msg.To.Add("juvegitau@yahoo.com");


            if (msg.To.Count == 0) return null;
            msg.Subject = string.Format("Reciept {0} Notification", notification.DocumentRef);
            msg.IsBodyHtml = true;
            string html = "";
            foreach (var item in notification.Items)
            {
                html += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", item.ItemName, item.Reference, item.Description, item.Quantity);
            }
            string body = string.Empty;
            using (StreamReader reader = new StreamReader("service/resources/ReceiptEmailTemplate.htm"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{Distributor}", distributor.Name);
            body = body.Replace("{Salesman}", salesman.Name);
            body = body.Replace("{Outlet}", outlet.Name);
          
            body = body.Replace("{TotalGross}", notification.TotalAmount.ToString("N2"));
            body = body.Replace("{OrderRef}", notification.OrderRef);
            body = body.Replace("{DocumentRef}", notification.DocumentRef);
            body = body.Replace("{InvoiceRef}", notification.InvoiceRef);


            body = body.Replace("{Items}", html);

            msg.Body = body;
            return msg;
        }
       
        private MailMessage FormatInvoice(NotificationInvoice notification)
        {
            var distributor = _costCentreRepository.GetById(notification.DistributorId);
            var salesman = _costCentreRepository.GetById(notification.SalemanId);
            var outlet = _costCentreRepository.GetById(notification.OutletId);
            if (outlet == null || distributor == null || salesman == null)
                throw new Exception("Invalid distributor , salesman or outlet");
            var msg = new MailMessage();
            GetAddresses(distributor.Id).ForEach(msg.To.Add);
            GetAddresses(salesman.Id).ForEach(msg.To.Add);
            GetAddresses(outlet.Id).ForEach(msg.To.Add);
           // msg.To.Add("juvegitau@yahoo.com");


            if (msg.To.Count == 0) return null;
            msg.Subject = string.Format("Invoice {0} Notification", notification.DocumentRef);
            msg.IsBodyHtml = true;
            string html = "<div>";
            html += "</br><table >";
            html += "<thead><tr><th>Item</th><th>Quantity</th><th>UnitPrice</th><th>VAT</th><th>Discount</th><th>Total Gross</th><th>Total Net</th></tr></thead>";
            foreach (var item in notification.Items)
            {
                html += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td></tr>", item.ItemName, item.Quantity, item.UnitPrice, item.TotalVat, item.Discount, item.TotalGross, item.TotalNet);
            }
            html += " </table>";

            string body = string.Empty;
            using (StreamReader reader = new StreamReader("service/resources/invoiceEmailTemplate.htm"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{Distributor}", distributor.Name);
            body = body.Replace("{Salesman}", salesman.Name);
            body = body.Replace("{Outlet}", outlet.Name);
            body = body.Replace("{SalevalueDiscount}", notification.SalevalueDiscount.ToString("N2"));
            body = body.Replace("{TotalVat}", notification.TotalVat.ToString("N2"));
            body = body.Replace("{TotalNet}", notification.TotalNet.ToString("N2"));
            body = body.Replace("{TotalGross}", notification.TotalGross.ToString("N2"));
            body = body.Replace("{OrderRef}", notification.OrderRef);
            body = body.Replace("{DocumentRef}", notification.DocumentRef);


            body = body.Replace("{Items}", html);

            msg.Body = body;
            return msg;
        }
        
        private MailMessage FormatOrderSale(NotificationOrderSale notification)
        {
            var distributor = _costCentreRepository.GetById(notification.DistributorId);
            var salesman = _costCentreRepository.GetById(notification.SalemanId);
            var outlet = _costCentreRepository.GetById(notification.OutletId);
            if (outlet == null || distributor == null || salesman == null)
                throw new Exception("Invalid distributor , salesman or outlet");
            var msg = new MailMessage();
            GetAddresses(distributor.Id).ForEach(msg.To.Add);
            GetAddresses(salesman.Id).ForEach(msg.To.Add);
            GetAddresses(outlet.Id).ForEach(msg.To.Add);
           // msg.To.Add("juvegitau@yahoo.com");
           

            if (msg.To.Count == 0) return null;
            msg.Subject = string.Format("Order / Sale {0} Notification", notification.DocumentRef);
            msg.IsBodyHtml = true;
            string html = "<div>";
           html += "</br><table >";
           html += "<thead><tr><th>Item</th><th>Quantity</th><th>UnitPrice</th><th>VAT</th><th>Discount</th><th>Total Gross</th><th>Total Net</th></tr></thead>";
            foreach (var item in notification.Items)
            {
                html += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td></tr>", item.ItemName, item.Quantity, item.UnitPrice, item.TotalVat, item.Discount, item.TotalGross, item.TotalNet);  
            }
            html += " </table>";
          
            string body = string.Empty;
            using (StreamReader reader = new StreamReader("service/resources/orderEmailTemplate.htm"))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{Distributor}", distributor.Name);
            body = body.Replace("{Salesman}", salesman.Name);
            body = body.Replace("{Outlet}", outlet.Name);
            body = body.Replace("{SalevalueDiscount}", notification.SalevalueDiscount.ToString("N2"));
            body = body.Replace("{TotalVat}", notification.TotalVat.ToString("N2"));
            body = body.Replace("{TotalNet}", notification.TotalNet.ToString("N2"));
            body = body.Replace("{TotalGross}", notification.TotalGross.ToString("N2"));
            body = body.Replace("{DocumentRef}", notification.DocumentRef);
          
          

            body = body.Replace("{Items}", html);
            
            msg.Body = body;
            return msg;
        }

        private List<MailAddress>  GetAddresses(Guid costCenterId)
        {
            var addresses = new List<MailAddress>();
            var costcentre = _contactRepository.GetByContactsOwnerId(costCenterId);
            if (costcentre != null)
            {
                foreach (var contact in costcentre)
                {
                    addresses.Add(new MailAddress(contact.Email));
                }
            }
            return addresses;
        }
        private List<string> GetPhoneNumber(List<Guid> costCenterIds)
        {
            var addresses = new List<string>();
            foreach (var costCenterId in costCenterIds)
            {
                var costcentre = _contactRepository.GetByContactsOwnerId(costCenterId);
                if (costcentre != null)
                {
                    foreach (var contact in costcentre)
                    {
                        addresses.Add(contact.BusinessPhone);
                    }
                }
            }
            return addresses;
        }
    }
}