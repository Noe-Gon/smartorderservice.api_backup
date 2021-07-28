using SmartOrderService.Models.Email;
using SmartOrderService.Models.Requests;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace SmartOrderService.Services
{
    public class EmailService
    {
        public bool SendWellcomeEmail(WellcomeEmailRequest request)
        {
            try
            {
                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Content/Template/WellcomeEmail.html")))
                {
                    string body = reader.ReadToEnd();

                    body = body.Replace("{CustomerName}", request.CustomerName);
                    body = body.Replace("{TermsAndConsitionLink}", request.TermsAndConditionLink);
                    body = body.Replace("{CanceledLink}", request.CanceledLink);

                    var mailInfo = new SendAPIEmailrequest()
                    {
                        To = request.CustomerEmail,
                        Subject = "¡Gracias por ser cliente Bepensa!",
                        Body = body
                    };

                    DummySendEmail(mailInfo);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void DummySendEmail(SendAPIEmailrequest request)
        {
            MailMessage mmsg = new MailMessage();

            mmsg.To.Add(request.To);
            mmsg.Subject = request.Subject;
            mmsg.SubjectEncoding = System.Text.Encoding.UTF8;

            mmsg.Body = request.Body;
            mmsg.BodyEncoding = System.Text.Encoding.UTF8;
            mmsg.IsBodyHtml = true;

            mmsg.From = new MailAddress("kevmkc2@gmail.com");

            SmtpClient client = new SmtpClient();

            client.Credentials = new NetworkCredential("kevmkc2@gmail.com", "kevinblablabla");

            client.Port = 587;
            client.EnableSsl = true;

            client.Host = "smtp.gmail.com";

            try
            {
                client.Send(mmsg);
            }
            catch (Exception e)
            {
                var error = e.Message;
            }
        }
    }
}