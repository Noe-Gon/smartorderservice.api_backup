namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_promotion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_promotion()
        {
            so_customer_promotion_config = new HashSet<so_customer_promotion_config>();
            so_global_promotion = new HashSet<so_global_promotion>();
            so_promotion_detail_article = new HashSet<so_promotion_detail_article>();
            so_promotion_detail_product = new HashSet<so_promotion_detail_product>();
            so_promotion_discount_list = new HashSet<so_promotion_discount_list>();
            so_sale_promotion = new HashSet<so_sale_promotion>();
            so_user_promotion_config = new HashSet<so_user_promotion_config>();
        }

        [Key]
        public int promotionId { get; set; }

        [Required]
        [StringLength(50)]
        public string code { get; set; }

        [Required]
        public string name { get; set; }

        public DateTime validity_start { get; set; }

        public DateTime validity_end { get; set; }

        public int type { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_customer_promotion_config> so_customer_promotion_config { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_global_promotion> so_global_promotion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_promotion_detail_article> so_promotion_detail_article { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_promotion_detail_product> so_promotion_detail_product { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_promotion_discount_list> so_promotion_discount_list { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_sale_promotion> so_sale_promotion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_user_promotion_config> so_user_promotion_config { get; set; }
    }
}
