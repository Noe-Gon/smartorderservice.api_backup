using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.DTO;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
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

        [HttpGet]
        [Route("~/api/deliveries")]
        public HttpResponseMessage GetDeliveries([FromUri] DeliveryRequest request)
        {
            HttpResponseMessage response;

            try
            {

                List<GetDeliveriesByInventoryResponse> deliveries = new DeliveryService().GetDeliveriesByInventory(request.InventoryId);

                response = Request.CreateResponse(HttpStatusCode.OK, deliveries);

            }
            catch (InventoryEmptyException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "No hay entregas para ese inventario");

            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "upss, lo arreglaremos...");
            }
            return response;
        }

        [HttpPost]
        [Route("~/api/registerorder")]
        public IHttpActionResult RegisterOrder(SendOrderRequest request)
        {
            try
            {
                var service = new DeliveryService();
                var response = service.SendOrder(request);

                if (response.Status)
                    return Content(HttpStatusCode.OK, response);

                return Content(HttpStatusCode.BadRequest, response);
            }
            catch (ApiPreventaNoAuthorizationException e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<SendOrderResponse>.Create(new List<string>()
                {
                    "Petición no autorizada, se requiere enviar el token de autorización o no cuenta con permisos.", e.Message
                }));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<SendOrderResponse>.Create(new List<string>()
                {
                    "Ocurrio un error al momento de procesar la información.", e.Message
                }));
            }
        }

        [HttpPost]
        [Route("~/api/registeredeliveries")]
        public IHttpActionResult GetDeliveriesStatus(GetDeliveriesRequest request)
        {
            try
            {
                var service = new DeliveryService();

                var response = service.GetDeliveriesStatus(request);

                if (response.Status)
                    return Content(HttpStatusCode.OK, response);

                return Content(HttpStatusCode.BadRequest, response);
            }
            catch (EntityNotFoundException e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<GetDeliveriesRequest>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<GetDeliveriesRequest>.Create(new List<string>()
                {
                    e.Message
                }));
            }
        }

        [HttpPost]
        [Route("~/api/delivery/preventa_update")]
        public IHttpActionResult Delivered(DeliveredRequest request)
        {
            try
            {
                var service = new DeliveryService();

                var response = service.UpdateDeliveryApiPreventa(request);

                if (response.Status)
                    return Content(HttpStatusCode.OK, response);

                return Content(HttpStatusCode.BadRequest, response);

            }
            catch (ArgumentNullException e)
            {
                return Content(HttpStatusCode.Conflict, ResponseBase<DeliveredResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (EntityNotFoundException e)
            {
                return Content(HttpStatusCode.NotFound, ResponseBase<DeliveredResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<DeliveredResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
        }

        [HttpPost]
        [Route("~/api/delivery/cancel")]
        public IHttpActionResult CancelDelivery(CancelDeliveryRequest request)
        {
            try
            {
                var service = new DeliveryService();

                var response = service.CancelDeliveryApiPreventa(request);

                if (response.Status)
                    return Content(HttpStatusCode.OK, response);

                return Content(HttpStatusCode.BadRequest, response);

            }
            catch (ArgumentNullException e)
            {
                return Content(HttpStatusCode.Conflict, ResponseBase<DeliveredResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (EntityNotFoundException e)
            {
                return Content(HttpStatusCode.NotFound, ResponseBase<DeliveredResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<DeliveredResponse>.Create(new List<string>()
                {
                    e.Message
                }));
            }
        }

        [HttpPost]
        [Route("~/api/delivery/status/update")]
        public IHttpActionResult UpdateDeliveryStatus(UpdateDeliveryStatus request)
        {
            try
            {
                using (var serivce = DeliveryStatusService.Create())
                {
                    var response = serivce.UpdateDeliveryStatus(request);

                    if (response.Status)
                        return Content(HttpStatusCode.OK, response);

                    return Content(HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception e)
            {
                var response = ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    "Error del sistema", e.Message
                });

                return Content(HttpStatusCode.InternalServerError, response);
            }
            
        }
    }
}
