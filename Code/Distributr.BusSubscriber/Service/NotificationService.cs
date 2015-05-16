using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Notifications;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Utility;
using Distributr.WSAPI.Lib.Services.CommandAudit;
using log4net;
using Newtonsoft.Json;
using StructureMap;

namespace Distributr.BusSubscriber.Service
{
    public  class  NotificationService:INotificationService
    {
        private static readonly ILog _log = LogManager.GetLogger("NotificationService Logger");
        private INotificationProcessingAuditRepository _processingAuditRepository;
        private ISettingsRepository _settingsRepository;

        public NotificationService( INotificationProcessingAuditRepository processingAuditRepository, ISettingsRepository settingsRepository)
        {
           
            _processingAuditRepository = processingAuditRepository;
            _settingsRepository = settingsRepository;
        }


        public void Start()
        {
           _log.Info("start notificatification service");
            var cancellationTokenSource = new CancellationTokenSource();
            var task = Repeat.Interval(TimeSpan.FromMinutes(10),this.Send, cancellationTokenSource.Token);
            
        }

        private void Send()
        {
            _log.Info("Resolve sending service");
            var allowSendEmail = _settingsRepository.GetByKey(SettingsKeys.AllowSendEmail);
            bool canSendEmail = false;
            if (allowSendEmail != null) bool.TryParse(allowSendEmail.Value, out canSendEmail);

            var allowSendsms = _settingsRepository.GetByKey(SettingsKeys.AllowSendSms);
            bool canSendsms = false;
            if (allowSendsms != null) bool.TryParse(allowSendsms.Value, out canSendsms);
            _log.InfoFormat("Send email ={0} , Send sms {1}", canSendEmail, canSendsms);
            if (canSendEmail || canSendsms) ResolveSendEmail(canSendEmail, canSendsms);

           
        }

        private void ResolveSendEmail(bool cansendemail,bool cansendsms)
        {
            var notifications = _processingAuditRepository.GetUnSent();
            using (var c = ObjectFactory.Container.GetNestedContainer())
            {
                var notificationResolver = c.GetInstance<INotificationResolver>();
                foreach (var n in notifications)
                {
                    try
                    {
                        var item = JsonConvert.DeserializeObject<NotificationBase>(n.JsonNotification);
                        var mail = notificationResolver.Mail(item);
                        if (mail != null && mail.MailMessage != null && cansendemail)
                            SendEmail(mail.MailMessage, n);
                        if (mail != null && mail.SmsMessage != null && cansendsms)
                            SendSms(mail.SmsMessage, n);
                        n.Status = NotificationProcessingStatus.InProgress;
                        _processingAuditRepository.Add(n);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
            }
        }
        private async void SendSms(NotificationSMS mail, NotificationProcessingAudit audit)
        {
            _log.Info(" sending sms ");
            var info = new NotificationProcessingAuditInfo
            {
                Id = audit.Id.ToString() + "_SMS",
                DateInserted = DateTime.Now,
                NotificationId = audit.Id,
                Type = "SMS",
                Contact = string.Join(",", mail.Recipitents.ToArray())
            };

            try
            {
            var smsuri = _settingsRepository.GetByKey(SettingsKeys.SmsUri);
            string smsurivalue = "";
            if (smsuri != null) smsurivalue = smsuri.Value;
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(1);
            client.BaseAddress = new Uri(smsurivalue);

            string urlSuffix = "api/gateway/sms/postnotification";
            _log.InfoFormat("Sending sms to {0} {1}",smsuri,urlSuffix);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
           

           
               
                var response = await client.PostAsJsonAsync(urlSuffix, mail );
                response.EnsureSuccessStatusCode();
                string r = await response.Content.ReadAsStringAsync();
                info.Info = r;
                _processingAuditRepository.Add(info);

            }
            catch (Exception ex)
            {

                info.Info = ex.Message;
                if (ex.InnerException != null)
                    info.Info += " --> " + ex.InnerException.Message;
                _processingAuditRepository.Add(info);

            }





        }

        private void SendEmail(MailMessage mail, NotificationProcessingAudit audit)
        {
            var smtphost = _settingsRepository.GetByKey(SettingsKeys.SmtpHost);
            string smtphostvalue = "";
            if (smtphost != null) smtphostvalue = smtphost.Value;

            var smtpport = _settingsRepository.GetByKey(SettingsKeys.SmptPort);
            int smtportvalue = 0;
            if (smtpport != null)
            {
                int.TryParse(smtpport.Value, out smtportvalue);
            }

            var smtpEmail = _settingsRepository.GetByKey(SettingsKeys.SmptEmail);
            string smtpEmailvalue = "";
            if (smtpEmail != null) smtpEmailvalue = smtpEmail.Value;

            var smtpusername = _settingsRepository.GetByKey(SettingsKeys.SmptUsername);
            string smtpusernamevalue = "";
            if (smtpusername != null) smtpusernamevalue = smtpusername.Value;

            var smtppassword = _settingsRepository.GetByKey(SettingsKeys.SmptPassword);
            string smtppasswordvalue = "";
            if (smtppassword != null) smtppasswordvalue = VCEncryption.DecryptString(smtppassword.Value);

            if (string.IsNullOrWhiteSpace(smtpEmailvalue)
                || string.IsNullOrWhiteSpace(smtphostvalue)
                || smtportvalue == 0
                || string.IsNullOrWhiteSpace(smtpusernamevalue)
                || string.IsNullOrWhiteSpace(smtppasswordvalue))
                throw new Exception("Invalid notification settings");

            SmtpClient client = new SmtpClient();
            mail.From = new MailAddress(smtpEmailvalue);
            client.Host = smtphostvalue;
            client.Port = smtportvalue;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(smtpusernamevalue, smtppasswordvalue);
            client.SendCompleted += (sender, e) =>
                                        {
                                            var notification = (NotificationProcessingAudit) e.UserState;
                                            var info = new NotificationProcessingAuditInfo
                                            {
                                                Id = notification.Id.ToString()+"_Email",
                                                DateInserted = DateTime.Now,
                                                NotificationId = notification.Id,
                                                Type = "Email",
                                                 Contact = string.Join(",", mail.To.Select(s=>s.Address).ToArray())
                                            };

                                           
                                            if (e.Error != null)
                                            {
                                                info.Info = e.Error.Message;
                                                if (e.Error.InnerException != null)
                                                    info.Info += " --> " + e.Error.InnerException.Message;
                                                _processingAuditRepository.Add(info);
                                            }
                                            else
                                            {
                                                notification.Info = "Sent";
                                                _processingAuditRepository.Add(info);
                                            }


                                        };
            client.SendAsync(mail, audit);

        }

        public void Stop()
        {
          
           
        }
    }

    internal static class Repeat
    {
        public static Task Interval(
            TimeSpan pollInterval,
            Action action,
            CancellationToken token)
        {
            // We don't use Observable.Interval:
            // If we block, the values start bunching up behind each other.
            return Task.Factory.StartNew(
                () =>
                {
                    for (; ; )
                    {
                        if (token.WaitCancellationRequested(pollInterval))
                            break;

                        action();
                    }
                }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
    }

    static class CancellationTokenExtensions
    {
        public static bool WaitCancellationRequested(
            this CancellationToken token,
            TimeSpan timeout)
        {
            return token.WaitHandle.WaitOne(timeout);
        }
    }
}