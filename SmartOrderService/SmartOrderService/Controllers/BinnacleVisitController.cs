using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.Requests;
using SmartOrderService.Services;
using SmartOrderService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class BinnacleVisitController : ApiController
    {
        // GET: api/BinnacleVisit
        public HttpResponseMessage Get([FromUri]BinnacleVisitRequest request)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.NoContent);


            var LastSync = DateUtils.getDateTime(request.LastSync);

            BinnacleVisitService service = new BinnacleVisitService();
            try
            {
                if (request.BranchCode != null && request.UserCode != null)
                {
                    var visits = service.getVisitByLastSync(request.BranchCode, request.UserCode, LastSync);
                    response = Request.CreateResponse(HttpStatusCode.OK, visits);
                }
            }
            catch (NoUserFoundException ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Usuario no encontrado");
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }

        // GET: api/BinnacleVisit/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/BinnacleVisit
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/BinnacleVisit/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/BinnacleVisit/5
        public void Delete(int id)
        {
        }
    }
}
