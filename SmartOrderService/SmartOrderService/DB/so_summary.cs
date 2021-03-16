namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_summary
    {
        [Key]
        public int summaryId { get; set; }

        public int processId { get; set; }

        [Required]
        public string table_name { get; set; }

        public int to_process { get; set; }

        public int process { get; set; }

        public bool is_warning { get; set; }

        [Required]
        [StringLength(50)]
        public string branch_code { get; set; }

        public virtual so_process so_process { get; set; }
    }
}
