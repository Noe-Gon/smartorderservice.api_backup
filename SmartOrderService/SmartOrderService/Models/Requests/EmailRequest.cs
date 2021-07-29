using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class WellcomeEmailRequest
    {
        public string CanceledLink { get; set; }
        public string TermsAndConditionLink { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
    }

    public class SendTicketDigitalEmail
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string TermsAndConditionLink { get; set; }
    }

    public class ThanksBuyingWithUs
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime BuyDate { get; set; }
        public string CustomerCode { get; set; }
        public string Vendedor { get; set; }
    }
}