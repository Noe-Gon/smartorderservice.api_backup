﻿using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.DTO;
using SmartOrderService.Models.Locks;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
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
    public class InventoryController : ApiController
    {
        private static Dictionary<string, InventoryLock> mapObjectService = new Dictionary<string, InventoryLock>();
        // GET: api/Inventory
        public HttpResponseMessage Get([FromUri]InventoryRequest request)
        {
            HttpResponseMessage response;

            try
            {
                var service = new InventoryService();
                List<InventoryDto> inventories = service.getInventories(request);

                /*if (inventories.Any()) {
                    var inventory = inventories.FirstOrDefault();

                    if (service.CloseInventory(inventory.InventoryId)) {
                        inventories = service.getInventories(request);
                    }

                }*/

                response = Request.CreateResponse(HttpStatusCode.OK, inventories);
            }
            catch (InventoryEmptyException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "No existe inventario, para este día");
            }
            catch (InventoryInProgressException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "El inventario esta siendo cargado a WBC");
            }
            catch (RelatedDriverNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
            }
            catch (BadRequestException e)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest,e.Message);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos...");
            }

            return response;
        }

        // POST: api/Inventory
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Inventory/5
        [HttpPut, Route("api/inventory/{inventoryId}/close")]
        public HttpResponseMessage CloseInventory(int inventoryId)
        {

            HttpResponseMessage response;

            try
            {
                var result = new InventoryService().CloseInventory(inventoryId);
                HttpStatusCode code = result ? HttpStatusCode.OK : HttpStatusCode.Conflict;
                response = Request.CreateResponse(code);

            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Error interno: " + e.Message);
            }

            return response;
        }

        [HttpPut, Route("api/inventory/{inventoryId}/open")]
        public HttpResponseMessage OpenInventory(int inventoryId)
        {

            HttpResponseMessage response;

            try
            {
                new InventoryService().OpenInventory(inventoryId);

                response = Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (EntityNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "No se encontro inventario con ese id");
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Error interno: " + e.Message);
            }

            return response;
        }

        // DELETE: api/Inventory/5
        public void Delete(int id)
        {
        }


        [HttpPut, Route("api/inventory/open")]
        public HttpResponseMessage OpenInventory([FromUri]InventoryRequest request)
        {
            HttpResponseMessage response;
            if (!request.InventoryId.HasValue)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "Falta el parametro InventoryId");
                return response;
            }
            try
            {
                int inventoryId = request.InventoryId.Value;
                using (var inventoryService = new InventoryService())
                {
                    inventoryService.OpenInventory(inventoryId, request.UserId);
                    response = Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (InventoryNotOpenException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict,e.Message);
            }
            catch(WorkdayNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
            }
            return response;
        }

        [HttpPut, Route("api/inventory/close")]
        public HttpResponseMessage CloseInventory([FromUri]InventoryRequest request)
        {
            HttpResponseMessage response;
            if (!request.InventoryId.HasValue)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, "Falta el parametro InventoryId");
                return response;
            }
            try
            {
                InventoryLock serviceLock = new InventoryLock(); 
                RouteTeamService servce = new RouteTeamService();
                var routeId = servce.GetRouteId(request.UserId);

                if (routeId == 0)
                {
                    var InventoryService = new InventoryService();
                    bool result = InventoryService.CloseInventory(request.InventoryId.Value, request.UserId);
                    HttpStatusCode code = result ? HttpStatusCode.OK : HttpStatusCode.Conflict;
                    return Request.CreateResponse(code);
                }

                if (mapObjectService.ContainsKey(routeId.ToString()))
                {
                    serviceLock = mapObjectService[routeId.ToString()];
                    serviceLock.key = request.UserId;
                }
                else
                {
                    serviceLock = new InventoryLock
                    {
                        key = request.UserId,
                        InventoryService = new InventoryService()
                    };
                    mapObjectService.Add(routeId.ToString(), serviceLock);
                }

                lock (serviceLock.InventoryService)
                {
                    bool result = serviceLock.InventoryService.CloseInventory(request.InventoryId.Value, request.UserId);
                    HttpStatusCode code = result ? HttpStatusCode.OK : HttpStatusCode.Conflict;

                    if (serviceLock.key == request.UserId)
                        mapObjectService.Remove(routeId.ToString());

                    response = Request.CreateResponse(code);
                }
            }
            catch (WorkdayNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
            }
            catch (EntityNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
            }
            catch (InventoryNotOpenException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
            }
            return response;
        }

        
        [HttpPost,Route("api/inventory/transferunsoldinventory")]
        public HttpResponseMessage TransferUnsoldInventory([FromBody] InventoryRequest request)
        {
            HttpResponseMessage response;
            try
            {
                var inventoryService = new InventoryService();
                inventoryService.TransferUnsoldInventory(request.UserId);
                response = Request.CreateResponse(HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict,e);
            }
            return response;
        }
        
        [HttpPost]
        [Route("api/inventory/loaddeliveries")]
        public IHttpActionResult LoadInventoryDeliveries([FromBody]LoadInventoryDeliveriesRequest request)
        {

            try
            {
                InventoryLock serviceLock = new InventoryLock();

                if (mapObjectService.ContainsKey(request.InventoryId.ToString()))
                {
                    serviceLock = mapObjectService[request.InventoryId.ToString()];
                    serviceLock.key++;
                    request.UserId = serviceLock.key;
                }
                else
                {
                    request.UserId = 1;
                    serviceLock = new InventoryLock
                    {
                        key = request.UserId,
                        InventoryService = new InventoryService()
                    };
                    mapObjectService.Add(request.InventoryId.ToString(), serviceLock);
                }

                lock (serviceLock.InventoryService)
                {
                    var inventoryService = new InventoryService();
                    var response = inventoryService.LoadDeliveries(request.InventoryId);

                    if (serviceLock.key == request.UserId)
                        mapObjectService.Remove(request.InventoryId.ToString());

                    if (response.Status)
                        return Content(HttpStatusCode.OK, response);

                    return Content(HttpStatusCode.Conflict, response);
                }
            }
            catch (EntityNotFoundException e)
            {
                var response = ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    e.Message
                });

                return Content(HttpStatusCode.NotFound, response);
            }
            catch (Exception e)
            {
                var response = ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    "Error no especificado", e.Message
                });

                return Content(HttpStatusCode.InternalServerError, response);
            }
        }


        [HttpGet, Route("api/inventory/isInventoryOpen")]
        public HttpResponseMessage isInventoryOpen([FromUri] int inventoryId, [FromUri] int userId)
        {
            HttpResponseMessage response;
            try
            {
                var inventoryService = new InventoryService();
                var inventoryOpen = inventoryService.isInventoryOpen(inventoryId,userId);
                response = Request.CreateResponse(HttpStatusCode.OK, inventoryOpen);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, e);
            }
            return response;
        }

        [HttpGet, Route("api/v2/inventory/isInventoryOpen")]
        public HttpResponseMessage isInventoryOpenv2([FromUri] int inventoryId, [FromUri] int userId)
        {
            HttpResponseMessage response;
            try
            {
                var inventoryService = new InventoryService();
                var inventoryOpen = inventoryService.isInventoryOpenv2(inventoryId, userId);
                response = Request.CreateResponse(HttpStatusCode.OK, inventoryOpen);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, e);
            }
            return response;
        }
    }
}
