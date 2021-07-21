using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartOrderService.DB
{
    public class so_customer_additional_data
    {
        public int Id { get; set; }

        [MaxLength(200)]
        [Column("email2")]
        public string Email_2 { get; set; }

        [Column("phone1")]
        [MaxLength(20)]
        public string Phone { get; set; }

        [Column("phone2")]
        [MaxLength(20)]
        public string Phone_2 { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("cfe_code")]
        [MaxLength(200)]
        public string CFECode { get; set; }

        [Column("code_place")]
        [MaxLength(100)]
        public string CodePlace { get; set; }

        [Column("reference_code")]
        [MaxLength(200)]
        public string ReferenceCode { get; set; }

        [Column("interior_number")]
        [MaxLength(100)]
        public string InteriorNumber { get; set; }

        [Column("neighborhood")]
        [MaxLength(100)]
        public string Neighborhood { get; set; }

        [Column("is_mailing_active")]
        public bool IsMailingActive { get; set; }

        [Column("is_sms_active")]
        public bool IsSMSActive { get; set; }

        [Column("counter_visits_without_sales")]
        public int CounterVisitsWithoutSales { get; set; }

        [Column("accepted_terms_and_conditions")]
        public bool AcceptedTermsAndConditions { get; set; }

        #region Relationship
        [Column("customerId")]
        public int CustomerId { get; set; }
        public so_customer Customer { get; set; }
        #endregion

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_customer_removal_request> CustomerRemovalRequests { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_portal_links_log> PortalLinksLog { get; set; }
    }
}