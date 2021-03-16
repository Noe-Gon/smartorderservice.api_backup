namespace SmartOrderService.Utils.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class facturas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public facturas()
        {
            factdet = new HashSet<factdet>();
            facturas_so_sale = new HashSet<facturas_so_sale>();
        }

        public int factura { get; set; }

        [Required]
        [StringLength(40)]
        public string serie { get; set; }

        public int estatus { get; set; }

        public int refacturado { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long foliointerno { get; set; }

        [Required]
        [StringLength(40)]
        public string cliente { get; set; }

        public int numeroimp { get; set; }

        public int supervisor { get; set; }

        public int notaventa { get; set; }

        [Required]
        [StringLength(20)]
        public string ruta { get; set; }

        [Required]
        [StringLength(50)]
        public string facturaref { get; set; }

        [Required]
        [StringLength(20)]
        public string rfc { get; set; }

        public int clienteopecdid { get; set; }

        public int formapago { get; set; }

        public int tiponc { get; set; }

        public decimal total { get; set; }

        public decimal importeiva { get; set; }

        public decimal importeiesp { get; set; }

        public decimal importeiespcuota { get; set; }

        public decimal importeiespbotana { get; set; }

        public int tipodoc { get; set; }

        public int tipofacturacion { get; set; }

        public int cia { get; set; }

        [Required]
        [StringLength(20)]
        public string sitio { get; set; }

        [Required]
        [StringLength(250)]
        public string observ { get; set; }

        [Required]
        [StringLength(250)]
        public string razonsocial { get; set; }

        [Required]
        [StringLength(250)]
        public string direccioncliente { get; set; }

        public DateTime fechafac { get; set; }

        public DateTime fechacap { get; set; }

        public int totallineas { get; set; }

        public int totalcantidad { get; set; }

        public int csv { get; set; }

        public bool imprimeieps { get; set; }

        public bool imprimeiepscuota { get; set; }

        public bool imprimeiepsbotana { get; set; }

        public short? canceladasat { get; set; }

        [Required]
        [StringLength(3)]
        public string moneda { get; set; }

        [Column(TypeName = "numeric")]
        public decimal tipodecambio { get; set; }

        public int subpedido { get; set; }

        public DateTime? fechacan { get; set; }

        public bool imprimeiva { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<factdet> factdet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<facturas_so_sale> facturas_so_sale { get; set; }
    }
}
