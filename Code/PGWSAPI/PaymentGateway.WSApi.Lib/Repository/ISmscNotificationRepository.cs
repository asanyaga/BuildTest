using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaymentGateway.WSApi.Lib.Domain.Notifications;

namespace PaymentGateway.WSApi.Lib.Repository
{
    public interface ISmscNotificationRepository : IRepositoryMaster<PgNotification>
    {
        List<ViewNotification> GetAllNotification();
    }
    public class ViewNotification
    {
        public string ReferenceNumber { set; get; }
        public decimal Amount { get; set; }
        public string Payee { get; set; }
        public Guid ApplicationId { get; set; }
        public string ResponseStatus { get; set; }
        public string PhoneNumber { get; set; }
        public string Type { get; set; }
        public string DateCreated { get; set; }
    }
}
