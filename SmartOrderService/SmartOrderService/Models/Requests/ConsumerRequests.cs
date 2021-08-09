using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Requests
{
    public class InsertConsumerRequest
    {
        public int UserId { get; set; }
        public int RouteId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Email_2 { get; set; }
        public string Phone { get; set; }
        public string Phone_2 { get; set; }
        public int Status { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string CFECode { get; set; }
        public int? CodePlace { get; set; }
        public string ReferenceCode { get; set; }
        public string Street { get; set; }
        public string ExternalNumber { get; set; }
        public string InteriorNumber { get; set; }
        public string Crossroads { get; set; }
        public string Crossroads_2 { get; set; }
        public string Neighborhood { get; set; }
        public List<int> Days { get; set; }
    }

    public class UpdateConsumerRequest
    {
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public int RouteId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Email_2 { get; set; }
        public string Phone { get; set; }
        public string Phone_2 { get; set; }
        public int Status { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string CFECode { get; set; }
        public int? CodePlace { get; set; }
        public string ReferenceCode { get; set; }
        public string Street { get; set; }
        public string ExternalNumber { get; set; }
        public string InteriorNumber { get; set; }
        public string Crossroads { get; set; }
        public string Crossroads_2 { get; set; }
        public string Neighborhood { get; set; }
        public bool IsActive { get; set; }
        public List<int> Days { get; set; }
    }

    public class ConsumerRemovalRequestRequest
    {
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public string Reason { get; set; }
    }

    public class GetConsumersRequest
    {
        public int userId { get; set; }
    }

    #region Resend TicketDigital
    public class ResendTicketDigitalRequest
    {
        public int SaleId { get; set; }
        public string PaymentMethod { get; set; }
    }
    #endregion

    public class ReactivationTicketDigitalRequest
    {
        public int CustomerId { get; set; }
        public string CustomerEmail { get; set; }
    }
}