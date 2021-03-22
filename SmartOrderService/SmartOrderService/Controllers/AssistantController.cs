using SmartOrderService.CustomExceptions;
using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class AssistantController : ApiController
    {

        [HttpGet,Route("api/assistant/travelstatus")]
        public HttpResponseMessage CheckTravelStatus(int UserId)
        {
            HttpResponseMessage response;
            try
            {
                AssistantService service = new AssistantService();
                bool result = service.checkCurrentTravelState(UserId);
                response =  Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (RelatedDriverNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
            }
            catch (InventoryEmptyException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "No existe inventario, para este día");
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos...");
            }
            return response;
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}