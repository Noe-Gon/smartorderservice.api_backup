using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.Requests;
using SmartOrderService.Services;
using SmartOrderService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class CellarNoticeController : ApiController
    {
        // GET: api/CellarNotice
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/CellarNotice/5
        public string Get(int id)
        {
            return "value";
        }
       
        // POST: api/CellarNotice
        public HttpResponseMessage Post([FromBody]CellarNoticeRequest dto)
        {
            HttpResponseMessage response;

            try
            {

                var token = Request.Headers.GetValues("OS_TOKEN").FirstOrDefault();

                var user = new UserService().getUserByToken(token);

                var notice = new CellarNoticeService().createNotice(dto.RouteId,DateUtils.getDateTime(dto.Date),user.UserId);
                response = Request.CreateResponse(HttpStatusCode.Created,notice);
            }
            catch (NotFoundUsersConfigException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "no se encontro configuración de aviso");
            }
            
            catch (DeviceNotFoundException e) {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "dispositivo no registrado");
            }

            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError,"ups... lo arreglaremos");
            }

            return response;
            
        }

        // PUT: api/CellarNotice/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/CellarNotice/5
        public void Delete(int id)
        {
        }
    }
}
