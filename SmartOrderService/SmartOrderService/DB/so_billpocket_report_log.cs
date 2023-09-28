using SmartOrderService.Models.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_billpocket_report_log : AuditData
    {
        [Column("billpocket_report_logId")]
        public int Id { get; set; }
        [Column("userId")]
        public int? UserId { get; set; }
        public virtual so_user User { get; set; }

        [Column("routeId")]
        public int RouteId { get; set; }
        public virtual so_route Route { get; set; }

        [Column("work_dayId")]
        public Guid WorkDayId { get; set; }
        public virtual so_work_day WorkDay { get; set; }

        [Column("send_date")]
        public DateTime SendDate { get; set; }

        [Column("total_sales")]
        public int TotalSales { get; set; }

        [Column("total_amount")]
        public double TotalAmount { get; set; }
    }
}