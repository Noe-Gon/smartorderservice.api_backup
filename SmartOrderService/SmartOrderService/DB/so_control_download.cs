namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_control_download
    {
        [Key]
        public int control_downloadId { get; set; }

        public int? processId { get; set; }

        public int userId { get; set; }

        public int modelId { get; set; }

        public int model_type { get; set; }

        public int process_type { get; set; }

        public DateTime? execute_date { get; set; }

        public bool closed { get; set; }

        public virtual so_process so_process { get; set; }

        public virtual so_user so_user { get; set; }
    }
}
