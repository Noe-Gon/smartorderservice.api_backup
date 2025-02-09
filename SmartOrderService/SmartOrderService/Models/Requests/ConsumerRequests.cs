﻿using System;
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
        public Guid? Neighborhood { get; set; }
        public string NeighborhoodName { get; set; }
        public List<int> Days { get; set; }
        public Guid? CountryId { get; set; }
        public string CountryName { get; set; }
        public Guid? StateId { get; set; }
        public string StateName { get; set; }
        public Guid? MunicipalityId { get; set; }
        public string MunicipalityName { get; set; }
    }

    public class UpdateConsumerRequest
    {
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public int? OriginalCustomerId { get; set; }
        public int RouteId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Email_2 { get; set; }
        public string Phone { get; set; }
        public string Phone_2 { get; set; }
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
        public Guid? Neighborhood { get; set; }
        public bool IsActive { get; set; }
        public List<int> Days { get; set; }
        public Guid? CountryId { get; set; }
        public Guid? StateId { get; set; }
        public Guid? MunicipalityId { get; set; }
        public string NeighborhoodName { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string MunicipalityName { get; set; }
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

    public class GetStatesRequest
    {
        public Guid? CountryId { get; set; }
    }

    public class GetMunicipalitiesRequest
    {
        public Guid? CountryId { get; set; }
        public Guid? StateId { get; set; }
    }

    public class GetNeighborhoodsRequest
    {
        public Guid? CountryId { get; set; }
        public Guid? StateId { get; set; }
        public Guid? MunicipalityId { get; set; }
    }

    public class CRMConsumerRequest
    {
        public Guid? EntityId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string CFECode { get; set; }
        public string Street { get; set; }
        public string ExternalNumber { get; set; }
        public string InteriorNumber { get; set; }
        public string Crossroads { get; set; }
        public string Crossroads_2 { get; set; }
        public Guid? Neighborhood { get; set; }
        public List<int> Days { get; set; }
        public Guid? CountryId { get; set; }
        public Guid? StateId { get; set; }
        public Guid? MunicipalityId { get; set; }
        public string Address { get; set; }
        public int? PriceListId { get; set; }
        public Guid? RouteCRMId { get; set; }
        public string StateIdName { get; set; }
        public string CountryIdName { get; set; }
        public string MunicipalityIdName { get; set; }
        public string NeighborhoodIdName { get; set; }
        public Guid? FiguraId { get; set; }
    }

    public class RemoveConsumerRequest
    {
        public int CustomerId { get; set; }
    }

    #region UnifiedTempCustomer

    public class UnifiedTempCustomerRequest
    {
        public int CustomerId { get; set; }
        public int TempCustomerId { get; set; }
    }

    #endregion

}