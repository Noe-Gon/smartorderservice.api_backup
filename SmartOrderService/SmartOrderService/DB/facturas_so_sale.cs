namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class facturas_so_sale
    {
        public int cia { get; set; }

        [Required]
        [StringLength(120)]
        public string serie { get; set; }

        public int factura { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long foliointerno { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int saleId { get; set; }

        public DateTime fechacap { get; set; }

        public virtual factura factura1 { get; set; }

        public virtual so_sale so_sale { get; set; }
    }
}
