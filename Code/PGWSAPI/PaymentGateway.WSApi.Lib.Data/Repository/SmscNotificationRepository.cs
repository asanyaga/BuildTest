using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Data.EF;
using PaymentGateway.WSApi.Lib.Domain.Notifications;
using PaymentGateway.WSApi.Lib.Repository;

namespace PaymentGateway.WSApi.Lib.Data.Repository
{
    public class SmscNotificationRepository : ISmscNotificationRepository
    {
        private PGDataContext _ctx;

        public SmscNotificationRepository(PGDataContext ctx)
        {
            _ctx = ctx;
        }

        public int Save(PgNotification entity)
        {
            tblRequest toSave = new tblRequest();
            toSave.Amount = entity.Amount;
            toSave.ApplicationId = entity.ApplicationId;
            toSave.DateCreated = DateTime.Now;
            toSave.Payee = entity.Payee;
            toSave.ReferenceId = entity.ReferenceNumber.ToString();
            if (entity is SmscNotification)
            {
                SmscNotification smcs = entity as SmscNotification;
                toSave.Smsc_PhoneNumber = smcs.OutletPhoneNumber;
                toSave.Smsc_TillNumber = smcs.TillNumber;
                toSave.MessageType = (int)entity.MessageType;

            } if (entity is SmscPaymentConfirmation)
            {
                SmscPaymentConfirmation smcs = entity as SmscPaymentConfirmation;
                toSave.Smsc_PhoneNumber = smcs.OutletPhoneNumber;
                toSave.Smsc_TillNumber = smcs.TillNumber;
                toSave.MessageType = (int)entity.MessageType;

            }
            if (entity is Eazy247Notification)
            {
                Eazy247Notification smcs = entity as Eazy247Notification;
                toSave.Smsc_PhoneNumber = smcs.OutletPhoneNumber;
                toSave.Smsc_TillNumber = smcs.BillerNumber;
                toSave.MessageType = (int)entity.MessageType;

            }
            if (entity is Easy247Payment)
            {
                Easy247Payment smcs = entity as Easy247Payment;
                toSave.Smsc_PhoneNumber = smcs.OutletPhoneNumber;
                toSave.Smsc_TillNumber = smcs.BillerNumber;
                toSave.MessageType = (int)entity.MessageType;

            }
            _ctx.tblRequest.Add(toSave);
            _ctx.SaveChanges();
            return toSave.Id;
        }

        public PgNotification GetById(int Id)
        {
            tblRequest request = _ctx.tblRequest.FirstOrDefault(p => p.Id == Id);
            if (request == null)
                return null;
            return Map(request);

        }
        PgNotification Map(tblRequest request)
        {
            PgNotification note = new PgNotification();
            if((PgMessageType)request.MessageType==PgMessageType.SMSCN)
            {
                note = new SmscNotification();
            }
            if ((PgMessageType)request.MessageType == PgMessageType.SMSCP)
            {
                note = new SmscPaymentConfirmation();
            }
            note.Amount = request.Amount;
            note.ApplicationId = request.ApplicationId;
            note.MessageType = (PgMessageType) request.MessageType;
            note.Payee = request.Payee;
            note.ReferenceNumber = request.ReferenceId;
                                       
            if(note.MessageType==PgMessageType.SMSCN)
            {
               var smscn = note as SmscNotification;
                smscn.OutletPhoneNumber = request.Smsc_PhoneNumber;
                smscn.TillNumber = request.Smsc_TillNumber;
            }
          
            return note;
        }
        public IEnumerable<PgNotification> GetAll()
        {
            return _ctx.tblRequest.ToList().Select(p => Map(p)).ToList();
        }

        public List<ViewNotification> GetAllNotification()
        {
            return _ctx.tblRequest.OrderByDescending(b=>b.DateCreated).Take(20).ToList().Select(p => new ViewNotification
                                                             {
                                                                 Amount=p.Amount,
                                                                 ApplicationId=p.ApplicationId,
                                                                 Payee=p.Payee,
                                                                 ReferenceNumber=p.ReferenceId,
                                                                 ResponseStatus=_ctx.tblRequestResponce.Any(s=>s.ReferenceId==p.ReferenceId)==true?_ctx.tblRequestResponce.FirstOrDefault(s=>s.ReferenceId==p.ReferenceId).StatusDetails:"",
                                                                PhoneNumber=p.Smsc_PhoneNumber,
                                                                Type=((PgMessageType)p.MessageType).ToString(),
                                                                DateCreated=p.DateCreated.ToString()
                                                             }).ToList();
        }
    }
}
