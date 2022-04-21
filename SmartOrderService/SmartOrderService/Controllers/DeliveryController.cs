using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.DTO;
using SmartOrderService.Models.Requests;
using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class DeliveryController : ApiController
    {
        // GET: api/Delivery
        public HttpResponseMessage Get([FromUri] DeliveryRequest request)
        {
            HttpResponseMessage response;
            
            try
            {

                List<DeliveryDto> deliveries = new DeliveryService().getDeliveriesBy(request.InventoryId);
                
                response = Request.CreateResponse(HttpStatusCode.OK, deliveries);

            }
            catch (InventoryEmptyException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "No hay entregas para ese inventario");

            }
            catch (InventoryNotOpenException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "upss, lo arreglaremos...");
            }
            return response;
           
        }

        // GET: api/Delivery/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Delivery
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Delivery/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Delivery/5
        public void Delete(int id)
        {
        }
    }
}
