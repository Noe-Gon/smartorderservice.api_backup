using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class PromotionController : ApiController
    {
        // GET: api/Promotion
        public HttpResponseMessage Get()
        {
            PromotionService service = new PromotionService();
            HttpResponseMessage response;
            try
            {

                var promotions = service.getPromotions();
                response = Request.CreateResponse(HttpStatusCode.OK, promotions);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Uppsss...no existen promociones");
            }

            return response;
        }

        // GET: api/Promotion/5
        public HttpResponseMessage Get(int id)
        {
            PromotionService service = new PromotionService();
            HttpResponseMessage response;
            try
            {

                var promotion = service.getPromotion(id);
                response = Request.CreateResponse(HttpStatusCode.OK, promotion);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Uppsss...no se encontro la promocion");
            }

            return response;
        }

        // POST: api/Promotion
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Promotion/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Promotion/5
        public void Delete(int id)
        {
        }

        [HttpGet, Route("api/promotion/user/{UserId}/inventary/{InventaryId}")]
        public HttpResponseMessage GetPromotions(int UserId, int InventaryId)
        {
            try
            {
                var PromotionsDto = new PromotionService().GetSalePromotions(UserId, InventaryId);

                return Request.CreateResponse(HttpStatusCode.OK, PromotionsDto);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
            }
        }
    }
}
