using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.Enum;
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
                    "Error interno del servidor", e.Message
                }));
            }

        }

        [HttpGet]
        [Route("~/api/liquidation/emptybottles")]
        public IHttpActionResult GetEmptyBottles([FromUri]int userId, [FromUri]int? inventoryId, [FromUri]DateTime? date)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.GetEmptyBottle(new GetEmptyBottleRequest
                    {
                        UserId = userId,
                        Date = date,
                        InventoryId = inventoryId
                    });

                    if (response.Status)
                        return Ok(response);

                    return Content(System.Net.HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, ResponseBase<GetEmptyBottleResponse>.Create(new List<string>()
                {
                    "Error interno del servidor", e.Message
                }));
            }
        }

        [HttpPost]
        [Route("~/api/liquidation/send")]
        public IHttpActionResult SendLiquidation(SendLiquidationRequest request)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.SendLiquidation(request);

                    if (response.Status)
                        return Ok(response);

                    return Content(System.Net.HttpStatusCode.BadRequest, response);
                }
            }
            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, ResponseBase<SendLiquidationResponse>.Create(new List<string>()
                {
                    "Error interno del servidor", e.Message
                }));
            }
        }

        [HttpPost]
        [Route("~/api/liquidation/status")]
        public IHttpActionResult GetLiquidationStatus(GetLiquidationStatusRequest request)
        {
            try
            {
                using (var service = GetService())
                {
                    var response = service.GetLiquidationStatus(request);

                    if (response.Status)
                    {
                        switch (response.Data.Code)
                        {
                            case HelperLiquidationLogStatus.SUCCEEDED:
                                return Ok(response);
                            case HelperLiquidationLogStatus.RUNNING:
                                return Content(System.Net.HttpStatusCode.Accepted, response);
                            case HelperLiquidationLogStatus.TIMED_OUT:
                                return Content(System.Net.HttpStatusCode.GatewayTimeout, response);
                            case HelperLiquidationLogStatus.FAILED:
                                return Content(System.Net.HttpStatusCode.Conflict, response);
                            case HelperLiquidationLogStatus.ABORTED:
                                return Content(System.Net.HttpStatusCode.PreconditionFailed, response);
                        }
                    }

                    return Content(System.Net.HttpStatusCode.BadRequest, response);
                }
            }

            catch (Exception e)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, ResponseBase<GetLiquidationStatusResponse>.Create(new List<string>()
                {
                    "Error interno del servidor", e.Message
                }));
            }
        }
    }
}