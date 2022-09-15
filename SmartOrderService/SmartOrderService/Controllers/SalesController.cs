using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SmartOrderService.DB;
using SmartOrderService.Mappers;
using SmartOrderService.Models;
using SmartOrderService.Models.Requests;
using SmartOrderService.Services;
using SmartOrderService.Models.Responses;
using SmartOrderService.CustomExceptions;
using SmartOrderService.Utils;
using SmartOrderService.Models.DTO;
using Newtonsoft.Json;
using System.Text;
using SmartOrderService.Models.Message;

namespace SmartOrderService.Controllers
{
    public class SalesController : ApiController
    {
        private SmartOrderModel db = new SmartOrderModel();

        private static SaleService objectService = new SaleService();
        private static Dictionary<string, SaleService> mapObjectService = new Dictionary<string, SaleService>();
        SaleService service;

        [HttpGet, Route("api/sales/{SaleId}/Lines")]
        public HttpResponseMessage getLines(int SaleId, [FromUri] InvoiceDataDto invoiceData)

        {
            try
            {
                var opeFacturaDto = new InvoiceService().getInvoiceOpeFacturaDto(SaleId, invoiceData);

                return Request.CreateResponse(HttpStatusCode.OK, opeFacturaDto);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
            }

        }

        [HttpGet, Route("api/sales/{SaleId}/ConvertLines")]
        public HttpResponseMessage convertToLine(int SaleId, [FromUri] InvoiceDataDto invoiceData)

        {
            try
            {
                var opeFacturaDto = new InvoiceService().createOpeFacturaDto(SaleId, invoiceData);

                return Request.CreateResponse(HttpStatusCode.OK, opeFacturaDto);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
            }

        }

        [HttpGet, Route("api/v2/sales/{SaleId}/ConvertLines")]
        public HttpResponseMessage convertToLinev2(int SaleId)

        {
            try
            {
                var i = Request.Headers.GetValues("InvoiceData");
                var invoicedto = i.FirstOrDefault().ToString();
                var invoiceData = JsonConvert.DeserializeObject<InvoiceDataDto>(invoicedto);
                var opeFacturaDto = new InvoiceService().createOpeFacturaDtoV2(SaleId, invoiceData);

                return Request.CreateResponse(HttpStatusCode.OK, opeFacturaDto);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
            }

        }

        // GET: api/Sales
        public HttpResponseMessage Getso_sale([FromUri] SaleRequest request)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.NoContent);


            var date = DateUtils.getDateTime(request.Date);

            service = new SaleService();
            try
            {
                if (request.BranchCode != null && request.UserCode != null)
                {

                    var sales = service.getSalesByRoute(request.BranchCode, request.UserCode, request.Trip, date, request.Unmodifiable);
                    response = Request.CreateResponse(HttpStatusCode.OK, sales);
                }

            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }

            return response;
        }

        // GET: api/Sales/5
        [ResponseType(typeof(so_sale))]
        public IHttpActionResult Getso_sale(int id)
        {
            so_sale so_sale = db.so_sale.Find(id);
            if (so_sale == null)
            {
                return NotFound();
            }

            return Ok(so_sale);
        }

        // PUT: api/Sales/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putso_sale(int id, so_sale so_sale)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != so_sale.saleId)
            {
                return BadRequest();
            }

            db.Entry(so_sale).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!so_saleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Sales
        [ResponseType(typeof(so_sale))]
        public IHttpActionResult Postso_sale(Sale sale)
        {
            SaleService service = new SaleService();
            service.create(sale);

            if (sale.SaleId == 0)
            {
                return BadRequest("parametros incorrectos");
            }

            return CreatedAtRoute("DefaultApi", new { id = sale.SaleId }, sale);

            //return StatusCode(HttpStatusCode.Created);
        }

        // POST: api/Sales
        [ResponseType(typeof(so_sale))]
        [HttpPost, Route("api/sales/saleteam")]
        public IHttpActionResult so_sale_team(Sale sale)
        {
            IHttpActionResult responseActionResult;
            HttpResponseMessage responseMessage;
            Sale saleResult = new Sale();
            try
            {
                lock (objectService) {
                    saleResult = objectService.SaleTeamTransaction(sale);
                }
            }
            catch (ProductNotFoundBillingException e)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.NotFound, e.Message);
                responseActionResult = ResponseMessage(responseMessage);
                return responseActionResult;
            }
            catch (BadRequestException e)
            {
                return BadRequest();
            }
            catch (EmptySaleException e)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
                responseActionResult = ResponseMessage(responseMessage);
                return responseActionResult;
            }
            catch (Exception e)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
                responseActionResult = ResponseMessage(responseMessage);
                return responseActionResult;
            }

            responseMessage = Request.CreateResponse(HttpStatusCode.OK, saleResult);
            responseActionResult = ResponseMessage(responseMessage);
            return responseActionResult;

        }

        [ResponseType(typeof(so_sale))]
        [HttpPost, Route("api/sales/saleTeam_v2")]
        public IHttpActionResult so_sale_team_v2(SaleTeam sale)
        {
            IHttpActionResult responseActionResult;
            HttpResponseMessage responseMessage;
            SaleTeam saleResult = new SaleTeam();
            try
            {
                SaleService serviceLock = null;
                RouteTeamService servce = new RouteTeamService();
                var routeId = servce.searchRouteId(sale.UserId);
                if (mapObjectService.ContainsKey(routeId.ToString()))
                {
                    serviceLock = mapObjectService[routeId.ToString()];
                }
                else
                {
                    serviceLock = new SaleService();
                    mapObjectService.Add(routeId.ToString(), serviceLock);
                }
                if (serviceLock == null)
                    serviceLock = objectService;

                lock (serviceLock)
                {
                    saleResult = serviceLock.SaleTeamTransaction(sale);
                    if(sale.EmailDeliveryTicket ?? false)
                        serviceLock.SenTicketDigital(new SendTicketDigitalRequest()
                        {
                            SaleId = saleResult.SaleId,
                            Email = sale.Email
                        });
                }
            }
            catch (ProductNotFoundBillingException e)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.NotFound, e.Message);
                responseActionResult = ResponseMessage(responseMessage);
                return responseActionResult;
            }
            catch (BadRequestException e)
            {
                return BadRequest();
            }
            catch (ApiPreventaException e)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.MethodNotAllowed, e.Message);
                responseActionResult = ResponseMessage(responseMessage);
                return responseActionResult;
            }
            catch (Exception e)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
                responseActionResult = ResponseMessage(responseMessage);
                return responseActionResult;
            }

            responseMessage = Request.CreateResponse(HttpStatusCode.OK, saleResult);
            responseActionResult = ResponseMessage(responseMessage);
            return responseActionResult;

        }

        [ResponseType(typeof(so_sale))]
        [HttpPost, Route("api/sales/saleTeam_v3")]
        public IHttpActionResult so_sale_team_v3(SaleTeamWithPoints sale)
        {
            IHttpActionResult responseActionResult;
            HttpResponseMessage responseMessage;
            SaleTeamWithPoints saleResult = new SaleTeamWithPoints();
            try
            {
                lock (objectService)
                {
                    saleResult = objectService.SaleTeamTransactionWithPoints(sale);
                }
            }
            catch (ProductNotFoundBillingException e)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.NotFound, e.Message);
                responseActionResult = ResponseMessage(responseMessage);
                return responseActionResult;
            }
            catch (BadRequestException e)
            {
                return BadRequest();
            }
            catch (ApiPreventaException e)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.MethodNotAllowed, e.Message);
                responseActionResult = ResponseMessage(responseMessage);
                return responseActionResult;
            }
            catch (EmptySaleException e)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.PreconditionFailed, e.Message);
                responseActionResult = ResponseMessage(responseMessage);
                return responseActionResult;
            }
            catch (Exception e)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
                responseActionResult = ResponseMessage(responseMessage);
                return responseActionResult;
            }

            responseMessage = Request.CreateResponse(HttpStatusCode.OK, saleResult);
            responseActionResult = ResponseMessage(responseMessage);
            return responseActionResult;

        }

        [HttpDelete, Route("api/sales/saleteam")]
        public HttpResponseMessage Deleteso_sale_team(int id)
        {
            HttpResponseMessage response;

            try
            {
                var service = new SaleService();
                var sale = service.Delete(id);
                service.RestoreInventoryAvailability(id);
                response = Request.CreateResponse(HttpStatusCode.OK, sale);
            }
            catch (DeviceNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "no estas autorizado");
            }
            catch (EntityNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "la venta no fue afectada");
            }

            return response;
        }

        [HttpDelete, Route("api/sales/saleteam_v2")]
        public HttpResponseMessage Deleteso_sale_team_v2(int id, string PaymentMethod)
        {
            HttpResponseMessage response;

            try
            {
                var service = new SaleService();
                var sale = service.Cancel(id, PaymentMethod);
                service.SenTicketDigital(new SendTicketDigitalRequest()
                {
                    SaleId = sale.SaleId,
                    Email = null
                });
                response = Request.CreateResponse(HttpStatusCode.OK, sale);
            }
            catch (DeviceNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "no estas autorizado");
            }
            catch (EntityNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "la venta no fue afectada");
            }

            return response;
        }

        [HttpDelete, Route("api/sales/saleteam_v3")]
        public HttpResponseMessage Deleteso_sale_team_v3(int id, string PaymentMethod)
        {
            HttpResponseMessage response;

            try
            {
                var service = new SaleService();
                var sale = service.Cancel_v3(id, PaymentMethod);
                response = Request.CreateResponse(HttpStatusCode.OK, sale);
            }
            catch (DeviceNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized, "no estas autorizado");
            }
            catch (EntityNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "la venta no fue afectada");
            }

            return response;
        }

        [HttpGet, Route("api/sales/PartnerSale/{UserId}/User/{InventoryId}/Inventory/{CustomerId}/Customer")]
        public HttpResponseMessage PartnerSale(int UserId, int InventoryId, int CustomerId)

        {
            try
            {
                var SaleDto = new SaleService().GetSaleTeam(UserId, InventoryId, CustomerId);

                return Request.CreateResponse(HttpStatusCode.OK, SaleDto);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
            }
        }

        // DELETE: api/Sales/5

        public HttpResponseMessage Delete(int id)
        {
            HttpResponseMessage response;

            try
            {
                //var token = Request.Headers.GetValues("OS_TOKEN").FirstOrDefault();

                //var user = new UserService().getUserByToken(token);

                var service = new SaleService();
                var sale = service.Delete(id);

                response = Request.CreateResponse(HttpStatusCode.OK,sale);
            }
            catch (DeviceNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Unauthorized,"no estas autorizado");
            }
            catch (EntityNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError,"la venta no fue afectada");
            }

            return response;
        }

        [HttpPost]
        [Route("~/api/sales/adjustment")]
        public IHttpActionResult SalesAdjustment([FromUri]int deleteSaleId, [FromBody]Sale newSale)
        {
            IHttpActionResult responseActionResult;
            HttpResponseMessage responseMessage;
            SaleAdjusmentResult saleResult = new SaleAdjusmentResult();
            try
            {
                saleResult.DeletedSale = service.Delete(deleteSaleId);
                lock (objectService)
                {
                    saleResult.NewSale = objectService.SaleTeamTransaction(newSale);
                }
            }
            catch (ProductNotFoundBillingException e)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.NotFound, e.Message);
                responseActionResult = ResponseMessage(responseMessage);
                return responseActionResult;
            }
            catch (BadRequestException e)
            {
                return BadRequest();
            }
            catch (DeviceNotFoundException e)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.Unauthorized, "no estas autorizado");
                responseActionResult = ResponseMessage(responseMessage);
                return responseActionResult;
            }
            catch (EntityNotFoundException e)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.NotFound);
                responseActionResult = ResponseMessage(responseMessage);
                return responseActionResult;
            }
            catch (Exception e)
            {
                responseMessage = Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
                responseActionResult = ResponseMessage(responseMessage);
                return responseActionResult;
            }

            responseMessage = Request.CreateResponse(HttpStatusCode.OK, saleResult);
            responseActionResult = ResponseMessage(responseMessage);
            return responseActionResult;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool so_saleExists(int id)
        {
            return db.so_sale.Count(e => e.saleId == id) > 0;
        }

        private String buildResponseProductAmount(List<Product> listProducts)
        {
            StringBuilder response = new StringBuilder("Los siguientes articulos no se pudieron registrar por falta de producto en el inventario: ");
            foreach (Product product in listProducts) {
                response.Append("\n" + product.productName + ", cantidad disponible: " + product.amount);
            }
            return response.ToString();
        }

        [HttpPost]
        [Route("~/api/sale/SendTicketDigital")]
        public IHttpActionResult SendTicketDigital(SendTicketDigitalRequest request)
        {
            try
            {
                var service = new SaleService();
                var response = service.SenTicketDigital(request);

                if (response.Status)
                    return Content(HttpStatusCode.OK, response);

                return Content(HttpStatusCode.BadRequest, response);
            }
            catch (EntityNotFoundException e)
            {
                var response = ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    e.Message
                });

                return Content(HttpStatusCode.InternalServerError, response);
            }
            catch (Exception e)
            {
                var response = ResponseBase<MsgResponseBase>.Create(new List<string>()
                {
                    e.Message
                });

                return Content(HttpStatusCode.InternalServerError, response); 
            }
        }
    }
}