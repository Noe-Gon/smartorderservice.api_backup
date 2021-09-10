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
using SmartOrderService.Models.Responses;
using System.Net.Mime;

namespace SmartOrderService.Services
{
    public class EmailService
    {
        public ResponseBase<SendWellcomeEmailResponse> SendWellcomeEmail(WellcomeEmailRequest request)
        {
            try
            {
                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Content/Template/WellcomeEmail.html")))
                {
                    string body = reader.ReadToEnd();

                    body = body.Replace("{CustomerName}", request.CustomerName);
                    body = body.Replace("{TermsAndConditionLink}", request.TermsAndConditionLink);
                    body = body.Replace("{CanceledLink}", request.CanceledLink);

                    var mailInfo = new SendAPIEmailrequest()
                    {
                        To = request.CustomerEmail,
                        Subject = "¡Gracias por ser cliente Bepensa!",
                        Body = body
                    };

                    DummySendEmail(mailInfo);
                }

                return ResponseBase<SendWellcomeEmailResponse>.Create(new SendWellcomeEmailResponse
                {
                    Msg = "Se envió correctamente"
                });
            }
            catch (Exception e)
            {
                return ResponseBase<SendWellcomeEmailResponse>.Create(new List<string>()
                {
                    e.Message
                });
            }
        }

        public ResponseBase<SendTicketDigitalEmailResponse> SendTicketDigitalEmail(SendTicketDigitalEmailRequest request)
        {
            try
            {
                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Content/Template/TicketDigitalEmail.html")))
                {
                    string body = reader.ReadToEnd();

                    body = body.Replace("{CustomerName}", request.CustomerName);
                    body = body.Replace("{CustomerFullName}", request.CustomerFullName);
                    body = body.Replace("{Date}", request.Date.ToString("dd/MMM/yy hh:mmtt"));
                    body = body.Replace("{RouteAddress}", request.RouteAddress);
                    body = body.Replace("{SellerName}", request.SellerName);

                    if (request.PaymentMethod == null || !string.IsNullOrEmpty(request.PaymentMethod))
                        body = body.Replace("{PaymentMethod}", "");
                    else
                        body = body.Replace("{PaymentMethod}", "Forma de pago: " + request.PaymentMethod);

                    string tdBody = "";
                    int totalProductsSold = 0;
                    int totalBoxesSold = 0;
                    double total = 0.0;
                    //Make Table
                    foreach (var row in request.Sales)
                    {
                        totalProductsSold++;
                        tdBody += "<tr><td>" + totalProductsSold + ") " + row.ProductName + "</td>";
                        tdBody += "<td>" + row.Amount + "</td>";
                        tdBody += "<td>" + String.Format("{0:0.00}", row.UnitPrice) + "</td>";
                        tdBody += "<td>" + String.Format("{0:0.00}", row.TotalPrice) + "</td></tr>";
                        totalBoxesSold += row.Amount;
                        total += row.TotalPrice;
                    }

                    body = body.Replace("{TdBody}", tdBody);
                    body = body.Replace("{TotalProductsSold}", totalProductsSold.ToString());
                    body = body.Replace("{TotalBoxesSold}", totalBoxesSold.ToString());
                    body = body.Replace("{TotalPrice}", String.Format("{0:0.00}", total));

                    var mailInfo = new SendAPIEmailrequest()
                    {
                        To = request.CustomerEmail,
                        Subject = "¡Gracias por ser cliente Bepensa!",
                        Body = body
                    };

                    DummySendEmail(mailInfo);
                }

                return ResponseBase<SendTicketDigitalEmailResponse>.Create(new SendTicketDigitalEmailResponse
                {
                    Msg = "El correo se envió correctamente"
                });
            }
            catch (Exception e)
            {
                return ResponseBase<SendTicketDigitalEmailResponse>.Create(new List<string>()
                {
                    e.Message
                });
            }
        }

        public ResponseBase<SendTicketDigitalEmailResponse> SendCancelTicketDigitalEmail(SendCancelTicketDigitalEmailRequest request)
        {
            try
            {
                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Content/Template/CancelTicketDigitalEmail.html")))
                {
                    string body = reader.ReadToEnd();

                    body = body.Replace("{CustomerName}", request.CustomerName);
                    body = body.Replace("{CustomerFullName}", request.CustomerFullName);
                    body = body.Replace("{Date}", request.Date.ToString("dd/MMM/yy hh:mmtt"));
                    body = body.Replace("{RouteAddress}", request.RouteAddress);
                    body = body.Replace("{SellerName}", request.SellerName);
                    body = body.Replace("{CancelDate}", request.Date.ToString("dd/MMM/yy"));

                    if (request.PaymentMethod == null || !string.IsNullOrEmpty(request.PaymentMethod))
                        body = body.Replace("{PaymentMethod}", "");
                    else
                        body = body.Replace("{PaymentMethod}", "Forma de pago: " + request.PaymentMethod);

                    string tdBody = "";
                    int totalProductsSold = 0;
                    int totalBoxesSold = 0;
                    double total = 0.0;
                    //Make Table
                    foreach (var row in request.Sales)
                    {
                        totalProductsSold++;
                        tdBody += "<tr><td>" + totalProductsSold + ") " + row.ProductName + "</td>";
                        tdBody += "<td>" + row.Amount + "</td>";
                        tdBody += "<td>" + String.Format("{0:0.00}", row.UnitPrice) + "</td>";
                        tdBody += "<td>" + String.Format("{0:0.00}", row.TotalPrice) + "</td></tr>";
                        totalBoxesSold += row.Amount;
                        total += row.TotalPrice;
                    }

                    body = body.Replace("{TdBody}", tdBody);
                    body = body.Replace("{TotalProductsSold}", totalProductsSold.ToString());
                    body = body.Replace("{TotalBoxesSold}", totalBoxesSold.ToString());
                    body = body.Replace("{TotalPrice}", String.Format("{0:0.00}", total));

                    var mailInfo = new SendAPIEmailrequest()
                    {
                        To = request.CustomerEmail,
                        Subject = "¡Gracias por ser cliente Bepensa!",
                        Body = body
                    };

                    DummySendEmail(mailInfo);
                }

                return ResponseBase<SendTicketDigitalEmailResponse>.Create(new SendTicketDigitalEmailResponse
                {
                    Msg = "El correo se envió correctamente"
                });
            }
            catch (Exception e)
            {
                return ResponseBase<SendTicketDigitalEmailResponse>.Create(new List<string>()
                {
                    e.Message
                });
            }
        }

        public ResponseBase<SendReactivationTicketDigitalResponse> SendReactivationTicketDigital(SendReactivationTicketDigitalRequest request)
        {
            try
            {
                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Content/Template/ReactivationTicketDigitalEmail.html")))
                {
                    string body = reader.ReadToEnd();

                    body = body.Replace("{CustomerName}", request.CustomerName);
                    body = body.Replace("{TermsAndConditionLink}", request.TermsAndConditionLink);

                    var mailInfo = new SendAPIEmailrequest()
                    {
                        To = request.CustomerEmail,
                        Subject = "Solicitud envío de ticket de compra",
                        Body = body
                    };

                    DummySendEmail(mailInfo);
                }

                return ResponseBase<SendReactivationTicketDigitalResponse>.Create(new SendReactivationTicketDigitalResponse()
                {
                    Msg = "Se envió correctamente"
                });
            }
            catch (Exception e)
            {
                return ResponseBase<SendReactivationTicketDigitalResponse>.Create(new List<string>()
                {
                    e.Message
                });
            }
        }

        public void DummySendEmail(SendAPIEmailrequest request)
        {
            MailMessage mmsg = new MailMessage();

            mmsg.To.Add(request.To);
            mmsg.Subject = request.Subject;
            mmsg.SubjectEncoding = System.Text.Encoding.UTF8;

            //Add image
            Attachment att = new Attachment(HttpContext.Current.Server.MapPath("~/Src/bepensa.png"));
            att.ContentDisposition.Inline = true;
            att.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
            att.ContentId = "Bepensa";
            att.ContentType.MediaType = "image/png";
            att.ContentType.Name = Path.GetFileName(HttpContext.Current.Server.MapPath("~/Src/bepensa.png"));
            request.Body = request.Body.Replace("{image}", "<img class=\"image\" src=\"cid:Bepensa\" />");

            mmsg.Body = request.Body;
            mmsg.BodyEncoding = System.Text.Encoding.UTF8;
            mmsg.IsBodyHtml = true;
            mmsg.Attachments.Add(att);

            mmsg.From = new MailAddress("bepensafullpotentialaws@walook.com.mx");

            SmtpClient client = new SmtpClient();

            client.Credentials = new NetworkCredential("AKIA4VWPJ4MQA5N5FLVM", "BE7TsEtOBV/9SIIFTZ6r9hDvg8HWTWbvyu/dRgXRvenz");

            client.Port = 587;
            client.EnableSsl = true;

            client.Host = "email-smtp.us-east-2.amazonaws.com";

            try
            {
                client.Send(mmsg);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}