using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class SaleRequest
    {
        public string UserCode { get; set; }
        public string BranchCode { get; set; }
        public string Date { get; set; }
        public int Trip { get; set; }
        public string CustomerCode { get; set; }
        public bool Unmodifiable { get; set; }
    }
}