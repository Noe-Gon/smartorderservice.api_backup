namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_replacement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_replacement()
        {
            so_delivery_replacement = new HashSet<so_delivery_replacement>();
            so_inventory_replacement_detail = new HashSet<so_inventory_replacement_detail>();
            so_sale_replacement = new HashSet<so_sale_replacement>();
        }

        [Key]
        public int replacementId { get; set; }

        [Required]
        [StringLength(50)]
        public string code { get; set; }

        [Required]
        public string name { get; set; }

        public DateTime createdon { get; set; }

        public int createdby { get; set; }

        public DateTime modifiedon { get; set; }

        public int modifiedby { get; set; }

        public bool status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_delivery_replacement> so_delivery_replacement { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_inventory_replacement_detail> so_inventory_replacement_detail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_sale_replacement> so_sale_replacement { get; set; }
    }
}
