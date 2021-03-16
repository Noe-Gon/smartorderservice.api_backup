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
    public class CategoryController : ApiController
    {
        // GET: api/Category
        public HttpResponseMessage Get()
        {
            HttpResponseMessage response;

            try {
                List<CategoryDto> categories = new CategoryService().getCategories();
                response = Request.CreateResponse(HttpStatusCode.OK, categories);
            }
            catch (Exception e) {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "upps... algo salio mal");
            }

            return response;
        }

        // GET: api/Category/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Category
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Category/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Category/5
        public void Delete(int id)
        {
        }
    }
}
