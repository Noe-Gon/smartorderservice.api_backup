﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_customer_removal_request
    {
        public Guid Id { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("limit_days")]
        public int LimitDays { get; set; }

        [Column("reason")]
        public string Reason { get; set; }

        [Column("status")]
        public int Status { get; set; }

        #region Relations
        [Column("customer_additional_dataId")]
        public int CustomerAdditionalDataId { get; set; }
        public so_customer_additional_data CustomerAdditionalData { get; set; }

        [Column("userId")]
        public int UserId { get; set; }
        public so_user User { get; set; }
        #endregion
    }
}