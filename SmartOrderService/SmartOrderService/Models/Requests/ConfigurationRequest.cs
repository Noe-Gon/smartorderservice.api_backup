using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class ConfigurationRequest
    {
        public string BranchCode { get; set; }
        public string UserCode { get; set; }
    }
}