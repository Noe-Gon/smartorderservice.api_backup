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
                var billPocket = new BillPocketService().GetTokensByUserId(routeId);
                response = Request.CreateResponse(HttpStatusCode.OK, billPocket);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Uppsss...");
            }

            return response;
        }
    }
}
