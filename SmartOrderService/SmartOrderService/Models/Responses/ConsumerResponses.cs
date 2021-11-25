using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Responses
{
    public class InsertConsumerResponse
    {
        public int CustomerId { get; set; }
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
        public Guid? Neighborhood { get; set; }
        public List<int> Days { get; set; }
        public Guid? CountryId { get; set; }
        public Guid? StateId { get; set; }
        public Guid? MunicipalityId { get; set; }
    }

    public class UpdateConsumerResponse
    {
        public string Msg { get; set; }
    }

    public class ConsumerRemovalResponse
    {
        public string Msg { get; set; }
    }

    public class GetConsumersResponse
    {
        public int CustomerId { get; set; }
        public int Order { get; set; }
        public bool Visited { get; set; }
        public int RouteId { get; set; }
        public string Contact { get; set; }
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
        public Guid? CountryId { get; set; }
        public Guid? StateId { get; set; }
        public Guid? TownId { get; set; }
        public Guid? Neighborhood { get; set; }
        public int CounterVisitsWithoutSales { get; set; }
        public bool IsActive { get; set; }
        public List<int> Days { get; set; }
        public bool IsMailingActive { get; set; }
        public bool IsSMSActive { get; set; }
        public bool IsTermsAndConditionsAccepted { get; set; }
        public bool CanBeRemoved { get; set; }
    }

    public class ResendTicketDigitalResponse
    {
        public string Msg { get; set; }
    }

    public class ReactivationTicketDigitalResponse
    {
        public string Msg { get; set; }
    }

    public class GetCountriesResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class GetStatesResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class GetMunicipalitiesResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class GetNeighborhoodsResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class RemoveConsumerResponse
    {
        public string Msg { get; set; }
    }

}