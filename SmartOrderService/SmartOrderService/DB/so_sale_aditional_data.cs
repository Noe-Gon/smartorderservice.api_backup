namespace SmartOrderService.DB
{
    using SmartOrderService.Models.Generic;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_sale_aditional_data : AuditDate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_sale_aditional_data() : base()
        {
        }

        [Key]
        public int saleAdicionalDataId { get; set; }
        public int saleId { get; set; }
        public string paymentMethod { get; set; }

    }
}
