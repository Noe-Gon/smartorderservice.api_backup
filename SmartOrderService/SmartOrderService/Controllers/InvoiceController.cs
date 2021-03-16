using SmartOrderService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SmartOrderService.Models.Requests;
using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.DTO;
using System.Net.Http.Headers;
using System.IO;
using System.Drawing.Imaging;
using SmartOrderService.Models.DTO.Invoice;

namespace SmartOrderService.Controllers
{
    public class InvoiceController : ApiController
    {
        // GET: api/Invoice
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Invoice/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet,Route("api/invoice/{id}/image")]
        public HttpResponseMessage GetImage(int id)
        {
            HttpResponseMessage response;

            try
            {
                var service = new InvoiceService();
                
                var content = service.getImageFromOpeFactura(id);

                response = new HttpResponseMessage(HttpStatusCode.OK);

                MemoryStream memoryStream = new MemoryStream(content);
                

                response.Content = new ByteArrayContent(memoryStream.ToArray());

                response.Content.Headers.ContentLength = memoryStream.Length;

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                

            }
            catch (EntityNotFoundException e)
            {

                response = Request.CreateResponse(HttpStatusCode.NotFound,"No se encontro la factura");
            }
            catch (Exception e)
            {
                
                response = Request.CreateResponse(HttpStatusCode.InternalServerError,e.Message);
            }


            return response;
        }

        // POST: api/Invoice
        public HttpResponseMessage Post([FromBody]InvoiceRequest Sale)
        {
            HttpResponseMessage response;

            try {
                List<InvoiceDto> invoices = new InvoiceService().createInvoiceInOpeFactura(Sale.SaleId, Sale.InvoiceData);
                response = Request.CreateResponse(HttpStatusCode.OK, invoices);
            }
            catch(InvoiceException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotAcceptable, "No se puede crear facturas. " + e.Message);
            }
            catch(ServerErrorException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Error en el servidor de facturacion: " + e.Message);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos..." + e.Message);
            }

            return response;
        }

        [HttpPost, Route("Api/Invoice/create")]
        public HttpResponseMessage CreateInvoice([FromBody]InvoiceRequest Sale)
        {
            HttpResponseMessage response;

            try
            {
                List<InvoiceDto> invoices = new InvoiceService().createInvoiceInOpeFactura2(Sale.SaleId, Sale.InvoiceData);
                response = Request.CreateResponse(HttpStatusCode.OK, invoices);
            }
            catch (InvoiceException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotAcceptable, "No se puede crear facturas. " + e.Message);
            }
            catch (ServerErrorException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Error en el servidor de facturacion: " + e.Message);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos..." + e.Message);
            }

            return response;
        }

        [HttpPost, Route("Api/v2/Invoice/create")]
        public HttpResponseMessage CreateInvoiceV2([FromBody]InvoiceRequest Sale)
        {
            HttpResponseMessage response;

            try
            {
                List<InvoiceDto> invoices = new InvoiceService().createInvoiceInOpeFacturaV2(Sale.SaleId, Sale.InvoiceData);
                response = Request.CreateResponse(HttpStatusCode.OK, invoices);
            }
            catch (InvoiceException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotAcceptable, "No se puede crear facturas. " + e.Message);
            }
            catch (ServerErrorException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Error en el servidor de facturacion: " + e.Message);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "ups lo arreglaremos..." + e.Message);
            }

            return response;
        }


        // PUT: api/Invoice/5
        [HttpPut, Route("Api/Invoice/{InvoiceId}/stamp")]
        public HttpResponseMessage Put(int InvoiceId)
        {
            HttpResponseMessage response;

            try
            {
                InvoiceService service = new InvoiceService();
                var code = service.StampInOpeFactura(InvoiceId);

                if (code == HttpStatusCode.Created)
                    response = Request.CreateResponse(HttpStatusCode.OK);
                else
                    response = Request.CreateResponse(code);
            }
            catch (InvoiceException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "No se puede facturar: " + e.Message);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError,"lo estamos arreglando: " +e.Message);
            }
            return response;
        }

        // DELETE: api/Invoice/5
        public void Delete(int id)
        {
        }

        [HttpPut,Route("api/Branch/{BranchCode}/Route/{RouteCode}/Customer/{CustomerId}/Register/Billing")]
        public HttpResponseMessage RegisterCustomer(int CustomerId, string BranchCode, string RouteCode)
        {
            HttpResponseMessage response;
            try
            {
                new InvoiceService().RegisterCustomerInvoice(CustomerId, BranchCode, RouteCode);
                response = Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (CustomerNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Cliente no encontrado");
            }
            catch (CustomerException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "No hay datos extendidos del cliente");
            }
            catch (RegisterCustomerBillingException e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "lo estamos arreglando: " + e.Message);
            }
           
            return response;
        }

        
    }
}
