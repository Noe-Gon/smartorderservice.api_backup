using Newtonsoft.Json;
using OpeCDLib.CustomExceptions;
using OpeCDLib.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpeCDLib.Services
{
    public class DownloadService
    {

        
        public string URL { get; set; }

        public DownloadService(string URL)
        {

            this.URL = URL;
        }

        public List<Venta> DownloadSales(int Branchcode,int UserCode,DateTime Date, int Trip, bool Unmodifiable) {

            var client = new RestClient(URL);

            List<Venta> Ventas;
    
            var request = new RestRequest("API/sales", Method.GET);
            request.AddParameter("BranchCode", Branchcode);
            request.AddParameter("UserCode", UserCode);
            request.AddParameter("Date", Date.ToString("yyyy-MM-dd"));
            request.AddParameter("Trip", Trip);
            request.AddParameter("Unmodifiable", Unmodifiable);
            
            IRestResponse response = client.Execute(request);

            if (response.StatusCode.CompareTo(HttpStatusCode.NoContent) == 0) {

                throw new NoSalesException();
            }

            var content = response.Content;
            Ventas = JsonConvert.DeserializeObject<List<Venta>>(content);

            return Ventas;
        }


        public List<Jornada> ReadJourneys(int BranchCode,DateTime Date) {
            var client = new RestClient(URL);

            List<Jornada> Jornadas = new List<Jornada>();

            var request = new RestRequest("API/WorkDay", Method.GET);
            request.AddParameter("BranchCode", BranchCode);
            request.AddParameter("Date", Date.ToString("yyyy-MM-dd"));

            IRestResponse response = client.Execute(request);

            if (response.StatusCode.CompareTo(HttpStatusCode.OK) == 0)
            {
                var content = response.Content;
                Jornadas = JsonConvert.DeserializeObject<List<Jornada>>(content);
            }
            
            return Jornadas;

        }

        public Jornada ReadJourney(int BranchCode,int UserCode, DateTime Date)
        {
            var client = new RestClient(URL);

            List<Jornada> Jornadas = new List<Jornada>();

            var request = new RestRequest("API/WorkDay", Method.GET);
            request.AddParameter("BranchCode", BranchCode);
            request.AddParameter("UserCode", UserCode);
            request.AddParameter("Date", Date.ToString("yyyy-MM-dd"));

            IRestResponse response = client.Execute(request);

            if (response.StatusCode.CompareTo(HttpStatusCode.OK) == 0)
            {
                var content = response.Content;
                Jornadas = JsonConvert.DeserializeObject<List<Jornada>>(content);
            }

            var jornada = Jornadas.Any() ? Jornadas.FirstOrDefault() : new Jornada();

            return jornada;

        }

        public List<Visita> DownloadVisit(int Branchcode, int UserCode, DateTime LastSync)
        {

            var client = new RestClient(URL);

            List<Visita> Visits;

            var request = new RestRequest("api/BinnacleVisit", Method.GET);
            request.AddParameter("BranchCode", Branchcode);
            request.AddParameter("UserCode", UserCode);
            request.AddParameter("LastSync", LastSync.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            IRestResponse response = client.Execute(request);

            if (response.StatusCode.CompareTo(HttpStatusCode.NoContent) == 0)
                throw new NoSalesException();

            var content = response.Content;
            Visits = JsonConvert.DeserializeObject<List<Visita>>(content);

            return Visits;
        }

    }
}
