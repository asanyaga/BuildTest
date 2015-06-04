using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;

namespace Distributr.Core.Domain.MailerEmailEntities
{
    /// <summary>
    /// Not used???
    /// </summary>
    public class MailerSettings : MasterEntity
    {
        public MailerSettings(Guid id) : base(id)
        {
        }

        public MailerSettings(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus status) : base(id, dateCreated, dateLastUpdated, status)
        {
        }

        public string SmtpClientHost { get; set; }
        public int SmtpClientPort { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public bool EnableSsl { get; set; }
        public bool IsDefault { get; set; }
        public SmtpDeliveryMethod DeliveryMethod { get; set; }
        public string PickupDirectoryLocation { get; set; }
    }
}
