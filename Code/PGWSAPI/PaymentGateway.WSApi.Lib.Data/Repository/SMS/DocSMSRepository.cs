using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PaymentGateway.WSApi.Lib.Data.EF;
using PaymentGateway.WSApi.Lib.Domain.Payments.Client;
using PaymentGateway.WSApi.Lib.Domain.SMS.Client;
using PaymentGateway.WSApi.Lib.Repository.Payments.Utility;
using PaymentGateway.WSApi.Lib.Repository.SMS;

namespace PaymentGateway.WSApi.Lib.Data.Repository.SMS
{
    public class DocSMSRepository : IDocSMSRepository
    {
        private PGDataContext _ctx;
        private IAuditLogRepository _auditLogRepository;
        public int Save(DocSMS entity)
        {
            tblSms tsms = _ctx.tblSms.FirstOrDefault(n => n.Id == entity.Id);
            if (tsms == null)
            {
                tsms = new tblSms();
                tsms.Id = entity.Id;
                tsms.DateCreated = DateTime.Now;
                _ctx.tblSms.Add(tsms);
            }
            tsms.DateLastUpdated = DateTime.Now;
            tsms.ClientRequestResponseType = (int) ClientRequestResponseType.SMS;
            tsms.DistributorCostCenterId = entity.DistributorCostCenterId;
            tsms.DocumentId = entity.DocumentId;
            tsms.DocumentType = entity.DocumentType;
            tsms.Recipients = JsonConvert.SerializeObject(entity.Recipitents);
            tsms.SMSBody = entity.SmsBody;
            tsms.SmsStatus = SmsStatuses.Pending.ToString();

            return _ctx.SaveChanges();
        }

        public bool SaveResponse(DocSMSResponse response)
        {
            tblSms tsms = _ctx.tblSms.FirstOrDefault(n => n.Id == response.Id);
            if(tsms != null)
            {
                tsms.SdpDestinationResponses = JsonConvert.SerializeObject(response.SdpDestinationResponses);
                tsms.SdpRequestId = response.SdpRequestId;
                tsms.SdpStatusCode = response.SdpResponseCode;
                tsms.SdpStatusString = response.SdpResponseStatus;
                tsms.SdpVersion = response.SdpVersion;
                tsms.SmsStatus =  SmsStatuses.Sent.ToString();
                return true;
            }

            return false;
        }

        public DocSMS GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DocSMS> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
