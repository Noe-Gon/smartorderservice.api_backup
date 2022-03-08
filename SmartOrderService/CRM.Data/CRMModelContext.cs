using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace CRM.Data
{
    public partial class CRMModelContext : DbContext
    {
        public CRMModelContext()
            : base("name=CRMModelContext")
        {
        }

        public virtual DbSet<Ope_colonia> Ope_colonia { get; set; }
        public virtual DbSet<Ope_estado> Ope_estado { get; set; }
        public virtual DbSet<Ope_municipio> Ope_municipio { get; set; }
        public virtual DbSet<Ope_pais> Ope_pais { get; set; }
        public virtual DbSet<Ope_rutas> Ope_rutas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ope_colonia>()
                .Property(e => e.VersionNumber)
                .IsFixedLength();

            modelBuilder.Entity<Ope_estado>()
                .Property(e => e.VersionNumber)
                .IsFixedLength();

            modelBuilder.Entity<Ope_municipio>()
                .Property(e => e.VersionNumber)
                .IsFixedLength();

            modelBuilder.Entity<Ope_pais>()
                .Property(e => e.VersionNumber)
                .IsFixedLength();

            modelBuilder.Entity<Ope_rutas>()
                .Property(e => e.VersionNumber)
                .IsFixedLength();
        }
    }
}
