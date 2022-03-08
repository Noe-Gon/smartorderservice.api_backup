namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_sale_detail_article
    {
        [Key]
        public int detailId { get; set; }

        public int saleId { get; set; }

        public int article_promotionalId { get; set; }

        public int amount { get; set; }

        public decimal price { get; set; }

        public decimal import { get; set; }
    }
}
