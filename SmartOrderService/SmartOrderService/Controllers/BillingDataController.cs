using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class BillingDataController : ApiController
    {
        // GET: api/BillingData
        public HttpResponseMessage Get()
        {
            HttpResponseMessage response;


            try {

                var billingDatas = new BillingDataService().getBillingDatas();
                response = Request.CreateResponse(HttpStatusCode.OK, billingDatas);
            } catch(Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError,"Uppss algo salio mal");

            }

            return response;
        }

        // GET: api/BillingData/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/BillingData
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/BillingData/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/BillingData/5
        public void Delete(int id)
        {
        }
    }
}
