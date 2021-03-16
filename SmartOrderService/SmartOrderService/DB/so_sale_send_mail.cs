namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_sale_send_mail
    {
        [Key]
        public int sale_send_mailId { get; set; }

        public int saleId { get; set; }

        public DateTime? date_send { get; set; }

        public bool is_send { get; set; }

        public int attempt { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }
    }
}
