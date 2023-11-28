using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_sale_adjustment_reason
    {
        [Column("sale_adjustment_reasonId")]
        public int Id { get; set; }

        [Column("saleId")]
        public int SaleId { get; set; }
        public so_sale Sale { get; set; }

        [Column("reason")]
        public string Reason { get; set; }

        [Column("customerId")]
        public int CustomerId { get; set; }
        public so_customer CustomerVario { get; set; }

        [Column("createdon")]
        public DateTime? CreatedOn { get; set; }

        [Column("createdby")]
        public int? CreatedBy { get; set; }

        [Column("modifiedon")]
        public DateTime? ModifiedOn { get; set; }

        [Column("modifiedby")]
        public int? ModifiedBy { get; set; }

        [Column("status")]
        public bool Status { get; set; }
    }
}