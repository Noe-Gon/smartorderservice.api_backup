namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_promotion_detail_product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_promotion_detail_product()
        {
            so_equivalence_product = new HashSet<so_equivalence_product>();
        }

        public int promotionId { get; set; }

        public int productId { get; set; }

        [Key]
        public int promotion_detailId { get; set; }

        public int amount { get; set; }

        public bool is_gift { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_equivalence_product> so_equivalence_product { get; set; }

        public virtual so_product so_product { get; set; }

        public virtual so_promotion so_promotion { get; set; }
    }
}
