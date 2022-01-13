﻿using SmartOrderService.CustomExceptions;
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
            catch (Exception e)
            {
                return Content(HttpStatusCode.InternalServerError, ResponseBase<GetDeliveriesRequest>.Create(new List<string>()
                {
                    e.Message
                }));
            }
        }
    }
}
