using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class ApplicationController : ApiController
    {
        // GET: api/Application
        public HttpResponseMessage Get()
        {
            HttpResponseMessage response;
            try
            {
                var applications = new ApplicationService().getApplications();
                response = Request.CreateResponse(HttpStatusCode.OK, applications);
            }

            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Uppsss...");
            }

            return response;
        }

        // GET: api/Application/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Application
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Application/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Application/5
        public void Delete(int id)
        {
        }
    }
}
