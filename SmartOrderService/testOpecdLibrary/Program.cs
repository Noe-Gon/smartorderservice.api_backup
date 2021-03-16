using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpeCDLib.Services;

namespace testOpecdLibrary
{
    class Program
    {
        static void Main(string[] args)
        {

            var date = new DateTime(2016,10,10);

            var service = new DownloadService("http://pbasworkbycloudds.bepensa.com:83/WbcDSApi/");
           
           
            //var ventas = service.DownloadSales(30, 924, date, 1);
           /* var contador = 1;

            foreach (var venta in ventas)
            {
                Console.WriteLine("Venta al cliente {0}, con {1} detalles, contador {2}",venta.CUC,venta.Detalles.Count(),contador);
                contador++;

            }
            */

            //var jornadas = service.ReadJourneys(30, date);

            var jornada = service.ReadJourney(30, 924, date);



           Console.Read();
        }
    }
}
