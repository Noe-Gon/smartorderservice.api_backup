namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_data_out
    {
        [Key]
        public int data_outId { get; set; }

        public int userId { get; set; }

        public string complete_data { get; set; }

        public string partial_data { get; set; }

        public int process_status { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public virtual so_user so_user { get; set; }
    }
}
