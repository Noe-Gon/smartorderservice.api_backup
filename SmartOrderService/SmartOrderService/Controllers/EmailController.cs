using SmartOrderService.Models.Requests;
using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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

                if (response)
                    return Ok();

                return BadRequest();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}