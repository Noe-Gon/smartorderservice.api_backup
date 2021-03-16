namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_log_errors
    {
        [Key]
        public int logId { get; set; }

        public int processId { get; set; }

        [Required]
        public string description { get; set; }

        [Required]
        [StringLength(50)]
        public string branch_code { get; set; }

        public virtual so_process so_process { get; set; }
    }
}
