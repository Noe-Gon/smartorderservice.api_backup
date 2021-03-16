namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_bottle
    {
        [Key]
        public int bottleId { get; set; }

        [Required]
        [StringLength(50)]
        public string code { get; set; }

        [Required]
        public string name { get; set; }

        public DateTime createdon { get; set; }

        public int createdby { get; set; }

        public DateTime modifiedon { get; set; }

        public int modifiedby { get; set; }

        public bool status { get; set; }
    }
}
