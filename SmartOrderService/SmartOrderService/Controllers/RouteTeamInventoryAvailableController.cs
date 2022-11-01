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

        [HttpGet, Route("api/v2/routeteaminventory")]
        public HttpResponseMessage GetRouteTeamInventoryv2(int inventoryId)
        {
            HttpResponseMessage response;
            RouteTeamInventoryAvailableService service = new RouteTeamInventoryAvailableService();
            var inventoryResult = service.GetRouteTeamInventoriesv2(inventoryId);
            response = Request.CreateResponse(HttpStatusCode.OK, inventoryResult);
            return response;
        }
    }
}
