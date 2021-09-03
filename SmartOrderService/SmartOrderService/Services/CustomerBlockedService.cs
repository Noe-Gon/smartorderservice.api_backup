
using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using SmartOrderService.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class CustomerBlockedService : IDisposable
    {
        public static CustomerBlockedService Create() => new CustomerBlockedService();

        private UoWConsumer UoWConsumer { get; set; }

        public CustomerBlockedService()
        {
            UoWConsumer = new UoWConsumer();
        }

        //Obtiene a los clientes bloqueados de la jornada o viaje
        public ResponseBase<List<GetCustomersBlockedResponse>> GetCustomersBlocked(GetCustomersBlockedRequest request)
        {
            try
            {
                var workDay = UoWConsumer.GetWorkdayByUserAndDate(request.UserId, DateTime.Today);
                int timeToUnblockCustomer = CustomerBlockedService.TimeToUnblockCustomer();
                var customerBlocked = UoWConsumer.RouteTeamTravelsCustomerBlocked
                    .Get(x => DbFunctions.AddMinutes(x.CreatedDate, timeToUnblockCustomer) >= DateTime.Now
                    && x.WorkDayId == workDay.work_dayId && x.UserId != request.UserId).ToList();

                if (request.InventoryId != null)
                    if (request.InventoryId != 0)
                        customerBlocked = customerBlocked
                            .Where(x => x.InventoryId == request.InventoryId)
                            .ToList();

                var response = customerBlocked
                    .Select(x => new GetCustomersBlockedResponse
                    {
                        CustomerId = x.CustomerId,
                        UserId = x.UserId
                    }).ToList();

                return ResponseBase<List<GetCustomersBlockedResponse>>.Create(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ResponseBase<BlockCustomerResponse> BlockCustomer(BlockCustomerRequest request)
        {

            var workDay = UoWConsumer.GetWorkdayByUserAndDate(request.UserId, DateTime.Today);

            var exist = UoWConsumer.RouteTeamTravelsCustomerBlocked
                .Get(x => x.CustomerId == request.CustomerId
                    && x.WorkDayId == workDay.work_dayId && x.InventoryId == request.InventoryId)
                .FirstOrDefault();

            //El cliente no esta bloqueado
            if (exist == null)
            {
                //Se bloquea al cliente
                UoWConsumer.RouteTeamTravelsCustomerBlocked
                    .Insert(new so_route_team_travels_customer_blocked
                    {
                        CustomerId = request.CustomerId,
                        InventoryId = request.InventoryId,
                        WorkDayId = workDay.work_dayId,
                        UserId = request.UserId,
                        CreatedDate = DateTime.Now
                    });

                UoWConsumer.Save();

                return ResponseBase<BlockCustomerResponse>.Create(new BlockCustomerResponse
                {
                    Msg = "Se ha bloqueado al cliente"
                });
            }
            //Si esta bloqueado por alguien mas
            if (exist.UserId != request.UserId)
            {
                //Verificar si esta activo el bloqueo
                if (exist.CreatedDate.AddMinutes(CustomerBlockedService.TimeToUnblockCustomer()) >= DateTime.Now)
                    return ResponseBase<BlockCustomerResponse>.Create(new List<string>()
                    {
                        "El cliente se encuentra bloqueado"
                    });

                //Eliminar registro
                UoWConsumer.RouteTeamTravelsCustomerBlocked.Delete(exist);
                //Se crea el nuevo
                //Se bloquea al cliente
                UoWConsumer.RouteTeamTravelsCustomerBlocked
                    .Insert(new so_route_team_travels_customer_blocked
                    {
                        CustomerId = request.CustomerId,
                        InventoryId = request.InventoryId,
                        WorkDayId = workDay.work_dayId,
                        UserId = request.UserId,
                        CreatedDate = DateTime.Now
                    });

                UoWConsumer.Save();
                return ResponseBase<BlockCustomerResponse>.Create(new BlockCustomerResponse
                {
                    Msg = "Se ha bloqueado al cliente"
                });
            }


            //Si la mima persona lo bloqueo
            if (exist.UserId == request.UserId)
            {
                //Verificar si esta activo el bloqueo
                if (exist.CreatedDate.AddMinutes(CustomerBlockedService.TimeToUnblockCustomer()) >= DateTime.Now)
                    return ResponseBase<BlockCustomerResponse>.Create(new BlockCustomerResponse
                    {
                        Msg = "El cliente se encuentra bloqueado"
                    });

                //Si no esta activo actualizar el registro
                exist.CreatedDate = DateTime.Now;
                UoWConsumer.RouteTeamTravelsCustomerBlocked.Update(exist);
                UoWConsumer.Save();
            }

            return ResponseBase<BlockCustomerResponse>.Create(new BlockCustomerResponse
            {
                Msg = "Se ha bloqueado al cliente"
            });
        }

        public ResponseBase<UnblockCustomerResponse> UnblockCustomer(UnblockCustomerRequest request)
        {
            try
            {
                var workday = UoWConsumer.GetWorkdayByUserAndDate(request.UserId, DateTime.Today);

                var deleteCustomerBlocked = UoWConsumer.RouteTeamTravelsCustomerBlocked
                    .Get(x => x.CustomerId == request.CustomerId && x.InventoryId == request.InventoryId
                        && x.WorkDayId == workday.work_dayId && x.UserId == request.UserId)
                    .FirstOrDefault();

                if (deleteCustomerBlocked == null)
                    return ResponseBase<UnblockCustomerResponse>.Create(new UnblockCustomerResponse
                    {
                        Msg = "El cliente esta desbloqueado"
                    });

                UoWConsumer.RouteTeamTravelsCustomerBlocked.Delete(deleteCustomerBlocked);
                UoWConsumer.Save();

                return ResponseBase<UnblockCustomerResponse>.Create(new UnblockCustomerResponse
                {
                    Msg = "El cliente se ha desbloqueado"
                });
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public ResponseBase<ClearBlockedCustomerResponse> ClearBlockedCustomer(ClearBlockedCustomerRequest request)
        {
            try
            {
                var workDay = UoWConsumer.GetWorkdayByUserAndDate(request.UserId, DateTime.Today);

                var deleteCustomerBlocked = UoWConsumer.RouteTeamTravelsCustomerBlocked
                    .Get(x => x.WorkDayId == workDay.work_dayId)
                    .ToList();

                UoWConsumer.RouteTeamTravelsCustomerBlocked.DeleteByRange(deleteCustomerBlocked);
                UoWConsumer.Save();

                return ResponseBase<ClearBlockedCustomerResponse>.Create(new ClearBlockedCustomerResponse
                {
                    Msg = "Se ha eliminado con exito"
                });
            }
            catch (WorkdayNotFoundException e)
            {
                return ResponseBase<ClearBlockedCustomerResponse>.Create(new List<string>()
                {
                    e.Message
                });
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static int TimeToUnblockCustomer()
        {
            var TimeToUnblockCustomer = ConfigurationManager.AppSettings["TimeToUnblockCustomer"];

            if (string.IsNullOrEmpty(TimeToUnblockCustomer))
                return 10;
            try
            {
                return Convert.ToInt32(TimeToUnblockCustomer);
            }
            catch (Exception)
            {
                return 10;
            }
        }

        public void Dispose()
        {
            this.UoWConsumer.Dispose();
            this.UoWConsumer = null;
        }
    }
}