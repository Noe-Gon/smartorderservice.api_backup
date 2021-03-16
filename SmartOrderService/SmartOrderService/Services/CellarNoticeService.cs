using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Models.DTO;
using SmartOrderService.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web;

namespace SmartOrderService.Services
{
    public class CellarNoticeService
    {

        private SmartOrderModel db = new SmartOrderModel();


        public CellarNoticeDto createNotice(int RouteId, DateTime Date, int userId) {


            var user_configs = db.so_user_notice_recharge_route
                .Where(x => x.routeId == RouteId && x.status)
                .Select(x => x.so_user_notice_recharge).ToList();

            if (!user_configs.Any())
          
                throw new NotFoundUsersConfigException();

            //send by config
            List<string> emails = new List<string>();

            foreach (var config in user_configs) {
                if (config.mail_enabled)
                    emails.Add(config.mail);
                
                
            }


            var routeName = db.so_route.Where(r => r.routeId == RouteId).FirstOrDefault().name;

            if(emails.Any())
            sendEmail(emails,"Recarga de ruta "+routeName,"la ruta va en camino al cedis");



            //save notice

            var datenow = DateTime.Now;

            var notice = new so_cellar_notice() {
                userId = userId,
                routeId = RouteId,
                date = Date,
                createdBy = userId,
                modifiedBy = userId,
                createdOn = datenow,
                modifiedOn = datenow,
                status = true,
            };

            db.so_cellar_notice.Add(notice);
            db.SaveChanges();

            CellarNoticeDto dto = new CellarNoticeDto() {

                CellarNoticeId = notice.cellar_noticeId,
                CreatedOn = Date.ToString(),
                RouteId = RouteId,
            };


            return dto;

        }

        private bool sendEmail(IEnumerable<string> to,string subject,string msgHtml)
        {
            try
            {

                var smtpClient = new SmtpClient();
                smtpClient.Port = Convert.ToInt32(MailSettings.SmtpPort);
                smtpClient.Host = MailSettings.SmtpHost;
                smtpClient.EnableSsl = false;
                //smtpClient.EnableSsl = true;
                smtpClient.Timeout = 1000000;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = true;

                var message = new MailMessage();
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = msgHtml;
                message.From = new MailAddress("WorkByCloudDS@bepensa.com");

                foreach (var email in to) {
                    message.To.Add(email);
                }

                //to.ForEach(mail => { message.To.Add(mail); });

                smtpClient.Send(message);

                var newThread = new Thread(delegate () { smtpClient.Send(message); });
                newThread.Start();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }

        }
    }


    public static class MailSettings
    {
        public static string UserName = ConfigurationManager.AppSettings["UserName"];

        public static string Password = ConfigurationManager.AppSettings["Password"];

        public static string SmtpHost = ConfigurationManager.AppSettings["SmtpHost"];

        public static string SmtpPort = ConfigurationManager.AppSettings["SmtpPort"];
    }

}