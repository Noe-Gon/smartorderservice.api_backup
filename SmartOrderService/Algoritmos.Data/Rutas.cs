namespace Algoritmos.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("cat.Rutas")]
    public partial class Rutas
    {
        [Key]
        [Column(Order = 0)]
        public int IdRuta { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string Clave { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(255)]
        public string Descripcion { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdCedis { get; set; }

        [Key]
        [Column(Order = 4, TypeName = "datetime2")]
        public DateTime FechaAlta { get; set; }

        [Key]
        [Column(Order = 5, TypeName = "datetime2")]
        public DateTime FechaModificacion { get; set; }
    }
}
