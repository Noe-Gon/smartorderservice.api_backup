using Newtonsoft.Json;
using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Models.DTO;
using SmartOrderService.Models.Enum;
using SmartOrderService.Models.Responses;
using SmartOrderService.Models.Message;
using SmartOrderService.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SmartOrderService.Services
{
    public class RouteTeamService
    {
        private SmartOrderModel db = new SmartOrderModel();
        private RoleTeamService roleTeamService = new RoleTeamService();
        private RouteTeamTravelsService routeTeamTravelsService = new RouteTeamTravelsService();

        public bool CheckCurrentTravelState(int userId)
        {
            try
            {
                ERolTeam userRole = roleTeamService.GetUserRole(userId);
                if (userRole == ERolTeam.SinAsignar)
                {
                    return true;
                }
                 var inventory = GetCurrentInventory(userId, DateTime.Today);
                int inventoryState = inventory.state;

                //Start Load Inventory Process OPCD
                CallLoadInventoryProcess(userId);
                //End Load Inventory Process
                int impulsorId = SearchDrivingId(userId);

                try
                {
                    if (IsSettlementSent(GetWorkdayByUserAndDate(impulsorId, DateTime.Now).work_dayId))
                        throw new SettlementSentException();
                }
                catch (SettlementSentException e)
                {
                    throw e;
                }
                catch (Exception) { }

                if (IsActualOpened(userId, inventory.inventoryId))
                    return true;

                if ((inventoryState == 0 && userRole == ERolTeam.Impulsor))
                {
                    CheckOpenedTravalers(userId);
                    return true;
                }
                if (((inventoryState == 1 || inventoryState == 2) && userRole == ERolTeam.Impulsor))
                {
                    throw new InventoryNotClosedByUserException("El viaje anterior no ha sido cerrado por otro usuario");
                }
                if (inventoryState == 1)
                {
                    CheckIfCurrentTravelsIsNewByUser(userId);

                    return true;
                }
                if (inventoryState == 0 && userRole == ERolTeam.Ayudante)
                    throw new InventoryNotOpenException();

                return false;
            }
            catch (InventoryInProgressException)
            {
                //Start Load Inventory Process OPCD
                CallLoadInventoryProcess(userId);
                //End Load Inventory Process

                throw new InventoryInProgressException();
            }
            catch (InventoryEmptyException)
            {
                //Start Load Inventory Process OPCD
                CallLoadInventoryProcess(userId);
                //End Load Inventory Process

                throw new InventoryEmptyException();
            }

        }

        public bool IsSettlementSent(Guid workDayId)
        {
            var log = db.so_liquidation_logs
                    .Where(x => x.WorkDayId == workDayId && x.ExecutionIdAws != null)
                    .OrderByDescending(x => x.CreatedOn)
                    .FirstOrDefault();

            return log != null;
        }

        private bool IsActualOpened(int userId, int inventoryId)
        {
            var routeTeam = db.so_route_team_travels_employees.Where(x => x.userId == userId && x.inventoryId == inventoryId).FirstOrDefault();
            return routeTeam == null ? false : routeTeam.active;
        }

        public void CheckIfCurrentTravelsIsNewByUser(int userId)
        {
            InventoryService inventoryService = new InventoryService();

            var impulsorId = SearchDrivingId(userId);
            var inventory = inventoryService.GetCurrentInventory(impulsorId, DateTime.Today);
            var workDay = GetWorkdayByUserAndDate(impulsorId, DateTime.Today);

            //Verificar que en el inventario actual yo no este
            var travel = db.so_route_team_travels_employees
                .Where(x => x.inventoryId == inventory.inventoryId && x.work_dayId == workDay.work_dayId
                    && x.userId == userId)
                .FirstOrDefault();

            if (travel != null)
                throw new InventoryNotClosedByUserException("El viaje anterior no ha sido cerrado por otro usuario");
        }

        public void CheckOpenedTravalers(int impulsorId)
        {
            var workDay = GetWorkdayByUserAndDate(impulsorId, DateTime.Today);

            var traval = db.so_route_team_travels_employees
                .Where(x => x.work_dayId == workDay.work_dayId && x.active)
                .FirstOrDefault();

            if (traval == null)
                return;

            throw new InventoryNotClosedException();
        }

        public void CallLoadInventoryProcess(int userId)
        {
            var inventoryService = new InventoryService();
            int impulsorId = SearchDrivingId(userId);
            var routeTeam = db.so_route_team.Where(x => x.userId == impulsorId).First();
            var route = db.so_route.Where(x => x.routeId == routeTeam.routeId).First();

            inventoryService.CallLoadInventoryProcess(impulsorId, route.so_branch.code, route.code, null);
        }

        public List<int> GetTeamIds(int userId)
        {
            int routeId = searchRouteId(userId);
            return db.so_route_team
                .Where(s => s.routeId.Equals(routeId))
                .Select(s => s.userId).ToList();
        }

        public bool checkDriverWorkDay(int userId)
        {
            /*
            ERolTeam userRole = roleTeamService.GetUserRole(userId);
            if (userRole == ERolTeam.Impulsor)
            {
                try
                {
                    var userWorkDay = GetWorkdayByUserAndDate(userId, DateTime.Today);
                }
                catch (WorkdayNotFoundException e)
                {
                    return true;
                }
                return false;
            }
            if (userRole == ERolTeam.Ayudante)
            {
                try
                {
                    int driverId = getDriverIdByAssistant(userId);
                    var userWorkDay = GetWorkdayByUserAndDate(driverId, DateTime.Today);
                }
                catch (WorkdayNotFoundException e)
                {
                    return false;
                }
                return true;
            }
            */
            return true;
        }

        public bool CheckTravelClosingStatus(int userId, int inventoryId)
        {
            ERolTeam userRole = roleTeamService.GetUserRole(userId);
            if (userRole == ERolTeam.SinAsignar)
            {
                return true;
            }

            //Start Load Inventory Process OPCD
            var inventoryService = new InventoryService();
            int impulsorId = SearchDrivingId(userId);
            var routeTeam = db.so_route_team.Where(x => x.userId == impulsorId).First();
            var route = db.so_route.Where(x => x.routeId == routeTeam.routeId).First();
            var inventory = db.so_inventory.Where(x => x.inventoryId == inventoryId).FirstOrDefault();
            inventoryService.CallLoadInventoryProcess(impulsorId, route.so_branch.code, route.code, null);
            //End Load Inventory Process

            //Si es de Viaje no sincronizado devolver true
            var workDay = GetWorkdayByUserAndDate(impulsorId, inventory.date);
            var isInTravelsemployees = db.so_route_team_travels_employees
                .Where(x => x.userId == userId && x.inventoryId == inventoryId && x.work_dayId == workDay.work_dayId)
                .FirstOrDefault() != null;

            if (isInTravelsemployees)
                return true;
            //Fin de validación de viaje no sincronizado

            if (userRole == ERolTeam.Impulsor)
            {
                if (GetInventoryState(userId,DateTime.Today) == 1 && routeTeamTravelsService.getTravelStatusByInventoryId(inventoryId) == EInventoryTeamStatus.InventarioCerradoPorAsistente)
                {
                    return true;
                }
                return false;
            }
            if (GetInventoryState(userId, DateTime.Today) == 1 && routeTeamTravelsService.getTravelStatusByInventoryId(inventoryId) == EInventoryTeamStatus.InventarioAbiertoPorAyudante)
            {
                return true;
            }
            return false;
        }

        public int searchRouteId(int userId)
        {
            so_route_team routeTeam = db.so_route_team.Where(
                i => i.userId == userId
                ).FirstOrDefault();
            if (routeTeam == null)
            {
                throw new RelatedDriverNotFoundException(userId);
            }
            return routeTeam.routeId;
        }

        public int GetRouteId(int userId)
        {
            so_user_route routeTeam = db.so_user_route.Where(
                i => i.userId == userId && i.status == true
                ).FirstOrDefault();
            if (routeTeam == null)
            {
                return 0;
            }
            return routeTeam.routeId;
        }

        public so_work_day GetWorkdayByUserAndDate(int userId, DateTime date)
        {
            so_work_day workday =  db.so_work_day.Where(
                i => i.userId == userId
                && DbFunctions.TruncateTime(i.date_start) == DbFunctions.TruncateTime(date)
                ).FirstOrDefault();

            if (workday == null)
                throw new WorkdayNotFoundException("No se encontro la jornada para el usuario " + userId + " y el dia " + date);
            
            return workday;
        }


        public int GetInventoryState(int userId, DateTime date)
        {
            InventoryService inventoryService = new InventoryService();
            if (date == null)
            {
                date = DateTime.Today;
            }
            userId = SearchDrivingId(userId);
            var inventory = inventoryService.GetCurrentInventory(userId,date);
            return inventory.state;
        }

        public so_inventory GetCurrentInventory(int userId, DateTime date)
        {
            InventoryService inventoryService = new InventoryService();
            if (date == null)
            {
                date = DateTime.Today;
            }
            userId = SearchDrivingId(userId);
            var inventory = inventoryService.GetCurrentInventory(userId, date);
            return inventory;
        }

        public bool CheckWorkDayClosingStatus(int userId)
        {
            
            ERolTeam userRole = roleTeamService.GetUserRole(userId);
            if (userRole == ERolTeam.SinAsignar)
            {
                return true;
            }
            /*if (userRole == ERolTeam.Impulsor)
            {
                var userWorkDay = GetWorkdayByUserAndDate(userId, DateTime.Today);
                if (userWorkDay == null)
                {
                    throw new WorkdayNotFoundException();
                }
                if (userWorkDay.date_end == null && routeTeamTravelsService.CheckTravelsClosingStatus(userId,DateTime.Today))
                {
                    return true;
                }
                return false;
            }
            return true;
            */
            
            int impulsorId = SearchDrivingId(userId);
            so_work_day workDay = GetWorkdayByUserAndDate(impulsorId, DateTime.Today);

            int userTravel = db.so_route_team_travels_employees
                .Where(x => x.work_dayId == workDay.work_dayId && x.active)
                .Count();
            if (userTravel > 0)
            {
                return false;
            }

            return true;
        }

        public bool CheckWorkDayClosingStatusByWorkDay(Workday workDay, string version = "v2")
        {

            ERolTeam userRole = roleTeamService.GetUserRole(workDay.UserId);
            if (userRole == ERolTeam.SinAsignar)
            {
                return true;
            }

            int impulsorId = SearchDrivingId(workDay.UserId);
            so_work_day workDayCurrent = db.so_work_day.Where(w =>
                w.work_dayId == workDay.WorkdayId
            ).FirstOrDefault();

            if (workDayCurrent == null)
                throw new WorkdayNotFoundException("No se encontro la jornada para el usuario " + impulsorId);

            int userTravel = db.so_route_team_travels_employees
                .Where(x => x.work_dayId == workDayCurrent.work_dayId && x.active)
                .Count();
            if (IsInOpe20(workDayCurrent))
            {
                userTravel = 0;
            }

            if (userTravel > 0)
                return false;

            if (version == "v3")
            {
                CloseInventoryOpe20(workDayCurrent);
            }

            if (workDay.CheckBillpocket)
                return ChalBillPocketReportForAllUsers(workDay.WorkdayId, workDay.UserId);

            

            return true;
        }

        public GetTravelsInProcessResponse GetTravelsInProcess(int userId, string date, List<ERolTeam> avoid = null, List<ERolTeam> specificSearch = null)
        {
            GetTravelsInProcessResponse result = new GetTravelsInProcessResponse();
            if (userId == 0)
            {
                throw new BadRequestException("Se requiere el campo UserId, no puede ser 0");
            }
            if (userId < 0)
            {
                throw new BadRequestException("El campo UserId no puede ser un número negativo");
            }
            //ERolTeam userRole = roleTeamService.GetUserRole(userId);
            DateTime dateStart = DateTime.Now;
            if (!string.IsNullOrEmpty(date))
            {
                try
                {
                    dateStart = DateTime.Parse(date);
                }
                catch (Exception e)
                {
                    throw new BadRequestException($"La fecha \"{date}\" no se pudo parsear, revisar el formato sea del tipo \"yyyy-MM-dd\" con fechas válidas");
                }
            }

            try
            {
                var routeId = db.so_route_team.Where(x => x.userId == userId).Select(x => x.routeId).FirstOrDefault();
                if (routeId == 0)
                {
                    throw new EntityNotFoundException($"No se encontró un equipo para el usuario con identificador de {userId}");
                }

                var usersTeam = GetUsersTeamByFilter(routeId, null, null);
                var usersTeamFilter = GetUsersTeamByFilter(routeId, avoid, specificSearch);

                var inventories = db.so_inventory.Where(x => usersTeam.Contains(x.userId) && DbFunctions.TruncateTime(x.date) == DbFunctions.TruncateTime(dateStart) && x.status).ToList();
                var inventoriesId = inventories.Select(x => x.inventoryId).ToList();

                var inventorieFilters = db.so_inventory.Where(x => usersTeamFilter.Contains(x.userId) && DbFunctions.TruncateTime(x.date) == DbFunctions.TruncateTime(dateStart) && x.status && x.state == 1).Select(x => x.inventoryId).ToList();
                var inventoriesTeamEmployees = db.so_route_team_travels_employees.Where(x => usersTeamFilter.Contains(x.userId) && inventoriesId.Contains(x.inventoryId) && x.active).Select(x => x.inventoryId).ToList();

                if (inventories.Count > 0 && (inventorieFilters.Count > 0 || inventoriesTeamEmployees.Count > 0))
                {
                    var inventoriesOpenId = inventorieFilters.Union(inventoriesTeamEmployees).Distinct().ToList();
                    var inventoriesOpen = inventories.Where(x => inventoriesOpenId.Contains(x.inventoryId)).ToList();
                    foreach (var inventory in inventoriesOpen)
                    {
                        result.TravelsInProcess.Add(new TravelsInProcess
                        {
                            InventoryId = inventory.inventoryId,
                            Code = inventory.code
                        });
                    }
                    if (result.TravelsInProcess.Count > 0)
                    {
                        result.IsInProcess = true;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        private List<int> GetUsersTeamByFilter(int routeId, List<ERolTeam> avoid, List<ERolTeam> specificSearch)
        {
            if (specificSearch == null)
            {
                specificSearch = new List<ERolTeam>();
            }
            if (avoid == null)
            {
                avoid = new List<ERolTeam>();
            }
            var users = db.so_route_team.Where(x => x.routeId == routeId).Select(x => x.userId).ToList();
            List<int> resultFilter = new List<int>();
            if (specificSearch.Count > 0)
            {
                foreach (var user in users)
                {
                    if (specificSearch.Contains(roleTeamService.GetUserRole(user)))
                    {
                        resultFilter.Add(user);
                    }
                }
                return resultFilter;
            }
            if (avoid.Count > 0)
            {
                foreach (var user in users)
                {
                    if (!avoid.Contains(roleTeamService.GetUserRole(user)))
                    {
                        resultFilter.Add(user);
                    }
                }
                return resultFilter;
            }
            return users;
        }

        public bool ChalBillPocketReportForAllUsers(Guid workdayId, int userId)
        {
            List<int> users = db.so_route_team_travels_employees.Where(x => x.work_dayId == workdayId)
                .Select(x => x.userId)
                .Distinct()
                .ToList();

            foreach (var user in users)
            {
                try
                {
                    CheckBillPocketReport(workdayId, user);
                }
                catch (EntityNotFoundException e)
                {
                    if (user == userId)
                        throw new EntityNotFoundException();
                    else
                        throw new BillpocketReportException();
                }
            }

            return true;
        }

        public bool CheckBillPocketReport(Guid workdayId, int userId)
        {
            //Se buscan las ventas
            List<int> inventories = db.so_route_team_travels_employees.Where(x => x.work_dayId == workdayId)
               .Select(x => x.inventoryId)
               .Distinct()
               .ToList();

            if (inventories.Count == 0)
                throw new EntityNotFoundException("No se encontraron viajes para la jornada " + workdayId.ToString());

            so_work_day workDay = db.so_work_day.Where(x => x.work_dayId == workdayId).FirstOrDefault();

            if (workDay == null)
                throw new EntityNotFoundException("No se encontraró la jornada " + workdayId.ToString());

            Expression<Func<so_sale, bool>> filter = x => x.status && inventories.Contains(x.inventoryId.Value) && workDay.date_start <= x.date;

            filter = filter.And(x => x.userId == userId);

            List<int> sales = db.so_sale.Where(filter).Select(x => x.saleId).ToList();

            if (sales.Count() == 0)
                return true;

            //Se busca las de billpocket
            var billSales = db.so_sale_aditional_data.Where(x => sales.Contains(x.saleId) && x.paymentMethod == "tarjeta").ToList();

            if (billSales.Count() == 0)
                return true;

            //Si hay almenos una venta con billpocket
            var report = db.so_billpocket_report_logs.Where(x => x.WorkDayId == workdayId && userId == x.UserId)
                .OrderByDescending(x => x.SendDate).FirstOrDefault();

            if (report == null)
                throw new EntityNotFoundException("No se ha enviado el reporte.");

            //Si las ventas no coinciden
            if (report.TotalSales != billSales.Count())
                throw new EntityNotFoundException("No se ha enviado el reporte final.");

            return true;
        }

        public int getDriverIdByAssistant(int assistantId)
        {
            int assistantRouteId = searchRouteId(assistantId);
            int driverId = searchDriverByRouteId(assistantRouteId).userId;
            return driverId;
        }

        public bool IsImpulsor(int userId)
        {
            int actualUserId = userId;
            try
            {
                int driverId = SearchDrivingId(userId);
                if (actualUserId == driverId)
                {
                    return true;
                }
                return false;
            }
            catch (RelatedDriverNotFoundException e)
            {
                return false;
            }
        }

        public int SearchDrivingId(int actualUserId)
        {
            so_route_team teamRoute = db.so_route_team.Where(i => i.userId == actualUserId).ToList().FirstOrDefault();
            if (teamRoute == null)
            {
                throw new RelatedDriverNotFoundException(actualUserId);
            }
            int DrivingId = db.so_route_team.Where(i => i.routeId == teamRoute.routeId && i.roleTeamId == (int)ERolTeam.Impulsor).ToList().FirstOrDefault().userId;
            return DrivingId;
        }

        private so_route_team searchDriverByRouteId(int routeId)
        {
            so_route_team routeTeam = db.so_route_team.Where(
                i => i.routeId == routeId
                && i.roleTeamId == (int)ERolTeam.Impulsor
                ).FirstOrDefault();
            return routeTeam;
        }

        public ResponseBase<List<GetRouteTeamResponse>> GetRouteTeam(int routeId)
        {
            var routeTeams = db.so_route_team
                .Where(x => x.routeId == routeId)
                .Select(x => new GetRouteTeamResponse
                {
                    RoleId = x.roleTeamId,
                    UserId = x.userId,
                    RoleName = x.roleTeamId == (int)ERolTeam.Impulsor ? "Impulsor" : "Ayudante",
                    UserName = db.so_user.Where(u => u.userId == x.userId).Select(u => u.name).FirstOrDefault()
                }).ToList();

            if (routeTeams.Count == 0)
                throw new EntityNotFoundException("No se encontró equipo para esa ruta");

            return ResponseBase<List<GetRouteTeamResponse>>.Create(routeTeams);
        }

        public bool IsInOpe20(so_work_day workDay)
        {
            Ope20Service service = new Ope20Service();

            var routeTeam = db.so_route_team.Where(x => x.userId == workDay.userId).FirstOrDefault();
            int routeId = routeTeam.routeId;
            var routeBranchCode = db.so_route
                .Where(x => routeId == x.routeId)
                .Select(x => new { RouteCode = x.code, BranchCode = x.so_branch.code }).FirstOrDefault();

            return service.IsCediInOpe20(routeBranchCode.BranchCode);
        }

        public void CloseInventoryOpe20(so_work_day workDay)
        {
            Ope20Service service = new Ope20Service();

            var date = DateTime.Now;
            
            var inventories = db.so_inventory.Where(x => x.userId == workDay.userId && DbFunctions.TruncateTime(x.date) == DbFunctions.TruncateTime(date) && x.status).ToList();
            var inventoriesNotClosed = inventories.Where(x => x.state != 2).ToList();
            var routeTeam = db.so_route_team.Where(x => x.userId == workDay.userId).FirstOrDefault();

            if (inventories == null || inventories.Count == 0)
                return;

            int routeId = routeTeam.routeId;
            var routeBranchCode = db.so_route
                .Where(x => routeId == x.routeId)
                .Select(x => new { RouteCode = x.code, BranchCode = x.so_branch.code }).FirstOrDefault();

            if (!service.IsCediInOpe20(routeBranchCode.BranchCode))
                return;

            var inventoriesFailed = new List<int>();
            var ope20Errors = new List<string>();

            foreach (var inventory in inventories)
            {
                try
                {
                    service.CloseRouteNotification(new CloseRouteNotificationRequest()
                    {
                        BranchCode = routeBranchCode.BranchCode,
                        RouteCode = routeBranchCode.RouteCode,
                        LoadID = inventory.code
                    });
                }
                catch (Exception e)
                {
                    inventoriesFailed.Add(inventory.inventoryId);
                    ope20Errors.Add(e.Message);
                }
            }

            var inventoriesId = inventories.Select(x => x.inventoryId).ToList();
            var routeTeamTravelsEmployees = db.so_route_team_travels_employees.Where(x => inventoriesId.Contains(x.inventoryId)).ToList();

            var dateModified = DateTime.Now;
            foreach (var inventory in inventoriesNotClosed)
            {
                if (!inventoriesFailed.Contains(inventory.inventoryId))
                {
                    inventory.state = 2;
                    inventory.modifiedon = dateModified;
                    //Se hace por medio de una lista debido a que puede existir tanto la del impulsor y ayudante en registros
                    var travelsEmployees = routeTeamTravelsEmployees.Where(x => x.inventoryId == inventory.inventoryId).ToList();
                    foreach (var travelEmployeeToUpdate in travelsEmployees)
                    {
                        travelEmployeeToUpdate.active = false;
                        travelEmployeeToUpdate.modifiedon = dateModified;
                    }
                }
            }

            db.SaveChanges();

            if (inventoriesFailed.Count > 0)
            {
                string inventoriesFailedStr = string.Join(",", inventoriesFailed);
                throw new Ope20Exception(JsonConvert.SerializeObject(new Ope20MessageException
                {
                    Message = $"No se han podido cerrar las siguientes cargas en Ope20: {inventoriesFailedStr}",
                    Ope20Errors = ope20Errors
                }));
            }
        }
    }
}