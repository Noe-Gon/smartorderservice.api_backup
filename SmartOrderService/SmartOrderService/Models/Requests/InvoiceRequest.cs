using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class InvoiceRequest
    {
        public int SaleId { get; set; }
        public CustomerInvoiceDataRequest InvoiceData { set; get; }

    }
}