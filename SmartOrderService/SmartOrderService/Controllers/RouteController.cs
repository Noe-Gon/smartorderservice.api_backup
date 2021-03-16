using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SmartOrderService.Services;

namespace SmartOrderService.Controllers
{
    public class RouteController : ApiController
    {
        [HttpGet, Route("api/Route/GetByBranch/{branchId}")]
        public HttpResponseMessage GetByBranch(int branchId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var routes = new RouteService().GetByBranch(branchId);
                response = Request.CreateResponse(HttpStatusCode.OK, routes);
            }
            catch (KeyNotFoundException ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            return response;
        }

        [HttpGet, Route("api/Route/GetByUser/{userNoticeRechargeId}")]
        public HttpResponseMessage GetByUserNoticeRecharge(int userNoticeRechargeId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var routes = new RouteService().GetByUserNoticeRecharge(userNoticeRechargeId);
                response = Request.CreateResponse(HttpStatusCode.OK, routes);
            }
            catch (KeyNotFoundException ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            return response;
        }

        [HttpGet, Route("api/Route/GetByBranch/{branchId}/Type/{type}")]
        public HttpResponseMessage DeliveryTypeGetByBranch(int branchId, int type)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var routes = new RouteService().GetByBranch(branchId, type);
                response = Request.CreateResponse(HttpStatusCode.OK, routes);
            }
            catch (KeyNotFoundException ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            return response;
        }
    }
}
