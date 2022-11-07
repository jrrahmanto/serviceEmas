using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace DailyStatementEmas
{
    public class SendEmail
    {
        public static void sendEmail(List<string> to_email, List<string> cc_email, List<path> path, string subject, string text)
        {
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress("automatic_ptkbi@outlook.com");
            foreach (var item_send in to_email)
            {
                message.To.Add(new MailAddress(item_send));
            }
            foreach (var item_send in cc_email)
            {
                message.CC.Add(new MailAddress(item_send));
            }

            message.Subject = subject;
            message.IsBodyHtml = true; //to make message body as html  
            message.Body = text;
            foreach (var item in path)
            {
                message.Attachments.Add(new Attachment(item.pathname));
            }
            smtp.Port = 587;
            smtp.Host = "smtp.outlook.com"; //for gmail host  
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = new NetworkCredential("automatic_ptkbi@outlook.com", "Jakarta2021");

            smtp.Send(message);
        }
        public class path
        {
            public string pathname { get; set; }
            public string filename { get; set; }
        }
    }
}
