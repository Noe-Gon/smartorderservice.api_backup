namespace SmartOrderService.DB
{
    using SmartOrderService.Models.Generic;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_route_team_travels : AuditDate
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int routeId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int inventoryId { get; set; }

        public int travelStatus { get; set; }

        [Key]
        [Column(Order = 2)]
        public Guid work_dayId { get; set; }

        public so_route_team_travels() : base()
        {

        }

    }
}
