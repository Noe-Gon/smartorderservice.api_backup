using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class RouteTeamInventoryAvailableController : ApiController
    {

        [HttpGet, Route("api/routeteaminventory")]
        public HttpResponseMessage GetRouteTeamInventory(int inventoryId)
        {
            HttpResponseMessage response;
            RouteTeamInventoryAvailableService service = new RouteTeamInventoryAvailableService();
            var inventoryResult = service.GetRouteTeamInventories(inventoryId);
            response = Request.CreateResponse(HttpStatusCode.OK,inventoryResult);
            return response;
        }
    }
}
