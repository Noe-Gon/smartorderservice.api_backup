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
    public class UserDevolutionController : ApiController
    {

        // GET: api/UserDevolution
        [HttpGet, Route("Api/UserDevolution/Reasons")]
        public HttpResponseMessage GetReasonDevolutions()
        {
            HttpResponseMessage response;

            try
            {
                var reasons = new UserDevolutionService().getReasons();
                response = Request.CreateResponse(HttpStatusCode.OK,reasons);
            }
            catch (Exception)
            {

                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos");
            }
            return response;
        }

        // GET: api/UserDevolution
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/UserDevolution/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/UserDevolution
        public HttpResponseMessage Post([FromBody]UserDevolutionDto dto)
        {

            HttpResponseMessage response;

            try
            {
                var token = Request.Headers.GetValues("OS_TOKEN").FirstOrDefault();

                var user = new UserService().getUserByToken(token);

                var service = new UserDevolutionService().createDevolution(dto,user.UserId);
                response = Request.CreateResponse(HttpStatusCode.Created, dto);

            }
            catch (DeviceNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "no estas registrado");

            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError,"upss lo arreglaremos");
   
            }

            return response;
        }

        // PUT: api/UserDevolution/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/UserDevolution/5
        public HttpResponseMessage Delete(int id)
        {
            HttpResponseMessage response;
            try
            {
                var token = Request.Headers.GetValues("OS_TOKEN").FirstOrDefault();
                var user = new UserService().getUserByToken(token);

                new UserDevolutionService().deleteDevolution(id);
                response = Request.CreateResponse(HttpStatusCode.Accepted);
            }
            catch (DeviceNotFoundException)
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "no estas registrado");
            }
            catch (Exception)
            {

                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "upss lo arreglaremos");
            }

            return response;
           
        }

        [HttpDelete, Route("Api/UserDevolution/Clear/{inventoryId}")]
        public HttpResponseMessage DeleteByInventory(int inventoryId)
        {
            HttpResponseMessage response;
            try
            {

                var token = Request.Headers.GetValues("OS_TOKEN").FirstOrDefault();

                var user = new UserService().getUserByToken(token);

                new UserDevolutionService().deleteInventoryDevolution(inventoryId);

                response = Request.CreateResponse(HttpStatusCode.Accepted);
            }
            catch (DeviceNotFoundException)
            {

                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "no estas registrado");
            }

            catch (Exception)
            {

                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "upss lo arreglaremos");
            }

            return response;

        }

    }
}
