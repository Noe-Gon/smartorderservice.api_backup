using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.DTO;
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
    public class InventoryController : ApiController
    {
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
                new InventoryService().OpenInventory(inventoryId,request.UserId);
                response = Request.CreateResponse(HttpStatusCode.OK);
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
                var result = new InventoryService().CloseInventory(request.InventoryId.Value,request.UserId);
                HttpStatusCode code = result ? HttpStatusCode.OK : HttpStatusCode.Conflict;
                response = Request.CreateResponse(code);
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
    }
}
