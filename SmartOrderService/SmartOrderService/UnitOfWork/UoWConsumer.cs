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
            CustomerDataRepository = new GenericRepository<so_customer_data>(Context);
            RouteRepository = new GenericRepository<so_route>(Context);
            BranchRepository = new GenericRepository<so_branch>(Context);
            RouteCustomerRepository = new GenericRepository<so_route_customer>(Context);
            RouteTeamRepository = new GenericRepository<so_route_team>(Context);
        }

        private SmartOrderModel Context { get; set; }
        public GenericRepository<so_customer_additional_data> CustomerAdditionalDataRepository { get; set; }
        public GenericRepository<so_customer> CustomerRepository { get; set; }
        public GenericRepository<so_customer_removal_request> CustomerRemovalRequestRepository { get; set; }
        public GenericRepository<so_customer_data> CustomerDataRepository { get; set; }
        public GenericRepository<so_route> RouteRepository { get; set; }
        public GenericRepository<so_branch> BranchRepository { get; set; }
        public GenericRepository<so_route_customer> RouteCustomerRepository { get; set; }
        public GenericRepository<so_route_team> RouteTeamRepository { get; set; }

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