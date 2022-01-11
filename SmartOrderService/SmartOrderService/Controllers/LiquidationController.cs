using SmartOrderService.Models.Message;
using SmartOrderService.Models.Responses;
using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SmartOrderService.Controllers
{
    public class LiquidationController : ApiController
    {
        public LiquidationService GetService()
        {
            return LiquidationService.Create();
        }

        [HttpGet]
        [Route("~/api/liquidation/sales")]
        public IHttpActionResult GetLiquidationSales([FromUri]int UserId, [FromUri]int RouteId, [FromUri]DateTime? Date)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.GetLiquidationSales(new GetLiquidationSalesRequest
                    {
                        UserId = UserId,
                        RouteId = RouteId,
                        Date = Date == null ? DateTime.Today : Date.Value.Date
                    });

                    if (response.Status)
                        return Ok(response);

                    return Content(System.Net.HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, ResponseBase<GetLiquidationSalesResponse>.Create(new List<string>()
                {
                    "Error interno del servidor", e.Message
                }));
            }
            
        }

        [HttpGet]
        [Route("~/api/liquidation/repayments")]
        public IHttpActionResult GetLiquidationRepayments([FromUri] int UserId, [FromUri] int RouteId, [FromUri] DateTime? Date)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.GetLiquidationRepayments(new GetLiquidationRepaymentsRequest
                    {
                        UserId = UserId,
                        RouteId = RouteId,
                        Date = Date == null ? DateTime.Today : Date.Value.Date
                    });

                    if (response.Status)
                        return Ok(response);

                    return Content(System.Net.HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, ResponseBase<GetLiquidationSalesResponse>.Create(new List<string>()
                {
                    "Error interno del servidor", e.InnerException.Message
                }));
            }

        }

        [HttpGet]
        [Route("~/api/liquidation/emptybottles")]
        public IHttpActionResult GetEmptyBottles([FromUri]int userId, [FromUri]int? inventoryId, [FromUri]DateTime? date)
        {
            try
            {

            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}