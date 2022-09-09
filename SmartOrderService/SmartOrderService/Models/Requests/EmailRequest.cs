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
    #region Send Ticket Digital Email

    
    public class SendTicketDigitalEmailRequest
    {
        public string CustomerName { get; set; }
        public string CustomerFullName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerReferenceCode { get; set; }
        public DateTime Date { get; set; }
        public string RouteAddress { get; set; }
        public string SellerName { get; set; }
        public string PaymentMethod { get; set; }
        public string CancelTicketLink { get; set; }
        public List<SendTicketDigitalEmailSales> Sales { get; set; }
        public List<SendTicketDigitalEmailSalesWithPoints> SalesWithPoints { get; set; }
        public DataTable dtTicket { get; set; }
        public SendTicketDigitalEmailOrder Order { get; set; }
        public string ReferenceCode { get; set; }
    }

    public class SendTicketDigitalEmailOrder
    {
        public DateTime DeliveryDate { get; set; }
        public List<SendTicketDigitalEmailOrderDetail> OrderDetail { get; set; }
    }

    public class SendTicketDigitalEmailOrderDetail
    {
        public string ProductName { get; set; }
        public int Amount { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
    }
    #endregion
    #region Send Cancel Ticket Digital Email

    
    public class SendCancelTicketDigitalEmailRequest
    {
        public string CustomerName { get; set; }
        public string CustomerFullName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime Date { get; set; }
        public string RouteAddress { get; set; }
        public string SellerName { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceCode { get; set; }
        public List<SendTicketDigitalEmailSales> Sales { get; set; }
        public DataTable dtTicket { get; set; }
        public SendCancelTicketDigitalEmailOrder Order { get; set; }
    }

    public class SendCancelTicketDigitalEmailOrder
    {
        public DateTime DeliveryDate { get; set; }
        public List<SendCancelTicketDigitalEmailOrderDetail> OrderDetail { get; set; }
    }

    public class SendCancelTicketDigitalEmailOrderDetail
    {
        public string ProductName { get; set; }
        public int Amount { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
    }
    #endregion

    public class SendTicketDigitalEmailSales
    {
        public string ProductName { get; set; }
        public int Amount { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
    }

    public class SendTicketDigitalEmailSalesWithPoints
    {
        public string ProductName { get; set; }
        public int Amount { get; set; }
        public int UnitPrice { get; set; }
        public int TotalPrice { get; set; }
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

    public class SendOrderTicketRequest
    {
        public string CustomerName { get; set; }
        public string CustomerFullName { get; set; }
        public string RouteAddress { get; set; }
        public string SallerName { get; set; }
        public DateTime Date { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string CustomerMail { get; set; }
        public string ReferenceCode { get; set; }
        public bool Status { get; set; }
        public List<SendOrderTicketItem> Items { get; set; }
    }

    public class SendOrderTicketItem
    {
        public string ProductName { get; set; }
        public int Amount { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }

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