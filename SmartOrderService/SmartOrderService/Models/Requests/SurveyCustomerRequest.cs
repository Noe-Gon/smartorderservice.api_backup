using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class SurveyCustomerRequest
    {
        public string route { set; get; }

        public int routeType { set; get; }

        public string code { set; get; }

        public string customer { set; get; }
        public string branch { set; get; }

        public string address { set; get; }
        public string data { set; get; }

        public string contactName { set; get; }
    }
}