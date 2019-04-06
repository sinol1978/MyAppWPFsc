using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Excel = Microsoft.Office.Interop.Excel;

namespace MyAppWPF
{

    public static class EmailSender
    {
        public static void SendMail(string toAddress, string body)
        {
            MailMessage msg = new MailMessage();

            //NameValueCollection appSettings = ConfigurationManager.AppSettings;

            msg.To.Add(toAddress);
            msg.From = new MailAddress("furnorders@gmail.com");// appSettings["smtpuser"]);
            msg.Subject = "Ваш заказ";
            msg.SubjectEncoding = Encoding.UTF8;
            msg.Body = body;
            msg.BodyEncoding = Encoding.UTF8;
            msg.IsBodyHtml = false;
            msg.Priority = MailPriority.High;
            //Add the Creddentials
            SmtpClient client = new SmtpClient();
            client.Credentials = new NetworkCredential("furnorders@gmail.com", "sinol1978IT");
            client.Port = int.Parse("587");
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Send(msg);
        }
        public static void SendMailWithAttachment(string toAddress, string filename)
        {
            MailMessage msg = new MailMessage();

            //NameValueCollection appSettings = ConfigurationManager.AppSettings;

            msg.To.Add(toAddress);
            msg.From = new MailAddress("furnorders@gmail.com");// appSettings["smtpuser"]);
            msg.Subject = "Ваш заказ";
            msg.SubjectEncoding = Encoding.UTF8;
            msg.Body = "Добрый день!\nВаш заказ находится во вложенном файле.";
            msg.BodyEncoding = Encoding.UTF8;
            msg.IsBodyHtml = false;
            msg.Priority = MailPriority.High;
            Excel.Application excelApp = new Excel.Application();
            if (excelApp.Version.ToString() == "15.0")
            {
                filename += ".xlsx";
            }
            else
            {
                filename += ".xls";
            }
            Attachment attachment = new Attachment(filename);
            msg.Attachments.Add(attachment);

            //Add the Creddentials
            SmtpClient client = new SmtpClient();
            client.Credentials = new NetworkCredential("furnorders@gmail.com", "sinol1978IT");
            client.Port = int.Parse("587");
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Send(msg);
            msg.Dispose();
        }

    }
}
