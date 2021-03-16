using SmartOrderService.CustomExceptions;
using SmartOrderService.Models;
using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class BranchController : ApiController
    {
        // GET: api/Branch
        public HttpResponseMessage Get()
        {
            HttpResponseMessage response;
            try
            {
                List<BranchDto> dto = new BranchService().Get();
                response = Request.CreateResponse(HttpStatusCode.OK, dto);
            }
            catch (KeyNotFoundException ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return response;
        }

        // GET: api/Branch/5
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response;
            try
            {

                BranchDto dto = new BranchService().getBranch(id);
                response = Request.CreateResponse(HttpStatusCode.OK, dto);
            }
            catch (EntityNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "no se encontro esa sucursal");
            }
            
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Uppsss...");
            }

            return response;

        }

        // POST: api/Branch
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Branch/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Branch/5
        public void Delete(int id)
        {
        }
    }
}
