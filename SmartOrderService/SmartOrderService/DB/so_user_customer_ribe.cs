namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_user_customer_ribe
    {
        [Key]
        public int user_customer_ribe_Id { get; set; }

        [Required]
        [StringLength(50)]
        public string userCode { get; set; }

        [Required]
        [StringLength(50)]
        public string customerCode { get; set; }

        public int type { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }
    }
}
