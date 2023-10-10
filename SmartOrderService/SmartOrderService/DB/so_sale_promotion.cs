namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_sale_promotion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_sale_promotion()
        {
            so_sale_promotion_detail_article = new HashSet<so_sale_promotion_detail_article>();
            so_sale_promotion_detail_product = new HashSet<so_sale_promotion_detail_product>();
            so_sale_promotion_detail = new HashSet<so_sale_promotion_detail>();
        }

        [Key]
        public int sale_promotionId { get; set; }

        public int saleId { get; set; }

        public int promotionId { get; set; }

        public int amount { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public decimal? additional_cost { get; set; }

        public int? promotion_catalogId { get; set; }

        public virtual so_promotion so_promotion { get; set; }

        public virtual so_sale so_sale { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_sale_promotion_detail_article> so_sale_promotion_detail_article { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_sale_promotion_detail> so_sale_promotion_detail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_sale_promotion_detail_product> so_sale_promotion_detail_product { get; set; }
    }
}
