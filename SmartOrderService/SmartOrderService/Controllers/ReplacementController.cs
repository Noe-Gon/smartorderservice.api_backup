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
    public class ReplacementController : ApiController
    {
        // GET: api/Replacement
        public HttpResponseMessage Get([FromUri] ReplacementRequest request)
        {
            HttpResponseMessage response;
            try
            {
                if(request.ReplacementId != null)
                {
                    var replacement = new ReplacementService().getReplacement((int)request.ReplacementId);

                    if(replacement == null)
                    {
                        response = Request.CreateResponse(HttpStatusCode.NoContent);
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, replacement);
                    }
                }
                else
                { 
                    List<ReplacementDto> replacements = new ReplacementService().getReplacements();
                    response = Request.CreateResponse(HttpStatusCode.Created, replacements);
                }
            }
            
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Uppsss...");
            }

            return response;
        }

        // GET: api/Replacement/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Replacement
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Replacement/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Replacement/5
        public void Delete(int id)
        {
        }
    }
}
