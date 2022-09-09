using SmartOrderService.CustomExceptions;
using SmartOrderService.UnitOfWork.Repositories;
using SmartOrderService.DB;
using SmartOrderService.Models.Enum;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Configuration;

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
            CustomerProductPriceListRepository = new GenericRepository<so_customer_products_price_list>(Context);
            ProductPriceListRepository = new GenericRepository<so_products_price_list>(Context);
            PortalLinksLogRepository = new GenericRepository<so_portal_links_log>(Context);
            LoyaltyLinksLogRepository = new GenericRepository<so_loyalty_links_log>(Context);
            RouteTeamTravelsCustomerBlocked = new GenericRepository<so_route_team_travels_customer_blocked>(Context);
            WorkDayRepository = new GenericRepository<so_work_day>(Context);
            RouteCustomerVarioRepository = new GenericRepository<so_route_customer_vario>(Context);
            ProductBottleRepository = new GenericRepository<so_product_bottle>(Context);
            SaleDetailRepository = new GenericRepository<so_sale_detail>(Context);
            LiquidationLogRepository = new GenericRepository<so_liquidation_log>(Context);
            LiquidationLogStatusRepository = new GenericRepository<so_liquidation_log_status>(Context);
            LeaderAuthorizationCodeRepository = new GenericRepository<so_leader_authorization_code>(Context);
            AuthentificationLogRepository = new GenericRepository<so_authentication_log>(Context);
            RouteTeamInventoryAvailableRepository = new GenericRepository<so_route_team_inventory_available>(Context);
            BranchTaxRepository = new GenericRepository<so_branch_tax>(Context);
            ProductRepository = new GenericRepository<so_product>(Context);
            DeliveryDetailRepository = new GenericRepository<so_delivery_detail>(Context);
            DeliveryStatusRepository = new GenericRepository<so_delivery_status>(Context);
            DeliveryAdditionalData = new GenericRepository<so_delivery_additional_data>(Context);
            SaleAdditionalDataRepository = new GenericRepository<so_sale_aditional_data>(Context);
            ProductTaxRepository = new GenericRepository<so_product_tax>(Context);
            InventoryRepository = new GenericRepository<so_inventory>(Context);
            SynchronizedConsumersRepository = new GenericRepository<so_synchronized_consumer>(Context);
            SynchronizedConsumerDetailsRepository = new GenericRepository<so_synchronized_consumer_detail>(Context);
            RouteTeamInventoryAvailableRepository = new GenericRepository<so_route_team_inventory_available>(Context);
            RouteTeamTravlesEmployeesRepository = new GenericRepository<so_route_team_travels_employees>(Context);
        }

        public SmartOrderModel Context { get; set; }
        public GenericRepository<so_customer_additional_data> CustomerAdditionalDataRepository { get; set; }
        public GenericRepository<so_customer> CustomerRepository { get; set; }
        public GenericRepository<so_customer_removal_request> CustomerRemovalRequestRepository { get; set; }
        public GenericRepository<so_customer_data> CustomerDataRepository { get; set; }
        public GenericRepository<so_route> RouteRepository { get; set; }
        public GenericRepository<so_branch> BranchRepository { get; set; }
        public GenericRepository<so_route_customer> RouteCustomerRepository { get; set; }
        public GenericRepository<so_route_team> RouteTeamRepository { get; set; }
        public GenericRepository<so_inventory> InventoryRepository { get; set; }
        public GenericRepository<so_delivery> DeliveryRepository { get; set; }
        public GenericRepository<so_user> UserRepository { get; set; }
        public GenericRepository<so_user_route> UserRouteRepository { get; set; }
        public GenericRepository<so_binnacle_visit> BinnacleVisitRepository { get; set; }
        public GenericRepository<so_code_place> CodePlaceRepository { get; set; }
        public GenericRepository<so_sale> SaleRepository { get; set; }
        public GenericRepository<so_customer_products_price_list> CustomerProductPriceListRepository { get; set; }
        public GenericRepository<so_products_price_list> ProductPriceListRepository { get; set; }
        public GenericRepository<so_portal_links_log> PortalLinksLogRepository { get; set; }
        public GenericRepository<so_loyalty_links_log> LoyaltyLinksLogRepository { get; set; }
        public GenericRepository<so_route_team_travels_customer_blocked> RouteTeamTravelsCustomerBlocked { get; set; }
        public GenericRepository<so_work_day> WorkDayRepository { get; set; }
        public GenericRepository<so_leader_authorization_code> LeaderAuthorizationCodeRepository { get; set; }
        public GenericRepository<so_authentication_log> AuthentificationLogRepository { get; set; }
        public GenericRepository<so_route_customer_vario> RouteCustomerVarioRepository { get; set; }
        public GenericRepository<so_sale_promotion> SalePromotionRepository { get; set; }
        public GenericRepository<so_product_bottle> ProductBottleRepository { get; set; }
        public GenericRepository<so_product> ProductRepository { get; set; }
        public GenericRepository<so_route_team_inventory_available> RouteTeamInventoryAvailableRepository { get; set; }
        public GenericRepository<so_product_tax> ProductTaxRepository { get; set; }
        public GenericRepository<so_branch_tax> BranchTaxRepository { get; set; }
        public GenericRepository<so_delivery_detail> DeliveryDetailRepository { get; set; }
        public GenericRepository<so_delivery_status> DeliveryStatusRepository { get; set; }
        public GenericRepository<so_delivery_additional_data> DeliveryAdditionalData { get; set; }
        public GenericRepository<so_sale_aditional_data> SaleAdditionalDataRepository { get; set; }
        public GenericRepository<so_sale_detail> SaleDetailRepository { get; set; }
        public GenericRepository<so_liquidation_log> LiquidationLogRepository { get; set; }
        public GenericRepository<so_liquidation_log_status> LiquidationLogStatusRepository { get; set; }
        public GenericRepository<so_route_team_travels_employees> RouteTeamTravlesEmployeesRepository { get; set; }
        public GenericRepository<so_synchronized_consumer> SynchronizedConsumersRepository { get; set; }
        public GenericRepository<so_synchronized_consumer_detail> SynchronizedConsumerDetailsRepository { get; set; }

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

        public bool CheckInventoryAvailability(int inventoryId, int productId, int amount)
        {
            var inventoryTeam = RouteTeamInventoryAvailableRepository.Get(s => s.inventoryId.Equals(inventoryId)).ToList();
            var inventoryProduct = inventoryTeam.Where(s => s.productId.Equals(productId)).FirstOrDefault();
            if (inventoryProduct == null)
            {
                throw new ProductNotFoundBillingException("No se encontro el producto con el id " + productId);
            }
            if (amount <= inventoryProduct.Available_Amount)
            {
                return true;
            }
            return false;
        }

        public string GetCancelLinkByCustomerId(int customerId)
        {
            var portalLinkLogs = PortalLinksLogRepository
                .Get(x => x.CustomerId == customerId && x.Status == (int)PortalLinks.STATUS.PENDING && x.Type == (int)PortalLinks.TYPE.EMAIL_DEACTIVATION)
                .FirstOrDefault();

            if (portalLinkLogs == null)
            {
                //Generar el link para cancelar el envio de correo
                Guid id = Guid.NewGuid();
                var cancelEmail = new so_portal_links_log
                {
                    CustomerId = customerId,
                    CreatedDate = DateTime.Today,
                    Id = id,
                    LimitDays = 0,
                    Status = (int)PortalLinks.STATUS.PENDING,
                    Type = (int)PortalLinks.TYPE.EMAIL_DEACTIVATION
                };

                PortalLinksLogRepository.Insert(cancelEmail);

                return ConfigurationManager.AppSettings["PortalUrl"] + "Consumer/CancelTicketDigital/" + id;
            }

            return ConfigurationManager.AppSettings["PortalUrl"] + "Consumer/CancelTicketDigital/" + portalLinkLogs.Id;
        }

        public ERolTeam GetUserRole(int userId)
        {
            so_route_team userRoleTeam = RouteTeamRepository.Get(i => i.userId == userId).FirstOrDefault();
            if (userRoleTeam == null)
            {
                return ERolTeam.SinAsignar;
            }
            return (ERolTeam)userRoleTeam.roleTeamId;
        }


    }
}