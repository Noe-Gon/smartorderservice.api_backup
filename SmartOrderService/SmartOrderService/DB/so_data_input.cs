namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_data_input
    {
        [Key]
        public int data_inputId { get; set; }

        public int userId { get; set; }

        public string data { get; set; }

        public DateTime? receive_date { get; set; }

        [Required]
        [StringLength(100)]
        public string checksum { get; set; }

        public int process_status { get; set; }

        public string result { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public virtual so_user so_user { get; set; }
    }
}
