using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Email
{
    public class SendAPIEmailrequest
    {
        public string To { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
    }
}