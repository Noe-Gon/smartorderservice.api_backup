namespace SmartOrderService.DB
{
    using SmartOrderService.Models.Generic;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_route_team_inventory_available : ICloneable
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int productId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int inventoryId { get; set; }

        public DateTime createOn { get; set; }

        public int Available_Amount { get; set; }

        public DateTime? modifiedon { get; set; }

        public object Clone()
        {
            var routeTeamInventory = (so_route_team_inventory_available)MemberwiseClone();
            return routeTeamInventory;
        }
    }
}
