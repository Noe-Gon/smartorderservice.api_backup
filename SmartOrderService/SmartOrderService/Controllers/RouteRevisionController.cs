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
    public class RouteRevisionController : ApiController
    {
        // GET: api/RouteRevision
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet, Route("api/RouteRevision/types")]
        public HttpResponseMessage GetTypes()
        {
            HttpResponseMessage response;

            try
            {

                var routeRevisionTypes = new InventoryRevisionService().getTypes();

                response = Request.CreateResponse(HttpStatusCode.OK, routeRevisionTypes);
            }
         
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos");
            }

            return response;
        }


        [HttpGet, Route("api/RouteRevision/states")]
        public HttpResponseMessage GetStates()
        {
            HttpResponseMessage response;

            try
            {

                var routeRevisionStates = new InventoryRevisionService().getStates();

                response = Request.CreateResponse(HttpStatusCode.OK, routeRevisionStates);
            }
        
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos");
            }

            return response;
        }



        // GET: api/RouteRevision/5
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response;

            try
            {
             
                var routeRevision = new InventoryRevisionService().FindRevision(id);

                response = Request.CreateResponse(HttpStatusCode.OK, routeRevision);
            }
            catch (DeviceNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "No estas registrado");
            }
            catch (InventoryRevisionException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Existe una revision pendiente");
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos");
            }

            return response;
        }

        // POST: api/RouteRevision
        public HttpResponseMessage Post([FromBody]RouteRevisionRequest dto)
        {

            HttpResponseMessage response;

            try
            {

                var token = Request.Headers.GetValues("OS_TOKEN").FirstOrDefault();

                var user = new UserService().getUserByToken(token);
                var date = DateUtils.getDateTime(dto.Date);
                //var routeRevision = new InventoryRevisionService().CreateRevision(dto.RouteId,dto.RevisionType,date,user.UserId);

                response = Request.CreateResponse(HttpStatusCode.Created);
            }
            catch (DeviceNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "No estas registrado");
            }
            catch (InventoryRevisionException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Existe una revision pendiente");
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos");
            }

            return response;


        }

        // PUT: api/RouteRevision/5
        [HttpPut, Route("api/RouteRevision/{id}/rejected")]
        public HttpResponseMessage Put(int id,bool aproved = false)
        {

            HttpResponseMessage response;

            try
            {

                new InventoryRevisionService().updateRevision(id, 3);

                response = Request.CreateResponse(HttpStatusCode.Accepted);

            }
      
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos");
            }

            return response;


        }

        [HttpPut, Route("api/RouteRevision/{id}/aproved") ]
        public HttpResponseMessage Put(int id)
        {

            HttpResponseMessage response;

            try
            {

            new InventoryRevisionService().updateRevision(id, 2);

                response = Request.CreateResponse(HttpStatusCode.Accepted);
            }
           
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos");
            }

            return response;

        }

        // DELETE: api/RouteRevision/5
        public void Delete(int id)
        {
        }
    }
}
