using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO.Invoice
{
    public class InvoiceOpeFacturaDto
    {
        public InvoiceHeaderOpeFacturaDto header = new InvoiceHeaderOpeFacturaDto();
        public List<Object> lines = new List<Object>();
    }
}