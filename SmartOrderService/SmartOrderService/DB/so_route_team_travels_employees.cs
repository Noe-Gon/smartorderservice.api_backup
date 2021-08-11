using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace SmartOrderService.DB
{
    public partial class so_route_team_travels_employees
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int routeTeamTravelId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int userId { get; set; }

        public int? employeeCode { get; set; }

        public bool active { get; set; }

        public virtual so_route_team_travels2 so_route_team_travels2 { get; set; }
    }
}