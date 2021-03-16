using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO.Invoice
{
    public class InvoiceLineOpeFacturaDto : SimpleInvoiceLineOpeFactura
    {
      
        public decimal importe_iva = 0;
        public decimal iva_total = 0;
        public decimal tasa_ieps = 0;
        public decimal importe_ieps = 0;
        public decimal importe_ieps_cuota = 0;
        public decimal tasa_ieps_botana = 0;
        public decimal importe_ieps_botana = 0;
        public decimal descuento_global = 0;
        public decimal litros_gravables = 0;
    }
}