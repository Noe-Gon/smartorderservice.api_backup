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
    public class InventoryRevisionController : ApiController
    {
        [HttpGet, Route("api/InventoryRevision/{id}")]
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response;

            try
            {

                var Revision = new InventoryRevisionService().FindRevision(id);

                response = Request.CreateResponse(HttpStatusCode.OK, Revision);
            }
            catch (InventoryRevisionException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "No se encontro revision con ese identificador");
            }

            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos: " + e.Message);
            }

            return response;
        }

        [HttpGet, Route("api/InventoryRevision/types")]
        public HttpResponseMessage GetTypes()
        {
            HttpResponseMessage response;

            try
            {

                var RevisionTypes = new InventoryRevisionService().getTypes();

                response = Request.CreateResponse(HttpStatusCode.OK, RevisionTypes);
            }

            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos");
            }

            return response;
        }

        [HttpGet, Route("api/InventoryRevision/states")]
        public HttpResponseMessage GetStates()
        {
            HttpResponseMessage response;

            try
            {

                var RevisionStates = new InventoryRevisionService().getStates();

                response = Request.CreateResponse(HttpStatusCode.OK, RevisionStates);
            }

            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos");
            }

            return response;
        }


        public HttpResponseMessage Get([FromUri] InventoryRevisionRequest request)
        {
            HttpResponseMessage response;
            try
            {
                var Date = DateUtils.getDateTime(request.Date);
                var inventories = new InventoryService().getInventoryByRoute(request.RouteId, Date);
                
                response = Request.CreateResponse(HttpStatusCode.OK, inventories);

            }
            catch (InventoryEmptyException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NoContent, "información no disponible");
            }

            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos...");
            }

            return response;
        }


        // POST: api/RouteRevision
        public HttpResponseMessage Post([FromBody]InventoryRevisionRequest dto)
        {

            HttpResponseMessage response;

            try
            {

                var token = Request.Headers.GetValues("OS_TOKEN").FirstOrDefault();

                var user = new UserService().getUserByToken(token);
                var date = DateUtils.getDateTime(dto.Date);

                var Revision = new InventoryRevisionService()
                    .CreateRevision(dto.RouteId, dto.InventoryId,dto.RevisionType, date, user.UserId);

                response = Request.CreateResponse(HttpStatusCode.Created, Revision);
            }
            catch (DeviceNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "No estas registrado");
            }
            catch (InventoryRevisionException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Existe una revision pendiente");
            }
            catch (NoCustomerVisitException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "No has Visitado a todos los clientes");
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos");
            }

            return response;


        }

        // PUT: api/RouteRevision/5
        [HttpPut, Route("api/InventoryRevision/{id}/rejected")]
        public HttpResponseMessage Put(int id, bool aproved = false)
        {

            HttpResponseMessage response;

            try
            {

                new InventoryRevisionService().updateRevision(id, 3);

                response = Request.CreateResponse(HttpStatusCode.OK);

            }
            catch (InventoryRevisionException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "No se encontro revision para ese inventario");
            }
            catch (Exception e)
            {
                
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos");
            }

            return response;


        }

        [HttpPut, Route("api/InventoryRevision/{id}/aproved")]
        public HttpResponseMessage Put(int id)
        {

            HttpResponseMessage response;

            try
            {

                new InventoryRevisionService().updateRevision(id, 2);

                response = Request.CreateResponse(HttpStatusCode.OK);
            }

            catch (InventoryRevisionException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "No se encontro revision para ese inventario");
            }
            catch(Exception e) { 
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos");
            }

            return response;

        }

        // DELETE: api/InventoryRevision/5
        public void Delete(int id)
        {
        }
    }
}
