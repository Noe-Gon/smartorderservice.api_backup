namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_delivery_references
    {
        [Key]
        public int delivery_references_Id { get; set; }

        [Required]
        [StringLength(100)]
        public string description { get; set; }

        public int value { get; set; }

        [Column(TypeName = "date")]
        public DateTime createdon { get; set; }

        public int createdby { get; set; }

        public bool status { get; set; }
    }
}
