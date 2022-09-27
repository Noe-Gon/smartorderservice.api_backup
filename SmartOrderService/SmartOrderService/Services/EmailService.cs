using RestSharp;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

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

                    var mailInfo = new APIEmailSendEmailRequest()
                    {
                        To = request.CustomerEmail,
                        Subject = "¡Gracias por ser cliente Bepensa!",
                        Body = body
                    };

                    APIEmailSendEmail(mailInfo);
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

                if (request.dtTicket.Columns.Count > 0 && request.dtTicket.Rows.Count > 0)
                {
                    lTienePromociones = true;
                }

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
                    string tdBodyPromociones = "";

                    //Orders
                    if (request.Order == null)
                        body = body.Replace("{OrderTableTamplate}", "");
                    else if(request.Order.OrderDetail.Count() == 0)
                        body = body.Replace("{OrderTableTamplate}", "");
                    else
                    {
                        using (StreamReader readerTemplate = new StreamReader(HttpContext.Current.Server.MapPath("~/Content/Template/OrderTableTemplate.html")))
                        {
                            string bodyOrderTemplate = readerTemplate.ReadToEnd();

                            body = body.Replace("{OrderTableTamplate}", bodyOrderTemplate);
                            body = body.Replace("{OrderDeliveryDate}", request.Order.DeliveryDate.ToString("dd/MM/yyyy"));
                            string tableData = "";
                            double totalPrice = 0;
                            int index = 0;
                            int totalOrderProductsSold = 0;

                            foreach (var item in request.Order.OrderDetail)
                            {
                                index++;
                                totalOrderProductsSold += item.Amount;
                                totalPrice += item.TotalPrice;
                                tableData += "<tr><td>" + index + ") " + item.ProductName + "</td>";
                                tableData += "<td>" + item.Amount + "</td>";
                                tableData += "<td>" + item.UnitPrice.ToString("0.00") + "</td>";
                                tableData += "<td>" + item.TotalPrice.ToString("0.00") + "</td></tr>";
                            }

                            body = body.Replace("{OrderTdBody}", tableData);
                            body = body.Replace("{OrderTotalProductsSold}", totalOrderProductsSold.ToString());
                            body = body.Replace("{OrderTotalPrice}", totalPrice.ToString("0.00"));

                            if (string.IsNullOrEmpty(request.ReferenceCode))
                                body = body.Replace("id='reference_code'", "id='reference_code' style='display: none'");
                            else
                            {
                                body = body.Replace("id='reference_code'", "id='reference_code' style='text-align: center; margin-bottom: 5px;'");
                                body = body.Replace("{OrderReferenceCode}", request.ReferenceCode);
                            }
                                
                        }
                    }

                    if(request.Sales.Count() == 0)
                    {
                        body = body.Replace("{SaleTableStyle}", "display: none;");
                    }
                    else
                    {
                        body = body.Replace("{SaleTableStyle}", "");
                    }

                    int totalProductsSold = 0;
                    int totalBoxesSold = 0;
                    int totalCountProductSold = 0;
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
                        totalCountProductSold += row.Amount;
                        total += row.TotalPrice;
                    }

                    if (lTienePromociones)
                    {
                        int totalPromos = 0;
                        body = body.Replace("id='promociones' style='display:none'", "id='promociones' style='display:'");
                        body = body.Replace("id='lblpromociones' style='display:none'", "id='lblpromociones' style='display:'");
                        foreach (DataRow row in request.dtTicket.Rows)
                        {
                            tdBodyPromociones += "<tr><td style='width:400px'>" + row["id"] + ") " + row["name_product"].ToString() + "</td>";
                            tdBodyPromociones += "<td style='width:100px'>" + row["amount"].ToString() + "</td>";
                            tdBodyPromociones += "<td style='width:100px'>" + "$" + String.Format("{0:0.00}", row["sale_price"]) + "</td>";
                            tdBodyPromociones += "<td style='width:100px'>" + "$" + String.Format("{0:0.00}", 0) + "</td></tr>";
                            totalPromos += (int)row["amount"];
                        }

                        body = body.Replace("{TdBodyPromociones}", tdBodyPromociones);
                        body = body.Replace("{TotalPromos}", totalPromos.ToString());
                    }

                    body = body.Replace("{TdBody}", tdBody);
                    body = body.Replace("{TotalProductsSold}", totalCountProductSold.ToString());
                    body = body.Replace("{TotalBoxesSold}", totalBoxesSold.ToString());
                    body = body.Replace("{TotalPrice}", String.Format("{0:0.00}", total));

                    

                    var mailInfo = new APIEmailSendEmailRequest()
                    {
                        To = request.CustomerEmail,
                        Subject = "¡Gracias por ser cliente Bepensa!",
                        Body = body
                    };

                    APIEmailSendEmail(mailInfo);
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
                bool lTienePromociones = false;
                string sRutaPlantilla = "~/Content/Template/CancelTicketDigitalEmail.html";

                if (request.dtTicket.Columns.Count > 0 && request.dtTicket.Rows.Count > 0)
                {
                    lTienePromociones = true;
                }

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

                    //Orders
                    if (request.Order == null)
                        body = body.Replace("{OrderTableTamplate}", "");
                    else
                    {
                        using (StreamReader readerTemplate = new StreamReader(HttpContext.Current.Server.MapPath("~/Content/Template/OrderTableTemplate.html")))
                        {
                            string bodyOrderTemplate = readerTemplate.ReadToEnd();

                            body = body.Replace("{OrderTableTamplate}", bodyOrderTemplate);
                            body = body.Replace("{OrderDeliveryDate}", request.Order.DeliveryDate.ToString("dd/MM/yyyy"));
                            string tableData = "";
                            double totalPrice = 0;
                            int index = 0;
                            int totalOrderProductsSold = 0;

                            foreach (var item in request.Order.OrderDetail)
                            {
                                index++;
                                totalOrderProductsSold += item.Amount;
                                totalPrice += item.TotalPrice;
                                tableData += "<tr><td>" + index + ") " + item.ProductName + "</td>";
                                tableData += "<td>" + item.Amount + "</td>";
                                tableData += "<td>" + item.UnitPrice.ToString("0.00") + "</td>";
                                tableData += "<td>" + item.TotalPrice.ToString("0.00") + "</td></tr>";
                            }

                            body = body.Replace("{OrderTdBody}", tableData);
                            body = body.Replace("{OrderTotalProductsSold}", totalOrderProductsSold.ToString());
                            body = body.Replace("{OrderTotalPrice}", totalPrice.ToString("0.00"));

                            if (string.IsNullOrEmpty(request.ReferenceCode))
                                body = body.Replace("id='reference_code'", "id='reference_code' style='display: none'");
                            else
                            {
                                body = body.Replace("id='reference_code'", "id='reference_code' style='text-align: center; margin-bottom: 5px;'");
                                body = body.Replace("{OrderReferenceCode}", request.ReferenceCode);
                            }
                        }
                    }

                    if (request.Sales.Count() == 0)
                    {
                        body = body.Replace("{SaleTableStyle}", "display: none;");
                    }
                    else
                    {
                        body = body.Replace("{SaleTableStyle}", "");
                    }

                    string tdBody = "";
                    int totalProductsSold = 0;
                    int totalCountProductSold = 0;
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
                        totalCountProductSold += row.Amount;
                        total += row.TotalPrice;
                    }

                    string tdBodyPromociones = "";
                    if (lTienePromociones)
                    {
                        int totalPromos = 0;
                        body = body.Replace("id='promociones' style='display:none'", "id='promociones' style='display:'");
                        body = body.Replace("id='lblpromociones' style='display:none'", "id='lblpromociones' style='display:'");
                        foreach (DataRow row in request.dtTicket.Rows)
                        {
                            tdBodyPromociones += "<tr><td style='width:400px'>" + row["id"] + ") " + row["name_product"].ToString() + "</td>";
                            tdBodyPromociones += "<td style='width:100px'>" + row["amount"].ToString() + "</td>";
                            tdBodyPromociones += "<td style='width:100px'>" + "$" + String.Format("{0:0.00}", 0) + "</td>";
                            tdBodyPromociones += "<td style='width:100px'>" + "$" + String.Format("{0:0.00}", 0) + "</td></tr>";
                            totalPromos += (int)row["amount"];
                        }

                        body = body.Replace("{TdBodyPromociones}", tdBodyPromociones);
                        body = body.Replace("{TotalPromos}", totalPromos.ToString());
                    }

                    body = body.Replace("{TdBody}", tdBody);
                    body = body.Replace("{TotalProductsSold}", totalCountProductSold.ToString());
                    body = body.Replace("{TotalBoxesSold}", totalBoxesSold.ToString());
                    body = body.Replace("{TotalPrice}", String.Format("{0:0.00}", total));

                    var mailInfo = new APIEmailSendEmailRequest()
                    {
                        To = request.CustomerEmail,
                        Subject = "¡Gracias por ser cliente Bepensa!",
                        Body = body
                    };

                    APIEmailSendEmail(mailInfo);
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

                    var mailInfo = new APIEmailSendEmailRequest()
                    {
                        To = request.CustomerEmail,
                        Subject = "Solicitud envío de ticket de compra",
                        Body = body
                    };

                    APIEmailSendEmail(mailInfo);
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

        public ResponseBase<MsgResponseBase> SendOrderTicket(SendOrderTicketRequest request)
        {
            try
            {
                string template = request.Status ? "~/Content/Template/OrderTicket.html" : "~/Content/Template/CancelOrderTicket.html";

                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath(template)))
                {
                    string body = reader.ReadToEnd();
                    string tableData = "";

                    body = body.Replace("{CustomerName}", request.CustomerName);
                    body = body.Replace("{Date}", request.Date.ToString("dd/MMM/yy hh:mmtt"));
                    body = body.Replace("{CustomerFullName}", request.CustomerFullName);
                    body = body.Replace("{RouteAddress}", request.RouteAddress);
                    body = body.Replace("{SellerName}", request.SallerName);
                    body = body.Replace("{DeliveryDate}", request.DeliveryDate.ToString("dd/MM/yyyy"));

                    int totalProductsSold = 0;
                    double totalPrice = 0;
                    int index = 0;
                    foreach (var item in request.Items)
                    {
                        index++;
                        totalProductsSold += item.Amount;
                        totalPrice += item.TotalPrice;
                        tableData += "<tr><td>" + index + ") " + item.ProductName + "</td>";
                        tableData += "<td>" + item.Amount + "</td>";
                        tableData += "<td>" + item.UnitPrice.ToString("0.00") + "</td>";
                        tableData += "<td>" + item.TotalPrice.ToString("0.00") + "</td></tr>";
                    }

                    body = body.Replace("{TdBody}", tableData);
                    body = body.Replace("{TotalProductsSold}", totalProductsSold.ToString());
                    body = body.Replace("{TotalPrice}", totalPrice.ToString("0.00"));

                    if (string.IsNullOrEmpty(request.ReferenceCode))
                        body = body.Replace("id='reference_code'", "id='reference_code' style='display: none'");
                    else
                        body = body.Replace("{ReferenceCode}", request.ReferenceCode);

                    APIEmailSendEmail(new APIEmailSendEmailRequest
                    {
                        Body = body,
                        Subject = request.Status ? "Hemos recibido tu pedido en Bepensa" : "¡Gracias por ser cliente Bepensa!",
                        To = request.CustomerMail
                    });
                }

                return ResponseBase<MsgResponseBase>.Create(new MsgResponseBase()
                {
                    Msg = "Se ha enviadó con exito"
                });
            }
            catch (Exception e)
            {
                return ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    e.Message
                });
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

            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
                return;

            throw new Exception(RestResponse.ErrorMessage);
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