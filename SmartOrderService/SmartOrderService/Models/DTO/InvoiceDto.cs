using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class InvoiceDto
    {
        public long InvoiceId { get; set; }
        public string Cia { get; set; }
        public string Folio { get; set; }
        public double Total { get; set; }
    }
}