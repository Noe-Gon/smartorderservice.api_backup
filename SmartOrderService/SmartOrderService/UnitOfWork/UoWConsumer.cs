using SmartOrderService.DB;
using SmartOrderService.UnitOfWork.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.UnitOfWork
{
    public class UoWConsumer : IDisposable
    {
        public static UoWConsumer Create() => new UoWConsumer();

        public UoWConsumer()
        {
            Context = new SmartOrderModel();
            CustomerAdditionalDataRepository = new GenericRepository<so_customer_additional_data>(Context);
            CustomerRepository = new GenericRepository<so_customer>(Context);
            CustomerRemovalRequestRepository = new GenericRepository<so_customer_removal_request>(Context);
            CustomerDataRepository =  new GenericRepository<so_customer_data>(Context);
        }

        private SmartOrderModel Context { get; set; }
        public GenericRepository<so_customer_additional_data> CustomerAdditionalDataRepository { get; set; }
        public GenericRepository<so_customer> CustomerRepository { get; set; }
        public GenericRepository<so_customer_removal_request> CustomerRemovalRequestRepository { get; set; }
        public GenericRepository<so_customer_data> CustomerDataRepository { get; set; }

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