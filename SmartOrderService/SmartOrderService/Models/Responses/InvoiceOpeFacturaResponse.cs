using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Responses
{
    public class InvoiceOpeFacturaResponse
    {
        public int tipo_mensaje { get; set; }
        public string mensaje { get; set; }
        public string opecdid { get; set; }
        public string serie { get; set; }
        public string folio { get; set; }
        public int folio_interno { get; set; }
        public int cia { get; set; }
        public string cia_nombre { get; set; }
        public double total { get; set; }

    }
}