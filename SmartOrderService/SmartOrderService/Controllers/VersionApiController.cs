using SmartOrderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace SmartOrderService.Controllers
{
    public class VersionApiController : ApiController
    {
        public HttpResponseMessage Get()
        {
            VersionAPI version = VersionAPI.getVersionAPI() ;

            return Request.CreateResponse(System.Net.HttpStatusCode.OK, version);
        }
    }
}