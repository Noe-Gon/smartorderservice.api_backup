using Newtonsoft.Json;
using SmartOrderService.Models.DTO;
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
    public class SurveyCustomerController : ApiController
    {

        

        [HttpGet, Route("api/surveycustomer/{branch}/{route}")]
        public HttpResponseMessage surveycustomer([FromUri]string branch, [FromUri]string route, [FromUri] bool withoutData = false)

        {
            try
            {
                //string variable1 = Request.;
                var list = new SurveyCustomerService().getAllByRoute(branch, route, withoutData);

                return Request.CreateResponse(HttpStatusCode.OK, list);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
            }

        }
    }
}