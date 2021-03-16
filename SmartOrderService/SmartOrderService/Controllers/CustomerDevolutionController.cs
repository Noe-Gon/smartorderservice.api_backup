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
    public class CustomerDevolutionController : ApiController
    {
        // GET: api/CustomerDevolution
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/CustomerDevolution/5
        public string Get(int id)
        {
            return "value";
        }

        [Route("api/Delivery/{DeliveryId}/Devolution")]
        public HttpResponseMessage Post(int? DeliveryId, [FromBody]CustomerDevolutionDto dto)
        {
            HttpResponseMessage response;

            try
            {

                var devolution = new DeliveryService().CreateDevolution(DeliveryId, dto.ReasonDevolutionId, dto.UserId);

                dto.DeliveryId = devolution.deliveryId;
                dto.ReasonDevolutionId = devolution.reasonId;

                response = Request.CreateResponse(HttpStatusCode.Created, dto);

            }
            catch (DeliveryNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "no existe un entrega con ese identificador: " + DeliveryId);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "existe un error: " + e.Message);
            }

            return response;
        }


        
        // POST: api/CustomerDevolution
        public HttpResponseMessage Post( [FromBody]CustomerDevolutionDto dto)
        {
            var token = Request.Headers.GetValues("OS_TOKEN").FirstOrDefault();
            HttpResponseMessage response;

            try
            {
                
                var devolution = new DeliveryService().CreateDevolution(dto.DeliveryId, dto.ReasonDevolutionId);

                dto.DeliveryId = devolution.deliveryId;
                dto.ReasonDevolutionId = devolution.reasonId;

                response = Request.CreateResponse(HttpStatusCode.Created, dto);

            }
            catch (DeliveryNotFoundException e)
            {

                response = Request.CreateResponse(HttpStatusCode.NotAcceptable, "no existe un entrega con ese identificador: "+dto.DeliveryId);

            }
            catch (DeviceNotFoundException e)
            {

                response = Request.CreateResponse(HttpStatusCode.NotAcceptable, "No estas registrado T" + token);

            }
            catch (Exception e)
            {

                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "existe un error: "+e.Message);

            }

            return response;

        }

        // PUT: api/CustomerDevolution/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/CustomerDevolution/5
        public void Delete(int id)
        {
        }
    }
}
