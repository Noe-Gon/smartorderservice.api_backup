using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SmartOrderService.Models.DTO;
using SmartOrderService.Services;
using SmartOrderService.CustomExceptions;
using System.Web;

namespace SmartOrderService.Controllers
{
    public class DeviceController : ApiController
    {
        // GET: api/Device
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Device/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Device
        public HttpResponseMessage Post([FromBody]Device device)
        {

            HttpResponseMessage response;
            

            try {
                DeviceService service = new DeviceService();
                service.RegisterDevice(device);
                response = Request.CreateResponse(HttpStatusCode.Created, device);

            }
            catch(NoUserFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "no se encontro el usuario");

            }
            catch(WorkdayStartedException e)
            {

                response = Request.CreateResponse(HttpStatusCode.Conflict, "existe una jornada abierta para el usuario del dispositivo");
            }
            catch(Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError,"upss, lo arreglaremos...");
            }

            

            return response;
        }

        // PUT: api/Device/5
        public void Put(string DeviceId, [FromBody]Device value)
        {
        }

        // DELETE: api/Device/5
        public void Delete(int id)
        {
        }
    }
}
