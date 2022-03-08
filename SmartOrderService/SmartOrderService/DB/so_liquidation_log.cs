using SmartOrderService.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_liquidation_log
    {
        public so_liquidation_log()
        {
            this.Status = true;
            this.CreatedBy = 2777;
            this.CreatedOn = DateTime.Now;
        }

        [Column("liquidationLogId")]
        public int Id { get; set; }

        [Column("branchId")]
        public int? BranchId { get; set; }

        [Column("routeId")]
        public int RouteId { get; set; }

        [Column("workDayId")]
        public Guid? WorkDayId { get; set; }

        [Column("executionIdAws")]
        public string ExecutionIdAws { get; set; }

        [Column("jsonInput")]
        public string JsonInput { get; set; }

        [Column("output")]
        public string OutPut { get; set; }

        [Column("createdon")]
        public DateTime CreatedOn { get; set; }

        [Column("createdby")]
        public int CreatedBy { get; set; }

        [Column("modifiedon")]
        public DateTime? ModifiedOn { get; set; }

        [Column("modifiedby")]
        public int? ModifiedBy { get; set; }

        [Column("status")]
        public bool Status { get; set; }

        #region Relations
        [Column("liquidationStatusId")]
        public int LiquidationStatusId { get; set; }
        public so_liquidation_log_status LiquidationStatus { get; set; }
        #endregion
    }
}