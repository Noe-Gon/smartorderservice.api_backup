using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_route_customer_vario
    {
        [Column("routecustomervarioId")]
        public int Id { get; set; }

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

        #region Relations

        [Column("customerId")]
        public int CustomerId { get; set; }

        public so_customer Customer { get; set; }

        [Column("routeId")]
        public int RouteId { get; set; }

        public so_route Route { get; set; }
        #endregion
    }
}