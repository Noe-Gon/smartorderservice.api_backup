namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_work_day
    {
        [Key]
        public Guid work_dayId { get; set; }

        public int? userId { get; set; }

        public int? deviceId { get; set; }

        public DateTime? date_start { get; set; }

        public DateTime? date_end { get; set; }

        public DateTime? createdon { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? openby_portal { get; set; }

        public int? closedby_portal { get; set; }

        public int? openby_device { get; set; }

        public int? closedby_device { get; set; }

        public bool status { get; set; }

        public virtual so_device so_device { get; set; }

        public virtual so_user so_user { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_route_team_travels_customer_blocked> RouteTeamTravelsCustomerBlockeds { get; set; }
    }
}
