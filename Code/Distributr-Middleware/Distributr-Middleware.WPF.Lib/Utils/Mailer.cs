using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Integration.Cussons.WPF.Lib.Utils
{
    public class Mailer
    {
        SmtpClient client = null;
        public Mailer(string IP, int port, string username, string password)
        {
            client = new SmtpClient(IP, port);
            NetworkCredential nc = new NetworkCredential(username, password);
            client.Credentials = nc;
        }

        public Mailer(string IP, string username, string password)
        {
            client = new SmtpClient(IP);
            NetworkCredential nc = new NetworkCredential(username, password);
            client.Credentials = nc;
        }

        public void Send(string source, string destination, string subject, string message)
        {
            MailMessage mail = new MailMessage(source, destination, subject, message);
            client.Send(mail);
        }
    }
}
