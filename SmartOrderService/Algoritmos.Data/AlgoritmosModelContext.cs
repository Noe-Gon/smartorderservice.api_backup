using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Algoritmos.Data
{
    public partial class AlgoritmosModelContext : DbContext
    {
        public AlgoritmosModelContext()
            : base("name=AlgoritmosModelContext")
        {
        }

        public virtual DbSet<RutasUsuario> RutasUsuario { get; set; }
        public virtual DbSet<Cedis> Cedis { get; set; }
        public virtual DbSet<Rutas> Rutas { get; set; }
        public virtual DbSet<Usuarios> Usuarios { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RutasUsuario>()
                .Property(e => e.NombreRuta)
                .IsFixedLength();
        }
    }
}
