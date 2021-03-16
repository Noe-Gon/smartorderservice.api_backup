using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO.Invoice
{
    public class InvoiceHeaderOpeFacturaDto
    {
        public int id = 0;
        public string opecdid = "";
        public string nota_venta = "";
        public string cuc = "";
        public string fecha = "";
        public int carga = 0;
        public string ruta = "";
        public string rfc = "";
        public string razon_social = "";
        public string direccion_fiscal = "";
        public int punto_venta = 0;
        public string codope = "";
        public string inventsite_id = "";
        public string siteId = "";
        public string supervisor = "";
        public string usoCFDI = "";
    }
}