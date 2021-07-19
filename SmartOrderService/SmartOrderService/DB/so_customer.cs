namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_customer()
        {
            so_binnacle_visit = new HashSet<so_binnacle_visit>();
            so_category_restriction = new HashSet<so_category_restriction>();
            so_collect_bottle = new HashSet<so_collect_bottle>();
            so_container_customer = new HashSet<so_container_customer>();
            so_customer_products_price_list = new HashSet<so_customer_products_price_list>();
            so_customer_promotion_config = new HashSet<so_customer_promotion_config>();
            so_route_customer = new HashSet<so_route_customer>();
            so_sale = new HashSet<so_sale>();
            so_reception_bottle = new HashSet<so_reception_bottle>();
            so_tag = new HashSet<so_tag>();
            so_customer_data = new HashSet<so_customer_data>();
        }

        [Key]
        public int customerId { get; set; }

        [Required]
        [StringLength(50)]
        public string code { get; set; }

        [Required]
        public string contact { get; set; }

        [Required]
        public string name { get; set; }

        public string address { get; set; }

        public string description { get; set; }

        public double? latitude { get; set; }

        public double? longitude { get; set; }

        public string email { get; set; }
        public string email_2 { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_binnacle_visit> so_binnacle_visit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_category_restriction> so_category_restriction { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_collect_bottle> so_collect_bottle { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_container_customer> so_container_customer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_customer_data> so_customer_data { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_customer_products_price_list> so_customer_products_price_list { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_customer_promotion_config> so_customer_promotion_config { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_route_customer> so_route_customer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_sale> so_sale { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_reception_bottle> so_reception_bottle { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_tag> so_tag { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_consumer> Consumers { get; set; }

        public string getSingleEmail()
        {
            if (String.IsNullOrEmpty(email))
                return "";

            string[] split = email.Split(';');
            if (split.Length == 0)
                return "";
            else
                return split[0];
        }
    }
}
