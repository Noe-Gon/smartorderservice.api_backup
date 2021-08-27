using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_route_team_travels_customer_blocked
    {
        [Column("workDayId")]
        public Guid WorkDayId { get; set; }
        public so_work_day WorkDay { get; set; }

        [Column("inventoryId")]
        public int InventoryId { get; set; }
        public so_inventory Inventory { get; set; }

        [Column("userId")]
        public int UserId { get; set; }
        public so_user User { get; set; }

        [Column("customerId")]
        public int CustomerId { get; set; }
        public so_customer Customer { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }
    }
}