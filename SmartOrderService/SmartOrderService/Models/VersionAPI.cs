using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models
{
    public class VersionAPI
    {
        public String Version = "2020.10.16";
        public String PublishDate = "2020-10-16";
        public List<string> ChangeLog = new List<string>();
        public List<string> Features = new List<string>();

        public static VersionAPI getVersionAPI()
        {
            return new VersionAPI();
        }

        public VersionAPI()
        {
            fillChangeLog();
            fillFeatures();
        }

        private void fillChangeLog()
        {
            ChangeLog.Add("El endpoint de facturacion para opecdbi siempre devuelve OK");

        }

        private void fillFeatures()
        {
            Features.Add("Se agrega el soporte a precio viajero");
        }
    }
}