namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_configuration_customer
    {
        [Key]
        public int configuration_customerId { get; set; }

        [StringLength(250)]
        public string payment_method { get; set; }

        [StringLength(250)]
        public string payment_account { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }
    }
}
