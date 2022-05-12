namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_route_customer
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int customerId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int routeId { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int day { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int visit_type { get; set; }

        public int order { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        //public bool venta_alcohol { get; set; }

        public virtual so_customer so_customer { get; set; }

        public virtual so_route so_route { get; set; }
    }
}
