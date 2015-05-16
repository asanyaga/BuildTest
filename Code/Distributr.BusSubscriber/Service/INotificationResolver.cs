using System.Net.Mail;
using Distributr.Core.Notifications;

namespace Distributr.BusSubscriber
{
    public interface INotificationResolver
    {
        NotificationEnvelope Mail(NotificationBase notification);
       
    }
    public class  NotificationEnvelope
    {
        public MailMessage MailMessage { get; set; }
        public NotificationSMS SmsMessage { get; set; }
    }


}