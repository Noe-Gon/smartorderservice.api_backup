using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace SmartOrderService.DB
{
    public partial class so_route_team_travels2
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_route_team_travels2()
        {
            so_route_team_travels_employees = new HashSet<so_route_team_travels_employees>();
        }

        [Key]
        public int routeTeamTravelId { get; set; }

        public int inventoryId { get; set; }

        public Guid work_dayId { get; set; }

        public int routeId { get; set; }

        public int travelNumber { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_route_team_travels_employees> so_route_team_travels_employees { get; set; }
    }
}