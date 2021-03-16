namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_promotion_detail_article
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_promotion_detail_article()
        {
            so_equivalence_article = new HashSet<so_equivalence_article>();
        }

        public int articleId { get; set; }

        public int promotionId { get; set; }

        public int amount { get; set; }

        [Key]
        public int promotion_detail_articleId { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_equivalence_article> so_equivalence_article { get; set; }

        public virtual so_product so_product { get; set; }

        public virtual so_promotion so_promotion { get; set; }
    }
}
