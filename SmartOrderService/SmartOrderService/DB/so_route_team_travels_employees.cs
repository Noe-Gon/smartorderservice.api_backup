using SmartOrderService.Models.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace SmartOrderService.DB
{
    public class so_route_team_travels_employees : AuditDate
    {
        [Key]
        [Column(Order = 0)]
        public int routeId { get; set; }

        [Key]
        [Column(Order = 1)]
        public int inventoryId { get; set; }

        [Key]
        [Column(Order = 2)]
        public Guid work_dayId { get; set; }

        [Key]
        [Column(Order = 3)]
        public int userId { get; set; }

        public int travelNumber { get; set; }

        public int? employeeCode { get; set; }

        public bool active { get; set; }

        public so_route_team_travels_employees() : base()
        {
            
        }
    }
}