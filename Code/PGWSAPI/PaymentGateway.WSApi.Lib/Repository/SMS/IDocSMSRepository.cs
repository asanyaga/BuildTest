using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentGateway.WSApi.Lib.Domain.SMS.Client;

namespace PaymentGateway.WSApi.Lib.Repository.SMS
{
    public interface IDocSMSRepository : IRepositoryMaster<DocSMS>
    {
        bool SaveResponse(DocSMSResponse response);
    }
}
