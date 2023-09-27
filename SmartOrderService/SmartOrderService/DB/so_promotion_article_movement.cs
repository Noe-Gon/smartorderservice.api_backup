namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_promotion_article_movement
    {
        public int id { get; set; }

        public int article_promotional_routeId { get; set; }

        public int type { get; set; }

        public int amount { get; set; }

        public int stock { get; set; }

        [Required]
        [StringLength(255)]
        public string comment { get; set; }

        public int createdby { get; set; }

        public DateTime createdon { get; set; }

        [Column(TypeName = "date")]
        public DateTime? date { get; set; }

        public virtual so_article_promotional_route so_article_promotional_route { get; set; }
    }
}
