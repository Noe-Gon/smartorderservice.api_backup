using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SmartOrderService.Models.DTO;
using SmartOrderService.Services;

namespace SmartOrderService.Controllers
{
    public class UserNoticeRechargeController : ApiController
    {
        [HttpGet, Route("api/UserNoticeRecharge/GetByBranch/{branchId}")]
        public HttpResponseMessage GetByBranch(int branchId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var userNoticeRecharge = new UserNoticeRechargeService().GetByBranch(branchId);
                response = Request.CreateResponse(HttpStatusCode.OK, userNoticeRecharge);
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

        [HttpGet]
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                var userNoticeRecharge = new UserNoticeRechargeService().Get(id);
                response = Request.CreateResponse(HttpStatusCode.OK, userNoticeRecharge);
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

        [HttpPost]
        public HttpResponseMessage Post(UserNoticeRechargeDto user)
        {
            HttpResponseMessage response;
            try
            {
                new UserNoticeRechargeService().Create(user);
                response = Request.CreateResponse(HttpStatusCode.Created, user);
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            return response;
        }

        [HttpPost]
        public HttpResponseMessage AssignRoutes(int id, List<int> routeIds)
        {
            HttpResponseMessage response;
            try
            {
                new UserNoticeRechargeService().AssignRoutes(id, routeIds);
                response = Request.CreateResponse(HttpStatusCode.OK, routeIds.Count());
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            return response;
        }

        [HttpPut]
        public HttpResponseMessage Put(int id, UserNoticeRechargeDto user)
        {
            HttpResponseMessage response;
            try
            {
                new UserNoticeRechargeService().Update(id, user);
                response = Request.CreateResponse(HttpStatusCode.OK, user);
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

        [HttpDelete]
        public HttpResponseMessage Delete(int id)
        {
            HttpResponseMessage response;
            try
            {
                new UserNoticeRechargeService().Deactivate(id);
                response = Request.CreateResponse(HttpStatusCode.OK, id);
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
