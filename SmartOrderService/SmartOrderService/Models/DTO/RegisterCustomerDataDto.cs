using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    [Serializable]
    public class RegisterCustomerDataDto
    {
        public string Name { get; set; }
        public string RFC { get; set; }
        public string Street { get; set; }
        public string InteriorNumber { get; set; }
        public string OutdoorNumber { get; set; }
        public string PostalCode { get; set; }
        public string Colony { set; get; }
        public string Location { set; get; }
        public string Municipality { set; get; }
        public string State { set; get; }
        public string Country { set; get; }
        public string Email { set; get; }
        public string IsVirtual { set; get; }
        public string BranchName { set; get; }
        public string Observations { get; set; }
        public string AccountName { set; get; }
    }
}