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

    public class SendTicketDigitalEmailRequest
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime Date { get; set; }
        public string RouteAddress { get; set; }
        public string SellerName { get; set; }
        public string PaymentMethod { get; set; }
        public List<SendTicketDigitalEmailSales> Sales { get; set; }
    }

    public class SendTicketDigitalEmailSales
    {
        public string ProductName { get; set; }
        public int Amount { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
    }

    public class SendReactivationTicketDigitalRequest
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string TermsAndConditionLink { get; set; }
    }

}