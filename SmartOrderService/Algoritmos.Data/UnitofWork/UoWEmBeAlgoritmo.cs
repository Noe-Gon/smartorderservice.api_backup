using SmartOrderService.UnitOfWork.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algoritmos.Data.UnitofWork
{
    public class UoWEmBeAlgoritmo : IDisposable
    {
        public static UoWEmBeAlgoritmo Create() => new UoWEmBeAlgoritmo();

        public UoWEmBeAlgoritmo()
        {
            Context = new AlgoritmosModelContext();
            UsuariosRepository = GenericRepository<Usuarios>.Create(Context);
        }

        public AlgoritmosModelContext Context { get; set; }
        public GenericRepository<Usuarios> UsuariosRepository { get; set; }

        public void Save()
        {
            Context.SaveChanges();
        }

        private bool Disposed = false;
        protected virtual void Dispose(bool Disposing)
        {
            if (!this.Disposed)
            {
                if (Disposing)
                {
                    Context.Dispose();
                }
            }
            Disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
