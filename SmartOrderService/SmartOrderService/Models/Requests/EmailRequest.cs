using System;
using System.Collections.Generic;
using System.Data;

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
        public string CustomerFullName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime Date { get; set; }
        public string RouteAddress { get; set; }
        public string SellerName { get; set; }
        public string PaymentMethod { get; set; }
        public string CancelTicketLink { get; set; }
        public List<SendTicketDigitalEmailSales> Sales { get; set; }
        //public DataTable dtTicket { get; set; }
    }

    public class SendCancelTicketDigitalEmailRequest
    {
        public string CustomerName { get; set; }
        public string CustomerFullName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime Date { get; set; }
        public string RouteAddress { get; set; }
        public string SellerName { get; set; }
        public string PaymentMethod { get; set; }
        public List<SendTicketDigitalEmailSales> Sales { get; set; }
        //public DataTable dtTicket { get; set; }
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

    public class SendRemovalRequestEmailRequest
    {
        public List<SendRemovalRequestTable> Table { get; set; }
        public List<string> LeaderEmail { get; set; }
    }

    public class SendRemovalRequestTable
    {
        public string CFECode { get; set; }
        public string ConsumerName { get; set; }
        public string Route { get; set; }
        public string ImpulsorName { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }
    }

    #region API EMAIL
    public class APIEmailSendEmailRequest
    {
        public string To { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
    }

    public class APIEmailSendEmailToManyUsersRequest
    {
        public List<string> To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
    #endregion
}