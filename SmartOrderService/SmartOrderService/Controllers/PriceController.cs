using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using SmartOrderService.Services;
using SmartOrderService.Utils;

namespace SmartOrderService.Controllers
{
    public class PriceController : ApiController
    {
        // GET: api/Price

        public HttpResponseMessage Get([FromUri] PriceRequest request)
        {

            HttpResponseMessage response;
            try
            {
                DateTime time = Utils.DateUtils.getDateTime(request.LastUpdate);

                PriceService service = new PriceService();

                var prices = service.getPricesByInventoryCustomer(request.InventoryId, request.BranchId, time, request.CustomerId);


                response = Request.CreateResponse(HttpStatusCode.OK, prices);
            }

            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Uppsss...");
            }

            return response;
        }

        [HttpGet, Route("api/v2/Price")]
        public HttpResponseMessage Getv2([FromUri] PriceRequest request)
        {

            HttpResponseMessage response;
            try
            {
                DateTime time = Utils.DateUtils.getDateTime(request.LastUpdate);

                PriceService service = new PriceService();

                var prices = service.getPricesByInventoryCustomerv2(request.InventoryId, request.BranchId, time, request.CustomerId);


                response = Request.CreateResponse(HttpStatusCode.OK, prices);
            }

            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Uppsss...");
            }

            return response;
        }

        [HttpGet, Route("Api/PriceList")]
        public HttpResponseMessage GetPriceList([FromUri] PriceListRequest request)
        {

            HttpResponseMessage response;
            try
            {
                DateTime time = Utils.DateUtils.getDateTime(request.LastUpdate);

                PriceService service = new PriceService();

                var prices = service.getPricesByInventory(request.InventoryId, request.BranchId, time);

                response = Request.CreateResponse(HttpStatusCode.OK, prices);
            }

            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Uppsss..." + e.Message);
            }

            return response;
        }

        [HttpGet, Route("api/v2/PriceList")]
        public HttpResponseMessage GetPriceListv2([FromUri] PriceListRequest request)
        {

            HttpResponseMessage response;
            try
            {
                DateTime time = Utils.DateUtils.getDateTime(request.LastUpdate);

                PriceService service = new PriceService();

                var prices = service.getPricesByInventoryv2(request.InventoryId, request.BranchId, time);

                response = Request.CreateResponse(HttpStatusCode.OK, prices);
            }

            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Uppsss..." + e.Message);
            }

            return response;
        }

        // GET: api/Price/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Price
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Price/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Price/5
        public void Delete(int id)
        {
        }
    }
}
