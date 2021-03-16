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
    public class ReceptionBottleController : ApiController
    {
        // GET: api/ReceptionBottle
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ReceptionBottle/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ReceptionBottle
        public HttpResponseMessage Post([FromBody]CollectBottleDto dto)
        {
            HttpResponseMessage response;

            try
            {
                var token = Request.Headers.GetValues("OS_TOKEN").FirstOrDefault();

                var user = new UserService().getUserByToken(token);

                new ReceptionBottleService().createReceptionBottle(dto, user.UserId);

                response = Request.CreateResponse(HttpStatusCode.Created);
            }
            catch (Exception)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return response;
        }

        // PUT: api/ReceptionBottle/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ReceptionBottle/5
        public void Delete(int id)
        {
        }
    }
}
