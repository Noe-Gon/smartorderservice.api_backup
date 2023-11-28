using Newtonsoft.Json;
using RestSharp;
using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.Responses;
using System;
using System.Linq;
using System.Net;

namespace SmartOrderService.Utils
{
    public static class StatusCodeValidatorPreSales
    {
        public static void ValidateClosingPreclosing(IRestResponse response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                try
                {
                    var des = JsonConvert.DeserializeObject<ClosingPreclosingErrorRsponse>(response.Content);
                    throw new ApiPreventaException(FormartException(des));
                }
                catch (Exception e)
                {
                    string parameters = "";
                    if (response.Request.Parameters.Count > 0)
                    {
                        var lstFormat = response.Request.Parameters.Select(x => x.Name + ":" + x.Value).ToList();
                        parameters = string.Join(",", lstFormat);
                    }
                    if (response.Request.Method == Method.POST)
                    {
                        throw new InternalServerException($"Ha ocurrido un error desconocido en {response.ResponseUri}. Parámetros = {parameters}");
                    }
                }
            }
        }

        private static string FormartException(ClosingPreclosingErrorRsponse response)
        {
            string errorMessage;
            string errorCode = "Sin código de error proporcionado";
            if (response.code != null)
            {
                errorCode = response.code.ToString();
            }
            if (response.data != null && response.data.Count > 0)
            {
                errorMessage = response.data[0].description;
            }
            else
            {
                errorMessage = response.message;
            }
            if (string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "No se ha obtenido mensaje de error por parte de API_Preventa";
            }

            string format = response.code + " - " + errorMessage;
            return format;
        }
    }
}