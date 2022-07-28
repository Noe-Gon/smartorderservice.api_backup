namespace SmartOrderService.DB
{
    using SmartOrderService.Models.Generic;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_route_team : AuditDate
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int routeId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int userId { get; set; }

        public byte roleTeamId { get; set; }

        public virtual so_role_team so_role_team { get; set; }

        public so_route_team() : base()
        {

        }
    }
}
