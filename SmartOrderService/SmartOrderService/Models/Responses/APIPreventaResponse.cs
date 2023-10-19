using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Responses
{
    public class ClosingPreclosingResponse
    {
        public bool success { get; set; }
        public string result { get; set; }
        public int? code { get; set; }
        public string data { get; set; }
        public string message { get; set; }
    }

    public class ClosingPreclosingErrorRsponse
    {
        public bool success { get; set; }
        public string result { get; set; }
        public int? code { get; set; }
        public List<ClosingPreclosingData> data { get; set; }
        public string message { get; set; }
    }

    public class ClosingPreclosingData
    {
        public string type { get; set; }
        public string description { get; set; }
    }
}