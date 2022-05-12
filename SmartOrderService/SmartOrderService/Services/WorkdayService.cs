using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartOrderService.DB;
using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.DTO;
using System.Data.Entity;
using OpeCDLib.Models;
using SmartOrderService.Models.Enum;
using RestSharp;
using System.Configuration;

namespace SmartOrderService.Services
{
    public class WorkdayService
    {
        private SmartOrderModel db = new SmartOrderModel();
        private RoleTeamService roleTeamService = new RoleTeamService();
        private RouteTeamInventoryAvailableService routeTeamInventoryAvailable = new RouteTeamInventoryAvailableService();
        private InventoryService inventoryService = new InventoryService();
        private SmartOrderModel inventoryContext = new SmartOrderModel();

        public Workday createWorkday(int userId)
        {

            var time = DateTime.Now;
            ERolTeam userRol = roleTeamService.GetUserRole(userId);

            if (userRol != ERolTeam.SinAsignar)
            {
                //Start Load Inventory Process OPCD
                int impulsorId = inventoryService.SearchDrivingId(userId);
                var routeTeam = inventoryContext.so_route_team.Where(x => x.userId == userId).First();
                var route = inventoryContext.so_route.Where(x => x.routeId == routeTeam.routeId).First();

                inventoryService.CallLoadInventoryProcess(impulsorId, route.so_branch.code, route.code, null);
                //End Load Inventory Process
            }
            //Validamos que exista un inventari actual
            try
            {
                if(userRol == ERolTeam.SinAsignar)
                {
                    inventoryService.GetCurrentInventory(userId, time.Date);
                }
            }
            catch(InventoryEmptyException e)
            {
                throw new CreateWorkdayInventoryNotFoundException();
            }
            Workday workday = new Workday();

            var currentWorkday = searchWorkDay(userId);

            if (currentWorkday != null)
            {
                workday.UserId = userId;
                workday.WorkdayId = currentWorkday.work_dayId;
                workday.IsOpen = true;
                return workday;
            }

            if (userRol != ERolTeam.SinAsignar)
            {
                var id = Guid.NewGuid();
                int driverId = inventoryService.SearchDrivingId(userId);
                var currentWorkdayTeam = searchWorkDay(driverId);

                if (currentWorkdayTeam != null)
                {
                    workday.UserId = userId;
                    workday.WorkdayId = currentWorkdayTeam.work_dayId;
                    workday.IsOpen = true;
                    return workday;
                }

                var device = db.so_device.Where(d => d.userId == userId && d.status);

                //validamos que este registrado
                if (!device.Any())
                    throw new NoUserFoundException();

                int deviceId = device.FirstOrDefault().deviceId;

                var newWorkday = new so_work_day()
                {
                    work_dayId = id,
                    userId = driverId,
                    date_start = time,
                    createdon = time,
                    deviceId = deviceId,
                    openby_device = userId,
                    modifiedon = time,
                    status = true
                };

                db.so_work_day.Add(newWorkday);
                db.SaveChanges();

                workday.UserId = userId;
                workday.WorkdayId = newWorkday.work_dayId;
                workday.IsOpen = true;

                return workday;
            }
            else
            {
                var id = Guid.NewGuid();

                var device = db.so_device.Where(d => d.userId == userId && d.status);

                //validamos que este registrado
                if (!device.Any())
                    throw new NoUserFoundException();

                int deviceId = device.FirstOrDefault().deviceId;

                var newWorkday = new so_work_day()
                {
                    work_dayId = id,
                    userId = userId,
                    date_start = time,
                    createdon = time,
                    deviceId = deviceId,
                    openby_device = userId,
                    modifiedon = time,
                    status = true
                };

                db.so_work_day.Add(newWorkday);
                db.SaveChanges();

                workday.UserId = userId;
                workday.WorkdayId = newWorkday.work_dayId;
                workday.IsOpen = true;

                return workday;
            }
        }

        public List<Jornada> RetrieveWorkDay(string BranchCode,string UserCode,DateTime Date)
        {
            var journeys = db.so_work_day.Where(w => 
                w.so_user.so_branch.code.Equals(BranchCode)
                && DbFunctions.TruncateTime(w.date_start.Value) == DbFunctions.TruncateTime(Date)
                && w.status
            ).ToList();

            var inventoryService = new InventoryService();

            var trips = inventoryService.getTrips(BranchCode,Date);

            if (UserCode != null)
                journeys = journeys.Where(j=> j.so_user.code.Equals(UserCode)).ToList();

            List<Jornada> Jornadas = new List<Jornada>();

            foreach (var trip in trips) {

                var currentJourney = Jornadas.Where(j => j.Ruta.Equals(trip.RouteCode)).FirstOrDefault();

                var viaje = new Viaje() { Numero = trip.Order, Finalizado = trip.IsFinished };

                if (currentJourney != null)
                {
                    currentJourney.Viajes.Add(viaje);
                }

                else {
                    var journey = journeys.Where(j=> j.userId == trip.UserId).FirstOrDefault();
                    var jornada = new Jornada();
                    jornada.Ruta = trip.RouteCode;

                    jornada.Finalizada = false;
                    jornada.Inicio = DateTime.Today;
                    jornada.Fin = null;

                    if (journey != null) {
                        jornada.Finalizada = journey.date_end.HasValue;
                        jornada.Inicio = journey.date_start.Value;
                        jornada.Fin = journey.date_end.HasValue ? journey.date_end : null;
                    }

                    jornada.Viajes.Add(viaje);
                    Jornadas.Add(jornada);
                }
            }

            return Jornadas;
        }

        public bool onOpenWorkDay(Workday workday)
        {
            return false;
        }

        public Workday FinishWorkday(Workday workday)
        {
            ERolTeam userRol = roleTeamService.GetUserRole(workday.UserId);
            if (userRol == ERolTeam.SinAsignar || userRol == ERolTeam.Impulsor)
            {
                var workDayUpdated = FinishWorkdayProcess(workday);
                //new RouteTeamTravelsService().SetClosingStatusRoutTeamTravels(workday.WorkdayId);
                if (userRol == ERolTeam.Impulsor)
                {
                    //OPCD Start
                    var routeTeam = db.so_route_team.Where(x => x.userId == workday.UserId).First();
                    var route = db.so_route.Where(x => x.routeId == routeTeam.routeId).First();
                  
                    finalizarJornadaOPCD(route.so_branch.code, route.code, DateTime.Today, workDayUpdated.DateEnd);
                    //OPCD End

                    using (var service = CustomerBlockedService.Create())
                    {
                        var response = service.ClearBlockedCustomer(new Models.Requests.ClearBlockedCustomerRequest
                        {
                            UserId = workday.UserId
                        });
                    }
                }
            }
            
            workday.IsOpen = false;
            return workday;
        }

        public Workday FinishWorkdayProcess(Workday workday)
        {
            int UserId = workday.UserId;
            Guid WorkdayId = workday.WorkdayId;

            var currentWorkDay = db.so_work_day.Where(w =>
                w.userId == UserId &&
                w.work_dayId == WorkdayId
            );

            if (!currentWorkDay.Any())
                throw new WorkdayNotFoundException();

            var start = currentWorkDay.FirstOrDefault().date_start.Value;

            if (currentWorkDay.FirstOrDefault().date_end.HasValue)
            {
                workday.IsOpen = false;
                workday.DateEnd = currentWorkDay.FirstOrDefault().date_end.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            }

            else
            {

                ERolTeam userRol = roleTeamService.GetUserRole(UserId);
                //checamos que se tenga visita para todos los clientes
                var today = DateTime.Today;
                var query = db.so_venta_View.Where(v => v.id_user == UserId
                    && DbFunctions.TruncateTime(v.inventory_date) >= DbFunctions.TruncateTime(start)
                    && DbFunctions.TruncateTime(v.inventory_date) <= DbFunctions.TruncateTime(today));
                var visits = db.so_binnacle_visit.Where(
                    b => b.userId.Equals(UserId)
                    && DbFunctions.TruncateTime(b.createdon) >= DbFunctions.TruncateTime(start)
                    && DbFunctions.TruncateTime(b.createdon) <= DbFunctions.TruncateTime(today)
                ).Select(c => c.customerId).ToList();

                var count = query.Where(v =>!visits.Contains(v.id_customer)).Count();

                if (count > 0)
                    throw new NoCustomerVisitException("clientes pendientes en este viaje");

                var finishWorkDay = currentWorkDay.FirstOrDefault();

                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        finishWorkDay.closedby_device = UserId;
                        finishWorkDay.date_end = DateTime.Now;
                        finishWorkDay.modifiedon = DateTime.Now;
                        workday.DateEnd = finishWorkDay.date_end.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                        TemporalCloseInventory(finishWorkDay.so_user);

                        db.SaveChanges();
                        workday.IsOpen = false;

                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
                //}
            }
            return workday;
        }

        private void TemporalCloseInventory(so_user so_user)
        {
            var activeInventory = so_user.so_inventory.Where(i => i.state == 1 && i.status).OrderByDescending(i=> i.date).FirstOrDefault();
            if (activeInventory != null)
                activeInventory.state = 2;
        }

        private void VerifyAnotherInventory(so_user user, DateTime date)
        {

            var PendingInventory = db.so_inventory.Where(i => i.state == 0 && i.status
               && i.userId == user.userId).OrderBy(x => x.date).ThenBy(x => x.order).FirstOrDefault();

            if (PendingInventory != null) {
                string msg = System.String.Format("Tienes pendiente el viaje {0} del {1}, Confirma con bodega e inicia viaje",PendingInventory.order,PendingInventory.date.ToShortDateString());
                throw new NoCustomerVisitException(msg);
            }
                    
           
        }

        private so_work_day GetDriverWorkDayByAssistantId(int assistantId)
        {
            RouteTeamService routeTeamService = new RouteTeamService();
            int driverId = routeTeamService.getDriverIdByAssistant(assistantId);
            return searchWorkDay(driverId);
        }

        private so_work_day searchWorkDay(int userId)
        {
            so_work_day currentWorkday = db.so_work_day.Where(
                w => w.userId == userId
                && !w.date_end.HasValue
                && w.status
                ).FirstOrDefault();
            return currentWorkday;
        }

        public Workday getWorkDay(string workDayId)
        {
            var workDay = db.so_work_day.Where(w => w.work_dayId == new Guid(workDayId) && w.status).FirstOrDefault();

            if(workDay == null)
            {
                return null;
            }

            Workday workDayDto = new Workday();
            
            workDayDto.IsOpen = workDay.date_end == null ? true : false;
            workDayDto.UserId = workDay.userId == null ? 0 : (int)workDay.userId;
            workDayDto.WorkdayId = workDay.work_dayId;
            workDayDto.CloseByDevice = workDay.closedby_device == null ? false : true;
            workDayDto.CloseByPortal = workDay.closedby_portal == null ? false : true;
            workDayDto.NameUserClosedSession = "Desconocido";


            workDayDto.DateEnd = String.Format("{1}", "es-MX", workDay.date_end);

            if (workDay.closedby_portal != null)
            {
                var userPortal = db.so_user_portal.Where(up => up.user_portalId == workDay.closedby_portal && up.status).FirstOrDefault();
                if(userPortal != null)
                {
                    workDayDto.NameUserClosedSession = userPortal.name;
                }
            }
            else
            {
                var user = db.so_user.Where(u => u.userId == workDay.closedby_device && u.status).FirstOrDefault();
                if (user != null)
                    workDayDto.NameUserClosedSession = user.name;
            }

            return workDayDto;
        }

        public string finalizarJornadaOPCD(string branchCode, string routeCode, DateTime deliveryDate, string createdOnWbc)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(ConfigurationManager.AppSettings["API_Integraciones"]);
            var requestConfig = new RestRequest("api/jornadasFinalizadasV2", Method.POST);
            requestConfig.RequestFormat = DataFormat.Json;
            requestConfig.AddBody(new { CedisIdOpecd = branchCode, Route = routeCode, DeliveryDate = deliveryDate.ToString("yyyy-MM-dd"), State = 0, CreatedOnWbc = createdOnWbc });
            var RestResponse = client.Execute(requestConfig);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return "OPCD finalizadó con exito";
            }
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                return "No insertado. Ya existe un registro con los mismos datos";
            }
            return "OPCD Falló en notificación";
        }
    }
}