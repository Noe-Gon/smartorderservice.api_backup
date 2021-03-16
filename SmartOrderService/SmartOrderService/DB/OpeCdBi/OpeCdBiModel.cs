using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB.OpeCdBi
{
    public class OpeCdBiModel : DbContext
    {
        public OpeCdBiModel() : base("name=OpeCDBiModel")
        {
        }

        public virtual DbSet<Biregistros> Biregistros { get; set; }

        public virtual DbSet<Birutasupreg> Birutasupreg { get; set; }

        public virtual DbSet<Birutascli> Birutascli { get; set; }

        public virtual DbSet<Birutas> Birutas { get; set; }

        public virtual DbSet<Biusocomprobantes> Biusocomprobantes { get; set; }

        public virtual DbSet<AxRefSitios> AxRefSitios { get; set; }
        public virtual DbSet<BiRutasTipos> BiRutasTipos { get; set; }
    }
}