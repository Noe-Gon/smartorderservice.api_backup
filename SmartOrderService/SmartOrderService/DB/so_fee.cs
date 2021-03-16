namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_fee
    {
        [Key]
        public int feeId { get; set; }

        public int userId { get; set; }

        public double goal { get; set; }

        public double progress { get; set; }

        [Required]
        public string tendency { get; set; }

        [Required]
        public string vsGoal { get; set; }

        [Required]
        public string vsPreviousGoal { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public virtual so_user so_user { get; set; }
    }
}
