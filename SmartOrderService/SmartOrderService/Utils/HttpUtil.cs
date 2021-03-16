using Newtonsoft.Json;
using RestSharp;
using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.DTO.Invoice;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;

namespace SmartOrderService.Utils
{
    public class HttpUtil
    {

        public static List<InvoiceOpeFacturaResponse> CreateInvoicesInOpeFactura(string Url, InvoiceOpeFacturaDto Dto)
        {
            
                var client = new RestClient(Url);
                var request = new RestRequest("api/invoice", Method.POST);

                request.AddHeader("Content-Type", "application/json");
                request.AddJsonBody(Dto);
                request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            
                IRestResponse<List<InvoiceOpeFacturaResponse >> response = client.Execute<List<InvoiceOpeFacturaResponse>>(request);

                if(response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    return response.Data;
                }
                else
                {
                    throw new ServerErrorException(response.Content);
                }
            
        }

        public static List<InvoiceOpeFacturaResponse> CreateInvoicesInOpeFacturaV2(string Url, InvoiceOpeFacturaDto Dto)
        {

            var client = new RestClient(Url);
            var request = new RestRequest("api/sale", Method.POST);

            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(Dto);
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

            IRestResponse<List<InvoiceOpeFacturaResponse>> response = client.Execute<List<InvoiceOpeFacturaResponse>>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return response.Data;
            }
            else
            {
                throw new ServerErrorException(response.Content);
            }

        }

        public static HttpStatusCode StampInOpeFactura(string Url, int InvoiceId)
        {
            var client = new RestClient(Url);
            var request = new RestRequest("api/invoice/{InvoiceId}", Method.PUT);

            var dto = new InvoiceOpeFacturaRequest() { folio_interno = InvoiceId };

            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(dto);
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
            request.AddUrlSegment("InvoiceId", Convert.ToString(InvoiceId));

            IRestResponse response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.Created)
                throw new InvoiceException("Error en el servidor de facturacion " + response.Content);
            return response.StatusCode;
        }

        public static byte[] GetImageOpeFactura(string Url, int InvoiceId)
        {
            var client = new RestClient(Url);
            var request = new RestRequest("api/invoice/{InvoiceId}", Method.GET);
            request.AddUrlSegment("InvoiceId", Convert.ToString(InvoiceId));

            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
                return response.RawBytes;
            throw new Exception("Error en el servidor de facturacion " + response.StatusCode);

        }
    }
}