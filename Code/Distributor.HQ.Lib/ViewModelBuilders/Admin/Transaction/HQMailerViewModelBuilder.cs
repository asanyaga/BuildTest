using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction
{
   public class HQMailerViewModelBuilder:IHQMailerViewModelBuilder
    {
              SmtpClient client = null;

              
               public HQMailerViewModelBuilder(string IP, int port, string username, string password)
               {
                   client = new SmtpClient(IP, port);
                   NetworkCredential nc = new NetworkCredential(username, password);
                   client.EnableSsl = true;
                   client.Credentials = nc;
               }

               public HQMailerViewModelBuilder(string IP, string username, string password)
               {
                   client = new SmtpClient(IP);
                   NetworkCredential nc = new NetworkCredential(username, password);
                   client.Credentials = nc;
               }
       public HQMailerViewModelBuilder()
               {

               }
        public void Send(string source, string destination, string subject, string message)
        {
            try
            {
                MailMessage m = new MailMessage(source, destination, subject, message);
                client.Send(m);
            }
            catch (Exception ex)
            {

                throw new ArgumentException("Failed sending mail", ex.ToString());
            }
        }
    }
}
