using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_loyalty_links_log
    {
        public Guid Id { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("limit_days")]
        public int LimitDays { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("type")]
        public int Type { get; set; }

        #region Relation
        [Column("customerId")]
        public int CustomerId { get; set; }
        public so_customer Customer { get; set; }
        #endregion
    }
}