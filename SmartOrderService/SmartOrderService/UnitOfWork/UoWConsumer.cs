using SmartOrderService.CustomExceptions;
using SmartOrderService.UnitOfWork.Repositories;
using SmartOrderService.DB;
using SmartOrderService.Models.Enum;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            DeliveryRepository = new GenericRepository<so_delivery>(Context);
            UserRepository = new GenericRepository<so_user>(Context);
            UserRouteRepository = new GenericRepository<so_user_route>(Context);
            BinnacleVisitRepository = new GenericRepository<so_binnacle_visit>(Context);
            CodePlaceRepository = new GenericRepository<so_code_place>(Context);
            SaleRepository = new GenericRepository<so_sale>(Context);
            CustomerProductPriceList = new GenericRepository<so_customer_products_price_list>(Context);
            ProductPriceList = new GenericRepository<so_products_price_list>(Context);
            PortalLinksLogRepository = new GenericRepository<so_portal_links_log>(Context);
            RouteTeamTravelsCustomerBlocked = new GenericRepository<so_route_team_travels_customer_blocked>(Context);
            WorkDayRepository = new GenericRepository<so_work_day>(Context);
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
        public GenericRepository<so_delivery> DeliveryRepository { get; set; }
        public GenericRepository<so_user> UserRepository { get; set; }
        public GenericRepository<so_user_route> UserRouteRepository { get; set; }
        public GenericRepository<so_binnacle_visit> BinnacleVisitRepository { get; set; }
        public GenericRepository<so_code_place> CodePlaceRepository { get; set; }
        public GenericRepository<so_sale> SaleRepository { get; set; }
        public GenericRepository<so_customer_products_price_list> CustomerProductPriceList { get; set; }
        public GenericRepository<so_products_price_list> ProductPriceList { get; set; }
        public GenericRepository<so_portal_links_log> PortalLinksLogRepository { get; set; }
        public GenericRepository<so_route_team_travels_customer_blocked> RouteTeamTravelsCustomerBlocked { get; set; }
        public GenericRepository<so_work_day> WorkDayRepository { get; set; }

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

        public so_work_day GetWorkdayByUserAndDate(int userId, DateTime date)
        {
            var impulsor = SearchDrivingId(userId);

            so_work_day workday = WorkDayRepository.Get(
                i => i.userId == impulsor
                && DbFunctions.TruncateTime(i.date_start) == DbFunctions.TruncateTime(date)
                ).FirstOrDefault();

            if (workday == null)
            {
                throw new WorkdayNotFoundException("No se encontro la jornada para el usuario " + impulsor + "y el dia " + date);
            }
            return workday;
        }

        public int SearchDrivingId(int actualUserId)
        {
            so_route_team teamRoute = RouteTeamRepository.Get(i => i.userId == actualUserId).FirstOrDefault();
            if (teamRoute == null)
            {
                throw new RelatedDriverNotFoundException(actualUserId);
            }
            int DrivingId = RouteTeamRepository.Get(i => i.routeId == teamRoute.routeId && i.roleTeamId == (int)ERolTeam.Impulsor).ToList().FirstOrDefault().userId;
            return DrivingId;
        }
    }
}