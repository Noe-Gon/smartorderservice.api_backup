
using System.Collections.Generic;
using System.Web.Http;
using SmartOrderService.Models.DTO;
using SmartOrderService.Services;
using System;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using SmartOrderService.Utils;
using System.Net.Http;
using System.Net;
using System.ComponentModel;

namespace SmartOrderService.Controllers
{
    public class ProductController : ApiController
    {
        // GET: api/Default
       
        public HttpResponseMessage Get([FromUri] ProductRequest request)
        {

            WorkdayService service = new WorkdayService();
            HttpResponseMessage response;
            try
            {
                if(request.ProductId != null)
                {
                    var product = new ProductService().getById((int)request.ProductId);
                    if(product == null)
                    {
                        response = Request.CreateResponse(HttpStatusCode.NoContent);
                    }
                    else
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, product);
                    }
                }
                else
                { 
                    DateTime updated = DateUtils.getDateTime(request.LastUpdate);

                    var products = new ProductService().getAll(updated, request.BranchId);
                
                    response = Request.CreateResponse(HttpStatusCode.OK, products);
                }
            }
           
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Uppsss...");
            }

            return response;

        }

        // GET: api/Default/5
        public ProductDto Get(int id)
        {
            return new ProductDto();
        }

        // POST: api/Default
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Default/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Default/5
        public void Delete(int id)
        {
        }
    }
}
