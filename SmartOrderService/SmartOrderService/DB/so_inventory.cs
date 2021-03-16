namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_inventory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_inventory()
        {
            so_delivery = new HashSet<so_delivery>();
            so_inventory_detail_article = new HashSet<so_inventory_detail_article>();
            so_inventory_detail = new HashSet<so_inventory_detail>();
            so_inventory_replacement_detail = new HashSet<so_inventory_replacement_detail>();
            so_sale_inventory = new HashSet<so_sale_inventory>();
            so_user_devolutions = new HashSet<so_user_devolutions>();
        }

        [Key]
        public int inventoryId { get; set; }

        public int userId { get; set; }

        public int order { get; set; }

        [Required]
        [StringLength(50)]
        public string code { get; set; }

        [Column(TypeName = "date")]
        public DateTime date { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public int state { get; set; }

        public bool status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_delivery> so_delivery { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_inventory_detail_article> so_inventory_detail_article { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_inventory_detail> so_inventory_detail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_inventory_replacement_detail> so_inventory_replacement_detail { get; set; }

        public virtual so_user so_user { get; set; }

        public virtual so_inventory_summary so_inventory_summary { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_sale_inventory> so_sale_inventory { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_user_devolutions> so_user_devolutions { get; set; }
    }
}
