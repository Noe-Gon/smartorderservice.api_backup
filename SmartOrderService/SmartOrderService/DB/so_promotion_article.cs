namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_promotion_article
    {
        public int id { get; set; }

        public int article_promotionalId { get; set; }

        public int amount { get; set; }

        public decimal? price { get; set; }

        public int promotion_catalogId { get; set; }

        public int createdby { get; set; }

        public int? modifiedby { get; set; }

        public DateTime createdon { get; set; }

        public DateTime? modifiedon { get; set; }

        public bool status { get; set; }

        public virtual so_article_promotional so_article_promotional { get; set; }
    }
}
