namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_article_promotional
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_article_promotional()
        {
            so_promotion_article = new HashSet<so_promotion_article>();
        }

        public int id { get; set; }

        [Required]
        [StringLength(255)]
        public string name { get; set; }

        public decimal? price { get; set; }

        public int createdby { get; set; }

        public int? modifiedby { get; set; }

        public DateTime createdon { get; set; }

        public DateTime? modifiedon { get; set; }

        public bool status { get; set; }

        [Required]
        [StringLength(255)]
        public string code { get; set; }

        public int amount { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_promotion_article> so_promotion_article { get; set; }
    }
}
