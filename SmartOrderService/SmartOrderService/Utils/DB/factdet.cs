namespace SmartOrderService.Utils.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("factdet")]
    public partial class factdet
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long folio { get; set; }

        public int cia { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string sitio { get; set; }

        public int consec { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(40)]
        public string articulo { get; set; }

        public int productid { get; set; }

        [Required]
        [StringLength(250)]
        public string codeupc { get; set; }

        [Required]
        [StringLength(40)]
        public string codope { get; set; }

        public int piezaspemp { get; set; }

        public decimal tasaiva { get; set; }

        [Column(TypeName = "numeric")]
        public decimal ivaimporte { get; set; }

        public decimal tasaieps { get; set; }

        [Column(TypeName = "numeric")]
        public decimal iepsimporte { get; set; }

        [Column(TypeName = "numeric")]
        public decimal iepscuota { get; set; }

        public decimal iepsbotana { get; set; }

        [Column(TypeName = "numeric")]
        public decimal iepsbotanaimporte { get; set; }

        [Column(TypeName = "numeric")]
        public decimal litros { get; set; }

        [Required]
        [StringLength(50)]
        public string descripimpuestocuota { get; set; }

        public decimal cantidad { get; set; }

        [Key]
        [Column(Order = 3)]
        public decimal precio { get; set; }

        [Key]
        [Column(Order = 4)]
        public decimal descuento { get; set; }

        public DateTime fechacap { get; set; }

        [Column(TypeName = "numeric")]
        public decimal ivatotal { get; set; }

        [Required]
        [StringLength(120)]
        public string descriparticulo { get; set; }

        [Required]
        [StringLength(40)]
        public string unidad { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(30)]
        public string notaventa { get; set; }

        public virtual facturas facturas { get; set; }
    }
}
