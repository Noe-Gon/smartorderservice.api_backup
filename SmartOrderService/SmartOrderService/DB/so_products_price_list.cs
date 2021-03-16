namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_products_price_list
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_products_price_list()
        {
            so_customer_products_price_list = new HashSet<so_customer_products_price_list>();
            so_price_list_products_detail = new HashSet<so_price_list_products_detail>();
        }

        [Key]
        public int products_price_listId { get; set; }

        public int branchId { get; set; }

        [Required]
        [StringLength(50)]
        public string code { get; set; }

        [Required]
        public string name { get; set; }

        public bool is_master { get; set; }

        public DateTime validity_start { get; set; }

        public DateTime? validity_end { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public virtual so_branch so_branch { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_customer_products_price_list> so_customer_products_price_list { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_price_list_products_detail> so_price_list_products_detail { get; set; }
    }
}
