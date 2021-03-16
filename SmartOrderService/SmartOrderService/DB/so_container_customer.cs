namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_container_customer
    {
        [Key]
        public int container_customerId { get; set; }

        public int containerId { get; set; }

        public int customerId { get; set; }

        [Required]
        public string name { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public virtual so_container so_container { get; set; }

        public virtual so_customer so_customer { get; set; }
    }
}
