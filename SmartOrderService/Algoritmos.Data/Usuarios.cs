namespace Algoritmos.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("cat.Usuarios")]
    public partial class Usuarios
    {
        [Key]
        [Column(Order = 0)]
        public int IdUsuario { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string Uuid { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(255)]
        public string Nombres { get; set; }

        [Key]
        [Column(Order = 3)]
        public bool Estatus { get; set; }

        [Key]
        [Column(Order = 4, TypeName = "datetime2")]
        public DateTime FechaAlta { get; set; }

        [Key]
        [Column(Order = 5, TypeName = "datetime2")]
        public DateTime FechaModificacion { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? FechaBaja { get; set; }

        public int? IdRole { get; set; }

        public int? IdCedis { get; set; }

        [StringLength(50)]
        public string Usuario { get; set; }

        [StringLength(150)]
        public string Token { get; set; }

        [StringLength(50)]
        public string Vencimiento { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(100)]
        public string Cedis { get; set; }

        [Key]
        [Column(Order = 7)]
        public string Contrasena { get; set; }

        [StringLength(255)]
        public string Email { get; set; }
    }
}
