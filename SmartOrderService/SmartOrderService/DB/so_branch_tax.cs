namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_branch_tax
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int branchId { get; set; }

        public decimal vat { get; set; }

        public decimal stps { get; set; }

        public decimal stps_fee { get; set; }

        public decimal stps_snack { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public virtual so_branch so_branch { get; set; }
    }
}
