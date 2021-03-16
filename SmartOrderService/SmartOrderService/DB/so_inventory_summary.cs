namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_inventory_summary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int inventoryId { get; set; }

        public int deliveries_amount { get; set; }

        public int products_amount { get; set; }

        public int replacements_amount { get; set; }

        public int articles_amount { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public virtual so_inventory so_inventory { get; set; }
    }
}
