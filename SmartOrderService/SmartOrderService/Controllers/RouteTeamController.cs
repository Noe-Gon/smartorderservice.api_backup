using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartOrderService.Services
{
    public class RouteTeamController : ApiController
    {

        [HttpGet, Route("api/routeam/travelstatus")]
        public HttpResponseMessage CheckTravelStatus(int userId)
        {
            HttpResponseMessage response;
            try
            {
                RouteTeamService service = new RouteTeamService();
                bool result = service.CheckCurrentTravelState(userId);
                response = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (RelatedDriverNotFoundException)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, false);
            }
            catch (InventoryEmptyException)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, false);
            }
            catch (InventoryInProgressException)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, false);
            }
            catch (InventoryNotOpenException)
            {
                response = Request.CreateResponse((HttpStatusCode)211, false);
            }
            catch (InventoryNotClosedException)
            {
                response = Request.CreateResponse((HttpStatusCode)212, false);
            }
            catch (InventoryNotClosedByUserException)
            {
                response = Request.CreateResponse((HttpStatusCode)213, false);
            }
            catch (Exception)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, false);
            }
            return response;
        }

        [HttpGet, Route("api/routeam/workdaystatus")]
        public HttpResponseMessage CheckWorkDayStatus(int userId)
        {
            HttpResponseMessage response;
            RouteTeamService service = new RouteTeamService();
            DateTime today = DateTime.Today;
            try
            {
                bool isStarted = service.checkDriverWorkDay(userId);
                response = Request.CreateResponse(HttpStatusCode.Accepted, isStarted);
            }
            catch (WorkdayNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict,true);
            }
            catch (NotSupportedException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, false);
            }
            catch (RelatedDriverNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, false);
            }
            return response;
        }

        [HttpGet, Route("api/routeam/travelclosestatus")]
        public HttpResponseMessage CheckTravelClosingStatus([FromUri]InventoryRequest request)
        {
            HttpResponseMessage response;
            if (!request.InventoryId.HasValue)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "Falta el parametro InventoryId");
                return response;
            }
            try
            {
                RouteTeamService routeTeamService = new RouteTeamService();
                response = Request.CreateResponse(HttpStatusCode.Accepted,routeTeamService.CheckTravelClosingStatus(request.UserId,request.InventoryId.Value));
            }
            catch (InventoryEmptyException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, false);
            }
            catch (InventoryNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict,false);
            }
            catch(Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict,false);
            }
            return response;
        }

        [HttpGet, Route("api/routeam/workdayclosestatus")]
        public HttpResponseMessage CheckWorkDayClosingStatus(int userId)
        {
            HttpResponseMessage response;
            try
            {
                RouteTeamService routeTeamService = new RouteTeamService();
                response = Request.CreateResponse(HttpStatusCode.Accepted,routeTeamService.CheckWorkDayClosingStatus(userId));
            }
            catch (WorkdayNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict,false);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, false);
            }
            return response;
        }

        [HttpGet]
        [Route("api/routeam")]
        public IHttpActionResult GetRouteTeam([FromUri] int? routeId)
        {
            try
            {
                if (routeId == null || routeId <= 0)
                    return Content(HttpStatusCode.NotFound, ResponseBase<List<GetRouteTeamResponse>>.Create(new List<string>()
                    {
                        "Es necesario proporcionar una ruta valida"
                    }));

                RouteTeamService routeTeamService = new RouteTeamService();
                var response = routeTeamService.GetRouteTeam(routeId.Value);

                if (response.Status)
                    return Content(HttpStatusCode.OK, response);

                return Content(HttpStatusCode.BadRequest, response);
            }
            catch (EntityNotFoundException e)
            {
                return Content(HttpStatusCode.NotFound, ResponseBase<List<GetRouteTeamResponse>>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<List<GetRouteTeamResponse>>.Create(new List<string>()
                {
                    "Error interno", e.Message
                }));
            }
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}