using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class ClosingPreclosingDTO
    {
        public string branchCode { get; set; }
        public string routeCode { get; set; }
        public int version { get; set; }
    }
}