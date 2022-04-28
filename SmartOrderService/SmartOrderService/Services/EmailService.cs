﻿using SmartOrderService.Models.Email;
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
using RestSharp;
using System.Data;

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
                bool lTienePromociones = false;
                string sRutaPlantilla = "~/Content/Template/TicketDigitalEmail.html";

                //if(request.dtTicket.Columns.Count > 0 && request.dtTicket.Rows.Count > 0)
                //{
                //    lTienePromociones = true;
                //}

                    using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath(sRutaPlantilla)))
                {
                    string body = reader.ReadToEnd();

                    body = body.Replace("{CustomerName}", request.CustomerName);
                    body = body.Replace("{CustomerFullName}", request.CustomerFullName);
                    body = body.Replace("{Date}", request.Date.ToString("dd/MMM/yy hh:mmtt"));
                    body = body.Replace("{RouteAddress}", request.RouteAddress);
                    body = body.Replace("{SellerName}", request.SellerName);
                    body = body.Replace("{CancelTicketLink}", request.CancelTicketLink);

                    if (request.PaymentMethod == null || !string.IsNullOrEmpty(request.PaymentMethod))
                        body = body.Replace("{PaymentMethod}", "");
                    else
                        body = body.Replace("{PaymentMethod}", "Forma de pago: " + request.PaymentMethod);

                    string tdBody = "";
                    //string tdBodyPromociones = "";

                    int totalProductsSold = 0;
                    int totalBoxesSold = 0;
                    double total = 0.0;
                    //Make Table
                    foreach (var row in request.Sales)
                    {
                        totalProductsSold++;
                        tdBody += "<tr><td>" + totalProductsSold + ") " + row.ProductName + "</td>";
                        tdBody += "<td>" + row.Amount + "</td>";
                        tdBody += "<td>" + "$" + String.Format("{0:0.00}", row.UnitPrice) + "</td>";
                        tdBody += "<td>" + "$" + String.Format("{0:0.00}", row.TotalPrice) + "</td></tr>";
                        totalBoxesSold += row.Amount;
                        total += row.TotalPrice;
                    }

                    //if (lTienePromociones)
                    //{
                    //    int totalPromos = 0;
                    //    body = body.Replace("id='promociones' style='display:none'", "id='promociones' style='display:'");
                    //    body = body.Replace("id='lblpromociones' style='display:none'", "id='lblpromociones' style='display:'");
                    //    foreach (DataRow row in request.dtTicket.Rows)
                    //    {
                    //        tdBodyPromociones += "<tr><td style='width:400px'>" + row["id"] + ") " + row["name_product"].ToString() + "</td>";
                    //        tdBodyPromociones += "<td style='width:100px'>" + row["amount"].ToString() + "</td>";
                    //        tdBodyPromociones += "<td style='width:100px'>" + "$" + String.Format("{0:0.00}", 0) + "</td>";
                    //        tdBodyPromociones += "<td style='width:100px'>" + "$" + String.Format("{0:0.00}", 0) + "</td></tr>";
                    //        totalPromos += (int)row["amount"];
                    //    }

                    //    body = body.Replace("{TdBodyPromociones}", tdBodyPromociones);
                    //    body = body.Replace("{TotalPromos}", totalPromos.ToString());
                    //}

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
                //bool lTienePromociones = false;
                string sRutaPlantilla = "~/Content/Template/CancelTicketDigitalEmail.html";

                //if (request.dtTicket.Columns.Count > 0 && request.dtTicket.Rows.Count > 0)
                //{
                //    lTienePromociones = true;
                //}

                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath(sRutaPlantilla)))
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

                    //string tdBodyPromociones = "";
                    //if (lTienePromociones)
                    //{
                    //    int totalPromos = 0;
                    //    body = body.Replace("id='promociones' style='display:none'", "id='promociones' style='display:'");
                    //    body = body.Replace("id='lblpromociones' style='display:none'", "id='lblpromociones' style='display:'");
                    //    foreach (DataRow row in request.dtTicket.Rows)
                    //    {
                    //        tdBodyPromociones += "<tr><td style='width:400px'>" + row["id"] + ") " + row["name_product"].ToString() + "</td>";
                    //        tdBodyPromociones += "<td style='width:100px'>" + row["amount"].ToString() + "</td>";
                    //        tdBodyPromociones += "<td style='width:100px'>" + "$" + String.Format("{0:0.00}", 0) + "</td>";
                    //        tdBodyPromociones += "<td style='width:100px'>" + "$" + String.Format("{0:0.00}", 0) + "</td></tr>";
                    //        totalPromos += (int)row["amount"];
                    //    }

                    //    body = body.Replace("{TdBodyPromociones}", tdBodyPromociones);
                    //    body = body.Replace("{TotalPromos}", totalPromos.ToString());
                    //}

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

        public ResponseBase<SendRemovalRequestEmailResponse> SendRemovalRequestEmail(SendRemovalRequestEmailRequest request)
        {
            try
            {
                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/Content/Template/RemovalRequestEmail.html")))
                {
                    string body = reader.ReadToEnd();
                    string tableData = "";

                    foreach (var info in request.Table)
                    {
                        tableData += "<tr><td style='border: 1px solid black;'>" + info.CFECode + "</td>";
                        tableData += "<td style='border: 1px solid black;'>" + info.ConsumerName + "</td>";
                        tableData += "<td style='border: 1px solid black;'>" + info.Route + "</td>";
                        tableData += "<td style='border: 1px solid black;'>" + info.ImpulsorName + "</td>";
                        tableData += "<td style='border: 1px solid black;'>" + info.Reason + "</td>";
                        tableData += "<td style='border: 1px solid black;'>" + info.Date.ToString("dd/MM/yyyy") + "</td></tr>";

                    }
                    body = body.Replace("{TableData}", tableData);

                    var routeIds = request.Table.GroupBy(x => x.Route).Select(x => x.Key);

                    APIEmailSendEmailToManyUsers(new APIEmailSendEmailToManyUsersRequest
                    {
                        Body = body,
                        Subject = "Solicitud de baja consumidores ruta: " + string.Join(",", routeIds),
                        To = request.LeaderEmail
                    });
                }

                return ResponseBase<SendRemovalRequestEmailResponse>.Create(new SendRemovalRequestEmailResponse
                {
                    Msg = "La notificación se envió con exito"
                });
            }
            catch (Exception e)
            {
                return ResponseBase<SendRemovalRequestEmailResponse>.Create(new List<string>()
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

        public void APIEmailSendEmail(APIEmailSendEmailRequest request)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIEmail"]);
            var requesto = new RestRequest("api/SendEmail", Method.POST);
            requesto.RequestFormat = DataFormat.Json;

            requesto.AddJsonBody(request);

            var RestResponse = client.Execute(requesto);
        }

        public void APIEmailSendEmailToManyUsers(APIEmailSendEmailToManyUsersRequest request)
        {

            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["APIEmail"]);
            var requesto = new RestRequest("api/SendEmailToManyUsers", Method.POST);
            requesto.RequestFormat = DataFormat.Json;

            requesto.AddJsonBody(request);

            var RestResponse = client.Execute(requesto);
        }
    }
}