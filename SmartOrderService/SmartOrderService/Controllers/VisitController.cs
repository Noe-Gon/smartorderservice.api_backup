using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using System.Web.Http;

using SmartOrderService.Services;
using SmartOrderService.CustomExceptions;

namespace SmartOrderService.Controllers
{
    public class VisitController : ApiController
    {
        // GET: api/Route
        public HttpResponseMessage Get([FromUri] VisitRequest request)
        {
            
            WorkdayService service = new WorkdayService();
            HttpResponseMessage response;
            try
            {
                var visits = new VisitService().getVisits(request.UserId);
                response = Request.CreateResponse(HttpStatusCode.OK, visits);
            }
            catch (NoUserFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Usuario no registrado");
            }
            catch (InventoryEmptyException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotAcceptable, "no tiene inventario para trabajar");
            }

            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Uppsss...");
            }

            return response;
        }

        [HttpGet]
        [Route("api/TeamVisit")]
        public HttpResponseMessage GetTeamVisit([FromUri] VisitRequest request)
        {

            WorkdayService service = new WorkdayService();
            HttpResponseMessage response;
            try
            {
                //var visits = new VisitService().getTeamVisits(request.UserId);

                //dummy
                var visitsDummy = new List<GetTeamVisitResponse>();
                visitsDummy.Add(new GetTeamVisitResponse
                {
                    UserId = 2792,
                    CustomerId = 161589,
                    Scanned = true,
                    CheckIn = DateTime.Now.AddHours(-2),
                    CheckOut = DateTime.Now.AddHours(-1),
                    LatitudeIn = 21.0204326,
                    LatitudeOut = 21.0204324,
                    LongitudeIn = 21.0204326,
                    LongitudeOut = -89.6023618,
                    ReasonFailed = null,
                    VisitId = 19573037
                });
                visitsDummy.Add(new GetTeamVisitResponse
                {
                    UserId = 2792,
                    CustomerId = 161660,
                    Scanned = false,
                    CheckIn = null,
                    CheckOut = null,
                    LatitudeIn = null,
                    LatitudeOut = null,
                    LongitudeIn = null,
                    LongitudeOut = null,
                    ReasonFailed = null
                });
                visitsDummy.Add(new GetTeamVisitResponse
                {
                    UserId = 2792,
                    CustomerId = 162013,
                    Scanned = false,
                    CheckIn = null,
                    CheckOut = null,
                    LatitudeIn = null,
                    LatitudeOut = null,
                    LongitudeIn = null,
                    LongitudeOut = null,
                    ReasonFailed = null
                });
                visitsDummy.Add(new GetTeamVisitResponse
                {
                    UserId = 2792,
                    CustomerId = 162145,
                    Scanned = false,
                    CheckIn = null,
                    CheckOut = null,
                    LatitudeIn = null,
                    LatitudeOut = null,
                    LongitudeIn = null,
                    LongitudeOut = null,
                    ReasonFailed = null
                });

                response = Request.CreateResponse(HttpStatusCode.OK, visitsDummy);
            }
            catch (NoUserFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Usuario no registrado");
            }
            catch (InventoryEmptyException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotAcceptable, "no tiene inventario para trabajar");
            }

            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Uppsss...");
            }

            return response;
        }

        // GET: api/Route/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Route
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Route/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Route/5
        public void Delete(int id)
        {
        }
    }
}
