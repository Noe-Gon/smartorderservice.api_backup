using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_portal_links_log
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
        [Column("customer_additional_dataId")]
        public int CustomerAdditionalDataId { get; set; }
        public so_customer_additional_data CustomerAdditionalData { get; set; }
        #endregion
    }
}