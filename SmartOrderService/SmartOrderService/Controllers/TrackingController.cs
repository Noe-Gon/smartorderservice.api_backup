using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.DTO;
using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class TrackingController : ApiController
    {
        // GET: api/Tracking
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Tracking/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet, Route("api/Tracking/GetConfiguration/UserCode/{userCode}/BranchCode/{branchCode}")]
        public HttpResponseMessage GetConfiguration(string userCode, string branchCode)
        {
            HttpResponseMessage response = null;
            TrackingConfigurationDto dto = new TrackingService().getConfiguration(userCode, branchCode);
            response = Request.CreateResponse<TrackingConfigurationDto>(HttpStatusCode.OK, dto);
            return response;
        }

        // POST: api/Tracking
        public HttpResponseMessage Post([FromBody]TrackingDto Dto)
        {
            HttpResponseMessage response;

            try
            {
                new TrackingService().RegisterPoint(Dto);
                response = Request.CreateResponse(HttpStatusCode.Created);
            }
            catch (NoUserFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict);
                
            }
            catch (Exception e)
            {

                response = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }


            return response;
            
        }

        [HttpGet, Route("api/Tracking/All")]
        public HttpResponseMessage GetAll()
        {
            HttpResponseMessage response;

            List<TrackingConfigurationDto> dtos = new TrackingService().getAll();
            HttpStatusCode statusCode = dtos.Count == 0 ? HttpStatusCode.NoContent : HttpStatusCode.OK;
            response = Request.CreateResponse(statusCode, dtos);
            return response;
        }

        // PUT: api/Tracking/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Tracking/5
        public void Delete(int id)
        {
        }
    }
}
