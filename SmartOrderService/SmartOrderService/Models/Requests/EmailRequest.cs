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
        public int AccumulatedPoints { get; set; }
        public int WonPoints { get; set; }
        public string ValidityPointsDates { get; set; }
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
        public string CancelTicketLink { get; set; }
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
        public string CancelTicketLink { get; set; }
        public List<SendOrderTicketItem> Items { get; set; }
    }

    public class SendOrderTicketItem
    {
        public string ProductName { get; set; }
        public int Amount { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }

    }

    #region Send BillPocket Report
    public class SendBillPocketReportEmailRequest
    {
        public string Email { get; set; }

        public string BranchName { get; set; }

        public string RouteName { get; set; }

        public string UserRole { get; set; }

        public DateTime WorkDayDate { get; set; }

        public DateTime SendDate { get; set; }

        public double TotalAmount { get; set; }

        public int TotalSales { get; set; }
    }
    #endregion

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

    #region Adjustment Email
    public class SendAdjustmentEmailRequest
    {
        public List<string> LidersEmail { get; set; }
        public DateTime Date { get; set; }
        public string RouteAddress { get; set; }
        public string Branch { get; set; }
        public string ImpulsorFullName { get; set; }
        public Double TotalAdjustment { get; set; }

        public List<SendAdjustmentEmailSales> Sales { get; set; }

        public SendAdjustmentEmailRequest()
        {
            LidersEmail = new List<string>();
            Sales = new List<SendAdjustmentEmailSales>();
            TotalAdjustment = 0;
        }
    }

    public class SendAdjustmentEmailSales
    {
        public string Coordenadas { get; set; }
        public string AjustmentReason { get; set; }
        public string Table { get; set; }
        public Double TotalAdjustment { get; set; }
    }

    public class SendAdjustmentEmailResponse
    {
        public string Table { get; set; }
        public Double Total { get; set; }
    }
    #endregion
}