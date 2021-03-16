namespace SmartOrderService.Utils.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_product()
        {
            so_sale_detail = new HashSet<so_sale_detail>();
        }

        [Key]
        public int productId { get; set; }

        public int brandId { get; set; }

        [StringLength(50)]
        public string code { get; set; }

        [Required]
        public string name { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool is_returnable { get; set; }

        public int pieces { get; set; }

        public bool status { get; set; }

        public double factor { get; set; }

        public int? type { get; set; }

        [StringLength(250)]
        public string barcode { get; set; }

        [StringLength(250)]
        public string reference { get; set; }

        public int? billing_dataId { get; set; }

        public virtual so_billing_data so_billing_data { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_sale_detail> so_sale_detail { get; set; }

        public virtual so_product_tax so_product_tax { get; set; }
    }
}
