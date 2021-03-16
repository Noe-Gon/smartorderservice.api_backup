using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class ReasonDevolutionController : ApiController
    {
        // GET: api/ReasonDevolution
        public HttpResponseMessage  Get()
        {
            HttpResponseMessage response;
            try
            {
                var reasons = new ReasonsService().getReasonDevolutions();
                response = Request.CreateResponse(HttpStatusCode.OK, reasons);
            }
          
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Uppsss...");
            }

            return response;
        }

        // GET: api/ReasonDevolution/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ReasonDevolution
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/ReasonDevolution/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ReasonDevolution/5
        public void Delete(int id)
        {
        }
    }
}
