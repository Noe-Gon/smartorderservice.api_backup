namespace SmartOrderService.Utils.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_billing_data
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_billing_data()
        {
            so_product = new HashSet<so_product>();
        }

        [Key]
        public int billing_dataId { get; set; }

        [Required]
        [StringLength(50)]
        public string code { get; set; }

        [Required]
        [StringLength(250)]
        public string name { get; set; }

        [StringLength(250)]
        public string address_street { get; set; }

        [StringLength(250)]
        public string address_number { get; set; }

        [StringLength(250)]
        public string postal_code { get; set; }

        [StringLength(250)]
        public string suburb { get; set; }

        [StringLength(250)]
        public string town { get; set; }

        [StringLength(250)]
        public string state { get; set; }

        [StringLength(250)]
        public string country { get; set; }

        [StringLength(250)]
        public string ftr { get; set; }

        [StringLength(100)]
        public string phone { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_product> so_product { get; set; }
    }
}
