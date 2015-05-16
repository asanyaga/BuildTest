using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.WPF.Lib.Services.DocumentReports;
using Distributr.WPF.Lib.Services.DocumentReports.Invoice;
using Distributr.WPF.Lib.ViewModels.Transactional.InvoiceDocument;

namespace Distributr.WPF.Lib.ViewModels.PrintableDocuments
{
    public partial class DocumentReportViewerViewModel
    {
        public InvoiceReportContainer GetInvoice(Guid orderId)
        {
            InvoiceReportContainer doc = new InvoiceReportContainer();
            InvoiceReportHeader docHeader = doc.InvoiceHeader;
            CompanyHeaderReport compHeader = new CompanyHeaderReport();
            doc.CompanyHeader = compHeader;
            using (StructureMap.IContainer c = NestedContainer)
            {
                var invoice = LoadInvoiceAndReceipts(orderId, doc);
                
                Distributor distributr = null;
                CostCentre cc = invoice.DocumentIssuerCostCentre;
                CostCentre cc2 = invoice.DocumentRecipientCostCentre;

                if (cc is Distributor)
                {
                    distributr = cc as Distributor;
                }
                if (cc2 is Distributor)
                {
                    distributr = cc2 as Distributor;
                }

                //company info
                compHeader.CompanyName = cc.Name;

                Contact contact = null;
                if (distributr.Contact.Any())
                {
                    contact = distributr.Contact.FirstOrDefault(
                        n => n.ContactClassification == ContactClassification.PrimaryContact) ??
                              distributr.Contact.FirstOrDefault();
                }
                if (contact == null)
                {
                    var contacts = Using<IContactRepository>(c).GetByContactsOwnerId(distributr.Id);
                    contact = contacts.FirstOrDefault(
                        n => n.ContactClassification == ContactClassification.PrimaryContact) ??
                              contacts.FirstOrDefault();
                }
                if (contact != null)
                {
                    compHeader.PostalAddress = contact.PostalAddress;
                    compHeader.PhysicalAddress = contact.City == contact.PhysicalAddress
                                                     ? contact.City
                                                     : contact.City + " ," +
                                                       contact.PhysicalAddress;
                    compHeader.Telephone = contact.BusinessPhone;
                    compHeader.Fax = contact.Fax;
                    compHeader.CellNo = contact.MobilePhone;
                    compHeader.Email = contact.Email;
                }

                compHeader.VATNo = distributr != null ? distributr.VatRegistrationNo : "";
                compHeader.PINNo = distributr != null ? distributr.PIN : "";
                compHeader.WebSite = "";



                compHeader.ContactsConcat = compHeader.PhysicalAddress + ", "
                                                + compHeader.PostalAddress + ", ";
                compHeader.ContactsConcat += "Tel: " + compHeader.Telephone + ",";
                compHeader.ContactsConcat += "Cell: " + compHeader.CellNo + ",";

                if (!string.IsNullOrEmpty(compHeader.Fax))
                    compHeader.ContactsConcat += "Fax: " + compHeader.Fax + ", ";

                if (!string.IsNullOrEmpty(compHeader.VATNo))
                    compHeader.ContactsConcat += "VAT No: " + compHeader.VATNo + ".";

                if (!string.IsNullOrEmpty(compHeader.PINNo))
                    compHeader.ContactsConcat += "PIN No: " + compHeader.PINNo + ".";

                if (!string.IsNullOrEmpty(compHeader.Email))
                    compHeader.ContactsConcat += "Email: " + compHeader.Email + ".";

                if (!string.IsNullOrEmpty(compHeader.WebSite))
                    compHeader.ContactsConcat += "Web Site: " + compHeader.WebSite + ".";

                docHeader.InvoiceRef = invoice.DocumentReference;
                docHeader.InvoiceDate = invoice.DocumentDateIssued;
                docHeader.DocumentIssuerCCName = invoice.DocumentIssuerCostCentre.Name;
                docHeader.DocumentIssuerUserName = invoice.DocumentIssuerUser.Username;
                docHeader.InvoiceRecipientCompanyName = invoice.DocumentRecipientCostCentre.Name;
                docHeader.CreditTerms = "";
                docHeader.PreparedByUserName = invoice.DocumentIssuerUser.Username;
                docHeader.PreparedByJobTitle = "";

                docHeader.TotalNet = invoice.TotalNet;
                docHeader.TotalVat = invoice.TotalVat;
                docHeader.TotalGross = invoice.TotalGross;
                var order = Using<IMainOrderRepository>(c).GetById(invoice.OrderId);
                docHeader.SaleDiscount = order.SaleDiscount;

                doc.InvoiceHeader.InvoiceSubBalance = doc.InvoiceHeader.TotalGross - doc.InvoiceHeader.TotalDeductions;
                doc.InvoiceHeader.InvoiceBalance = doc.InvoiceHeader.InvoiceSubBalance - doc.InvoiceHeader.TotalAmountPaid;

                docHeader.InvoiceBalance -= docHeader.SaleDiscount;

                docHeader.TotalProductDiscount = invoice.LineItems.Sum(n => n.ProductDiscount);
                var ili = invoice.LineItems.Select((n, i) => new InvoiceReportLineItem
                                                                 {
                                                                     RowNumber = i + 1,
                                                                     Description = n.Product.Description,
                                                                     Qty = n.Qty,
                                                                     UnitPrice =
                                                                         (n.Value < 0 ? -n.Value : n.Value) +
                                                                         n.LineItemVatValue,
                                                                     TotalNet = n.LineItemTotal,
                                                                     UnitDiscount = n.ProductDiscount,
                                                                 });
                foreach (var item in ili) doc.InvoiceLineItems.Add(item);

                docHeader.DocumentIssuerDetails = "Generated by: " + docHeader.DocumentIssuerUserName + " ; Cost centre: " +
                                                 docHeader.DocumentIssuerCCName + "; Date: " +
                                                 docHeader.DatePrinted.ToShortDateString();
                return doc;
            }
        }

        public Invoice LoadInvoiceAndReceipts(Guid orderId, InvoiceReportContainer doc)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var invoice = Using<IInvoiceRepository>(c).GetInvoiceByOrderId(orderId);
                InvoiceReceipts.Clear();
                var rec = new Receipt(Guid.Empty)
                              {
                                  DocumentReference = "--Select Receipt To View--",
                              };
                InvoiceReceipts.Add(rec);
                SelectedReceipt = rec;
                var invReceipts = Using<IReceiptRepository>(c).GetByInvoiceId(invoice.Id);
                invReceipts.Where(n => n.Total > 0).ToList().ForEach(InvoiceReceipts.Add);

                doc.PaymentInformationLineItems =
                    invReceipts.Where(n => n.Total > 0).Where(n => n.Id != Guid.Empty).ToList()
                        .Select((n, i) => new InvoiceReportLineItem
                                              {
                                                  RowNumber = i + 1,
                                                  ReceiptDate = n.DocumentDateIssued,
                                                  InvoiceReceiptRefField = n.DocumentReference,
                                                  TotalNet = n.Total
                                              }).ToList();

                doc.InvoiceHeader.TotalAmountPaid = invReceipts.Sum(i => i.Total);

                LoadInvoiceDeductions(invoice,doc);

                //doc.InvoiceHeader.InvoiceBalance = doc.InvoiceHeader.InvoiceSubBalance - doc.InvoiceHeader.TotalAmountPaid;
                return invoice;
            }
        }

        public void LoadInvoiceDeductions(Invoice invoice, InvoiceReportContainer doc)
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                var invoiceCreditNotes =
                    Using<ICreditNoteRepository>(c).GetAll().OfType<CreditNote>().Where(
                        n => n.InvoiceId == invoice.Id);

                doc.InvoiceDeductionsLineItems.Clear();
                foreach (var cn in invoiceCreditNotes)
                {
                    cn.LineItems.Select((n, i) => new InvoiceReportLineItem
                                                      {
                                                          RowNumber = i + 1,
                                                          TotalNet = n.LineItemTotal,
                                                          InvoiceCreditNoteRefField = cn.DocumentReference,
                                                          Qty = n.Qty,
                                                          Description = n.Product.Description,
                                                          UnitPrice = n.Value + n.LineItemVatValue,
                                                      }).ToList().ForEach(doc.InvoiceDeductionsLineItems.Add);
                }

                doc.InvoiceHeader.TotalDeductions = invoiceCreditNotes.Sum(n => n.Total);
                //doc.InvoiceHeader.InvoiceSubBalance = doc.InvoiceHeader.TotalGross - doc.InvoiceHeader.TotalDeductions;
            }
        }
    }
}
