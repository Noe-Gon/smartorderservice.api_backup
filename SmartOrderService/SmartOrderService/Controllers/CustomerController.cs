using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SmartOrderService.Models.Responses;
using SmartOrderService.Models.Requests;
using SmartOrderService.Services;
using SmartOrderService.Models.DTO;
using SmartOrderService.CustomExceptions;

namespace SmartOrderService.Controllers
{
    public class CustomerController : ApiController
    {

        public HttpResponseMessage Get([FromUri] CustomerRequest request)
        {
            HttpResponseMessage response;
            try
            {

                var Customers = new CustomerService().FindCustomers(request);
                response = Request.CreateResponse(HttpStatusCode.OK, Customers);
            }
            catch (CustomerNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Error: " + e.Message);
            }
            catch (InventoryEmptyException)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Error: no se han cargado los clientes a visitar en el recorrido, no hay inventario del día");
            }

            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Error: " + e.Message);
            }


            return response;
        }

        [HttpGet]
        [Route("~/api/CustomersWithVario")]
        public HttpResponseMessage GetCustomer([FromUri] CustomerWithVarioRequest request)
        {
            HttpResponseMessage response;
            try
            {

                var Customers = new CustomerService().FindCustomersWithVario(request);
                response = Request.CreateResponse(HttpStatusCode.OK, Customers);
            }
            catch (CustomerNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Error: " + e.Message);
            }
            catch (InventoryEmptyException)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, "Error: no se han cargado los clientes a visitar en el recorrido, no hay inventario del día");
            }

            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Error: " + e.Message);
            }

            return response;
        }

        [HttpGet, Route("api/Customer/Data/GetByRoute/{RouteId}")]
        public HttpResponseMessage GetDataByRoute(int RouteId)
        {
            HttpResponseMessage response;
            try
            {
                var Customers = new CustomerService().getDataByRoute(RouteId);
                response = Request.CreateResponse(HttpStatusCode.OK, Customers);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
            }


            return response;
        }

        [HttpPut, Route("api/Customer/Data/Status/{CustomerId}")]
        public HttpResponseMessage PutStatus(int CustomerId, CustomerDataDto Customer)
        {
            HttpResponseMessage response;
            try
            {
                new CustomerService().SetStatus(CustomerId, Customer);
                response = Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
            }

            return response;
        }

        [HttpPut, Route("api/Customer/{CustomerId}/SetUpBilling")]
        public HttpResponseMessage SetUpBilling(int CustomerId, [FromBody] CustomerRequest request)
        {
            /*HttpResponseMessage response;
            try
            {
                new CustomerService().SetUpBilling(CustomerId, request.BranchCode, request.RouteCode, request.SetUpBilling);
                response = Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (CustomerBillingDataNotFoundException e)
            {
                response = Request.CreateResponse(HttpStatusCode.NotFound, "Error: no existen los datos de facturación del cliente");
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }

            return response;*/
            return Request.CreateResponse(HttpStatusCode.OK);
        }


        [HttpGet, Route("api/Customer/{CustomerId}/InvoiceData")]
        public HttpResponseMessage GetInvoiceData(int CustomerId, [FromUri] string routeCode = null, [FromUri] string branchCode = null)
        {
            HttpResponseMessage response;
            try
            {
                var InvoiceData = new CustomerService().FindCustomerInvoiceData(CustomerId, routeCode, branchCode);
                response = Request.CreateResponse(HttpStatusCode.OK, InvoiceData);
            }
            catch (Exception e)
            {
                response = Request.CreateResponse(HttpStatusCode.Conflict, e.Message);
            }


            return response;
        }

        // GET: api/Customer/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Customer
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Customer/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Customer/5
        public void Delete(int id)
        {
        }
    }
}
