using Microsoft.AspNet.Identity;
using SmartOrderService.Models.Requests;
using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class ConsumerController : ApiController
    {
        public ConsumerService GetService()
        {
            return ConsumerService.Create();
        }

        [HttpPost]
        [Route("~/api/consumer")]
        public IHttpActionResult InserConsumer(InsertConsumerRequest request)
        {
            using (var service = GetService())
            {
                var response = service.InsertConsumer(request);

                if (response.Status)
                    return (IHttpActionResult)Request.CreateResponse(HttpStatusCode.Created, response);
                else
                    return (IHttpActionResult)Request.CreateResponse(HttpStatusCode.BadRequest, response);
            }
        }

        [HttpPut]
        [Route("~/api/consumer")]
        public IHttpActionResult UpdateConsumer(UpdateConsumerRequest request)
        {
            using (var service = GetService())
            {
                var response = service.UpdateConsumer(request);

                if (response.Status)
                    return (IHttpActionResult)Request.CreateResponse(HttpStatusCode.OK, response);
                else
                    return (IHttpActionResult)Request.CreateResponse(HttpStatusCode.BadRequest, response);
            }
        }

        //[HttpPost]
        //[Route("~/api/consumer/removalrequest")]
        //public IHttpActionResult ConsumerRemovalRequest(ConsumerRemovalRequestRequest request)
        //{

        //}
    }
}