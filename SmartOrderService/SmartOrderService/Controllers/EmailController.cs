using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class EmailController : ApiController
    {
        [HttpPost]
        [Route("~/api/SendEmail/Wellcome")]
        public IHttpActionResult SendWellcomeEmail(WellcomeEmailRequest request)
        {
            try
            {
                var service = new EmailService();
                var response = service.SendWellcomeEmail(request);

                if (response.Status)
                    return Ok(response);

                return Content(HttpStatusCode.BadRequest, response);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpPost]
        [Route("~/api/SendEmail/TicketDigital")]
        public IHttpActionResult SendTicketDigital(SendTicketDigitalEmailRequest request)
        {
            try
            {
                var service = new EmailService();
                var response = service.SendTicketDigitalEmail(request);

                if (response.Status)
                    return Ok(response);

                return Content(HttpStatusCode.BadRequest, response);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpPost]
        [Route("~/api/SendEmail/ReactivationTicket")]
        public IHttpActionResult SendReactivationTicketDigital(SendReactivationTicketDigitalRequest request)
        {
            try
            {
                var service = new EmailService();
                var response = service.SendReactivationTicketDigital(request);

                if (response.Status)
                    return Ok(response);

                return Content(HttpStatusCode.BadRequest, response);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpPost]
        [Route("~/api/SendRemovalRequest")]
        public IHttpActionResult SendRemovalRequest(SendRemovalRequestEmailRequest request)
        {
            var service = new EmailService();
            var response = service.SendRemovalRequestEmail(request);

            if (response.Status)
                return Ok(response);

            return Content(HttpStatusCode.BadRequest, response);
        }

        [HttpPost]
        [Route("~/api/SendOrderTicket")]
        public IHttpActionResult SendOrderTicket(SendOrderTicketRequest request)
        {
            try
            {
                var service = new EmailService();
                var response = service.SendOrderTicket(request);

                if (response.Status)
                    return Ok(response);

                return Content(HttpStatusCode.BadRequest, response);
            }
            catch (Exception e)
            {
                var response = ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    e.Message
                });

                return Content(HttpStatusCode.InternalServerError, response);
            }
        }
    }
}