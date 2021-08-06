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
                    return Ok(response);
                else
                    return Content(HttpStatusCode.BadRequest, response);
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
                    return Ok(response);
                else
                    return Content(HttpStatusCode.BadRequest, response);
            }
        }

        [HttpPost]
        [Route("~/api/consumer/removalrequest")]
        public IHttpActionResult ConsumerRemovalRequest(ConsumerRemovalRequestRequest request)
        {
            using (var service = GetService())
            {
                try
                {
                    var response = service.ConsumerRemovalRequestRequest(request);

                    if (response.Status)
                        return Ok(response);
                    else
                        return Content(HttpStatusCode.BadRequest, response);
                }
                catch (Exception e)
                {
                    return InternalServerError(e);
                }
            }
        }

        [HttpGet]
        [Route("~/api/consumer")]
        public IHttpActionResult GetConsumers([FromUri]int userId)
        {
            using (var service = GetService())
            {
                try
                {
                    var response = service.GetConsumers(new GetConsumersRequest { userId = userId});

                    if(response.Status)
                        return Ok(response);
                    else
                        return Content(HttpStatusCode.BadRequest, response);
                }
                catch (Exception e)
                {
                    return InternalServerError(e);
                }
            }
        }

        [HttpPost]
        [Route("~/api/ResendEmail/TicketDigital")]
        public IHttpActionResult ResendTicketDigital(ResendTicketDigitalRequest request)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.ResendTicketDigital(request);

                    if (response.Status)
                        return Ok(response);
                    else
                        return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}