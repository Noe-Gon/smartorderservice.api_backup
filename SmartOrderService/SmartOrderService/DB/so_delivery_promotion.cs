namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_delivery_promotion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_delivery_promotion()
        {
            so_delivery_promotion_article_detail = new HashSet<so_delivery_promotion_article_detail>();
            so_delivery_promotion_detail = new HashSet<so_delivery_promotion_detail>();
        }

        [Key]
        public int delivery_promotionId { get; set; }

        public int deliveryId { get; set; }

        public int promotionId { get; set; }

        public int amount { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public virtual so_delivery so_delivery { get; set; }

        public virtual so_promotion so_promotion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_delivery_promotion_article_detail> so_delivery_promotion_article_detail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_delivery_promotion_detail> so_delivery_promotion_detail { get; set; }
    }
}
