using SmartOrderService.UnitOfWork.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM.Data.UnitOfWork
{
    public class UoWCRM : IDisposable
    {
        public static UoWCRM Create() => new UoWCRM();

        public UoWCRM()
        {
            Context = new CRMModelContext();
            RutasRepository = new GenericRepository<Ope_rutas>(Context);
            PaisesRepository = new GenericRepository<Ope_pais>(Context);
            MunicipiosRepository = new GenericRepository<Ope_municipio>(Context);
            EstadosRepository = new GenericRepository<Ope_estado>(Context);
            ColoniasRepository = new GenericRepository<Ope_colonia>(Context);
        }

        private CRMModelContext Context { get; set; }

        public GenericRepository<Ope_rutas> RutasRepository { get; set; }
        public GenericRepository<Ope_pais> PaisesRepository { get; set; }
        public GenericRepository<Ope_municipio> MunicipiosRepository { get; set; }
        public GenericRepository<Ope_estado> EstadosRepository { get; set; }
        public GenericRepository<Ope_colonia> ColoniasRepository { get; set; }

        public void Save()
        {
            Context.SaveChanges();
        }

        private bool Disposed = false;

        protected virtual void Dispose(bool Disposing)
        {
            if (this.Disposed)
                if (Disposing)
                    Context.Dispose();

            Disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
