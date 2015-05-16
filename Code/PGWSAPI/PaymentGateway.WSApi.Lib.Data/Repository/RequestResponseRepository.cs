using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Data.EF;
using PaymentGateway.WSApi.Lib.Domain.ResultResponse;
using PaymentGateway.WSApi.Lib.Repository;

namespace PaymentGateway.WSApi.Lib.Data.Repository
{
    public class RequestResponseRepository : IRequestResponseRepository
    {
        private PGDataContext _ctx;

        public RequestResponseRepository(PGDataContext ctx)
        {
            _ctx = ctx;
        }

        public int Save(RequestResponse entity)
        {
            tblRequestResponce tblresponse = new tblRequestResponce();
            tblresponse.DateCreated = DateTime.Now;
            tblresponse.MessageId = entity.messageId;
            tblresponse.ReferenceId = entity.ReferenceId;
            tblresponse.StatusCode = entity.statusCode;
            tblresponse.StatusDetails = entity.statusDetail;
            tblresponse.Vesion = entity.version;
            foreach (DestinationResponse edest in entity.DesinationResponses)
            {
                tblRequestResponceDestination dest = new tblRequestResponceDestination();
                dest.Address = edest.address;
                dest.DateCreated = DateTime.Now;
                dest.ResponseTimeStamp = edest.timeStamp;
                dest.StatusCode = edest.statusCode;
                dest.StatusDetails = edest.statusDetail;
                dest.MessageId = edest.messageId;
                tblresponse.tblRequestResponceDestination.Add(dest);
            }
            _ctx.tblRequestResponce.Add(tblresponse);
            _ctx.SaveChanges();
            return tblresponse.Id;
        }

        public RequestResponse GetById(int Id)
        {
            tblRequestResponce tbRe = _ctx.tblRequestResponce.FirstOrDefault(s => s.Id == Id);
            return Map(tbRe);

        }
        RequestResponse Map(tblRequestResponce rp)
        {
            if (rp == null) return null;
            RequestResponse response = new RequestResponse
                                           {
                                               messageId = rp.MessageId,
                                               statusCode = rp.StatusCode,
                                               statusDetail = rp.StatusDetails,
                                               version = rp.Vesion,
                                           };
            foreach (var item in rp.tblRequestResponceDestination)
            {
                DestinationResponse dr = Map(item);
                response.DesinationResponses.Add(dr);
            }
            return response;


        }
        DestinationResponse Map(tblRequestResponceDestination dest)
        {
            if (dest == null) return null;
            DestinationResponse desitintion = new DestinationResponse
                                                  {
                                                      address=dest.Address,
                                                      timeStamp=dest.ResponseTimeStamp,
                                                      messageId=dest.MessageId,
                                                      statusCode=dest.StatusCode,
                                                      statusDetail=dest.StatusDetails
                                                  };
            return desitintion;
        }
        public IEnumerable<RequestResponse> GetAll()
        {
            return _ctx.tblRequestResponce.ToList().Select(s => Map(s)).ToList();
        }
    }
}
