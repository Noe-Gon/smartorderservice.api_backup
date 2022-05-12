namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_article
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_article()
        {
            so_branch_articles = new HashSet<so_branch_articles>();
            so_sale_promotion_detail_article = new HashSet<so_sale_promotion_detail_article>();
        }

        [Key]
        public int articleId { get; set; }

        [Required]
        public string name { get; set; }

        public double price { get; set; }

        [Required]
        [StringLength(50)]
        public string code { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_branch_articles> so_branch_articles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_sale_promotion_detail_article> so_sale_promotion_detail_article { get; set; }
    }
}
