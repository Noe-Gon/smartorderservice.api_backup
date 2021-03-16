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
    public class CollectBottleController : ApiController
    {
        // GET: api/CollectBottle
        public HttpResponseMessage Get([FromUri] CollectBottleRequest request)
        {
            HttpResponseMessage response;

            int customerId = request.CustomerId;

            try
            {
                var collect = new CollectBottleService().getCollect(customerId);
                response = Request.CreateResponse(HttpStatusCode.OK,collect);
            }
            catch (Exception e)
            {

                response = Request.CreateResponse(HttpStatusCode.InternalServerError,"Upps lo arreglaremos");
            }
            return response;
        }

        // GET: api/CollectBottle/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/CollectBottle
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/CollectBottle/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/CollectBottle/5
        public void Delete(int id)
        {
        }
    }
}
