using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class CountConfigurationController : ApiController
    {
        // GET: api/CountConfiguration
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        
        // GET: api/CountConfiguration/5
        public HttpResponseMessage Get(int id)
        {

            HttpResponseMessage response;
            try
            {
                var result = new CountConfigurationService().CountRoute(id);
                response = Request.CreateResponse(HttpStatusCode.Accepted,result);
            }
            catch (Exception)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "revisaremos que ocurrio");
                
            }


            return response;
        }

        // POST: api/CountConfiguration
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/CountConfiguration/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/CountConfiguration/5
        public void Delete(int id)
        {
        }
    }
}
