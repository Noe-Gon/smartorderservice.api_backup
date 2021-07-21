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
    public class CustomerVisitController : ApiController
    {
        // GET: api/CustomerVisit
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/CustomerVisit/5
        public string Get(int id)
        {
            return "value";
        }

        [Route("api/Customer/{CustomerId}/Visit")]
        public HttpResponseMessage Post(int? CustomerId, [FromBody]CustomerVisitDto dto)
        {
            HttpResponseMessage response;
            try
            {
                
                var visit = new CustomerService().CreateVisit(dto, CustomerId);
                response = Request.CreateResponse(HttpStatusCode.Created, visit);

            }
            catch(CustomerException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "No existe un cliente con ese identificador " + CustomerId);
            }

            catch (Exception e)
            {

                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "No se pudo registrar la visita, valida estar registrado ");
            }

            return response;
        }


        // POST: api/CustomerVisit
        public HttpResponseMessage Post([FromBody]CustomerVisitDto dto)
        {
            //var token = Request.Headers.GetValues("OS_TOKEN").FirstOrDefault();
            HttpResponseMessage response;
            try
            {

                var UserId = 0;
               
                    var user = new UserService().getUser((int)dto.UserId);
                    if (user != null)
                        UserId = user.UserId;
               

                var visit = new CustomerService().CreateVisit(dto, UserId);
                response = Request.CreateResponse(HttpStatusCode.Created, visit);

            }
            catch (NoUserFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "El usuario no existe en WBC");
            }

            catch (Exception e)
            {

                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "No se pudo registrar la visita, valida estar registrado e" + e.InnerException.Message + " d " + e.InnerException.ToString());
            }

            return response;
        }

        [HttpPost]
        [Route("api/Customer/TeamVisit")]
        public HttpResponseMessage PostTeamVisit([FromBody] CustomerVisitDto dto)
        {
            HttpResponseMessage response;
            try
            {
                var UserId = 0;

                var user = new UserService().getUser((int)dto.UserId);
                if (user != null)
                    UserId = user.UserId;

                var visit = new CustomerService().CreateTeamVisit(dto, UserId);
                response = Request.CreateResponse(HttpStatusCode.Created, visit);

            }
            catch (NoUserFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "El usuario no existe en WBC");
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "No se pudo registrar la visita, valida estar registrado e" + e.InnerException.Message + " d " + e.InnerException.ToString());
            }

            return response;
        }

        // PUT: api/CustomerVisit/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/CustomerVisit/5
        public void Delete(int id)
        {
        }
    }
}
