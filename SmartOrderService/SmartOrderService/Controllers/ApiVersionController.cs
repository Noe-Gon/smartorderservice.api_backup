using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.Requests;
using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class ApiVersionController : ApiController
    {
        // GET: api/ApiVersion
        public HttpResponseMessage Get([FromUri] ApiVersionRequest request)
        {
            HttpResponseMessage response;
            try
            {

                var user = new ApiVersionService().getApiVersion(request.Branchcode, request.UserCode,request.PackageName);
                response = Request.CreateResponse(HttpStatusCode.OK, user);
            }
            catch (NoUserFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "No se encontro un usuario o paquete con esos valores");
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Uppsss..." + e.Message);
            }

            return response;
        }

        // GET: api/ApiVersion/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ApiVersion
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/ApiVersion/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiVersion/5
        public void Delete(int id)
        {
        }
    }
}
