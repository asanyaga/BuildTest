using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master;

namespace Distributr.Core.Domain.MailerEmailEntities
{
    /// <summary>
    /// Not Used ????
    /// </summary>
    public class Email : MasterEntity
    {
        public Email(Guid id) : base(id)
        {
        }

        public Email(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus status) : base(id, dateCreated, dateLastUpdated, status)
        {
        }

        public string SenderAddress { get; set; }
        public List<string> RecipientAddresses { get; set; }
        public List<string> CcAddresses { get; set; }
        public List<string> BCcAddresses { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<EmailAttachment> Attachments { get; set; }
        public bool SendCopyToSelf { get; set; }
        public bool IsSent { get; set; }
    }

    public class EmailAttachment : MasterEntity
    {
        public EmailAttachment(Guid id) : base(id)
        {
        }

        public EmailAttachment(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus status) : base(id, dateCreated, dateLastUpdated, status)
        {
        }

        public byte[] Attachment { get; set; }
        public string AttachmentFileType { get; set; }
        public string AttachmentFilePath { get; set; }
    }
}
