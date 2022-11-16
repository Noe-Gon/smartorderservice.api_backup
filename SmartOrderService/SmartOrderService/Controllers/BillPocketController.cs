using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.DTO;
using SmartOrderService.Models.Message;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class BillPocketController : ApiController
    {
        // GET: api/BillPocket
        public HttpResponseMessage Get([FromUri] int routeId)
        {
            HttpResponseMessage response;
            try
            {
                using (var service = BillPocketService.Create())
                {
                    var billPocket = service.GetTokensByUserId(routeId);
                    response = Request.CreateResponse(HttpStatusCode.OK, billPocket);
                }
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("api/BillPocket/Check")]
        public IHttpActionResult CheckBillPocketSales([FromUri]CheckBillPocketSalesRequest request)
        {
            try
            {
                using (var service = BillPocketService.Create())
                {
                    var response = service.CheckBillPocketSales(request);

                    if (response.Status)
                        return Content(HttpStatusCode.OK, response);

                    return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (InventoryInProgressException e)
            {
                return Content(HttpStatusCode.Conflict, ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (EntityNotFoundException e)
            {
                return Content(HttpStatusCode.NotFound, ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (ArgumentNullException e)
            {
                return Content(HttpStatusCode.Forbidden, ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    "Error no controlado", e.Message
                }));
            }
        }

        [HttpPost]
        [Route("api/BillPocket/SendReport")]
        public IHttpActionResult SendBillPocketReport(SendBillPocketReportRequest request)
        {
            try
            {
                using (var service = BillPocketService.Create())
                {
                    var response = service.SendBillPocketReport(request);

                    if (response.Status)
                        return Content(HttpStatusCode.OK, response);

                    return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (InventoryInProgressException e)
            {
                return Content(HttpStatusCode.Conflict, ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (EntityNotFoundException e)
            {
                return Content(HttpStatusCode.NotFound, ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (ArgumentNullException e)
            {
                return Content(HttpStatusCode.Forbidden, ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    "Error no controlado", e.Message
                }));
            }
        }
    }
}