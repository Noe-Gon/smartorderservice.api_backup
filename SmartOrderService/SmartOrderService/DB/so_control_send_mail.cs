namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_control_send_mail
    {
        [Key]
        public int control_send_mailId { get; set; }

        public int modelId { get; set; }

        public int modelType { get; set; }

        public bool is_send { get; set; }

        public DateTime? date_send { get; set; }

        public int attempt { get; set; }
    }
}
