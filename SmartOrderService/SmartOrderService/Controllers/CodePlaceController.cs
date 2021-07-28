using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class CodePlaceController : ApiController
    {
        public CodePlaceService GetService()
        {
            return CodePlaceService.Create();
        }

        [HttpGet]
        [Route("~/api/codeplaces")]
        public IHttpActionResult GetCodePlaces()
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.GetCodePlaces();

                    if (response.Status)
                        return Ok(response);

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