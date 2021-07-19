using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_consumer
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string Phone { get; set; }
        [MaxLength(20)]
        public string Phone_2 { get; set; }
        public int Status { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        [MaxLength(200)]
        public string CFECode { get; set; }
        [MaxLength(100)]
        public string CodePlace { get; set; }
        [MaxLength(200)]
        public string ReferenceCode { get; set; }
        [MaxLength(250)]
        public string Street { get; set; }
        [MaxLength(100)]
        public string ExternalNumber { get; set; }
        [MaxLength(100)]
        public string InteriorNumber { get; set; }
        [MaxLength(200)]
        public string Crossroads { get; set; }
        [MaxLength(200)]
        public string Crossroads_2 { get; set; }
        [MaxLength(100)]
        public string Neighborhood { get; set; }
        public bool IsMailingActive { get; set; }
        public bool IsMSMActive { get; set; }
        public int CounterVisitsWithoutSales { get; set; }
        public bool AcceptedTermsAndConditions { get; set; }

        #region Relationship
        public int CustomerId { get; set; }
        public so_customer Customer { get; set; }
        #endregion

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_consumer_removal_request> ConsumerRemovalRequests { get; set; }
    }
}