using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class ApiVersionRequest
    {
        public string UserCode { get; set; }
        public string Branchcode { get; set; }
        public string PackageName { get; set; }

    }
}