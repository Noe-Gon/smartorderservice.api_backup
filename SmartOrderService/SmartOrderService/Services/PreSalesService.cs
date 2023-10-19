using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Interfaces;
using SmartOrderService.Models.DTO;
using System;
using System.Configuration;
using System.Linq;

namespace SmartOrderService.Services
{
    public sealed class PreSalesService : IPreSalesService
    {
        private SmartOrderModel _db = new SmartOrderModel();
        public IAPIPreventaService _apiPreventaService { get; set; }
        private int _version { get; set; }
        private static PreSalesService _instance;
        private PreSalesService()
        {
            _apiPreventaService = APIPreventaService.GetInstance();
            int numberVersion;
            try
            {
                numberVersion = Int32.Parse(ConfigurationManager.AppSettings["API_Preventa_Version"]);
            }
            catch (Exception)
            {
                throw new Exception("El valor 'API_Preventa_Version' en el archivo de conifguración no se encuentra con el formato correcto, revisar el formato por favor.");
            }
            _version = numberVersion;
        }
        public static PreSalesService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PreSalesService();
            }
            return _instance;
        }

        public bool SendPreSales(SendPreSalesDTO request)
        {
            bool response = false;
            if (string.IsNullOrEmpty(request.WorkDayId))
            {
                throw new BadRequestException("Se requiere el campo WorkDayId");
            }

            try
            {
                var workDay = _db.so_work_day.Where(x => x.work_dayId.ToString() == request.WorkDayId).FirstOrDefault();
                if (workDay == null)
                {
                    throw new WorkdayNotFoundException($"No se encontró la jornada de '{request.WorkDayId}'");
                }
                var routeTeam = _db.so_route_team.Where(x => x.userId == workDay.userId.Value).FirstOrDefault();
                var route = _db.so_route.Where(x => x.routeId == routeTeam.routeId).FirstOrDefault();
                var branch = _db.so_branch.Where(x => x.branchId == route.branchId).FirstOrDefault();

                ClosingPreclosingDTO requestApiPreventa = new ClosingPreclosingDTO()
                {
                    branchCode = branch.code,
                    routeCode = route.code,
                    version = _version
                };
                _apiPreventaService.SendPreSales(requestApiPreventa);
                response = true;
            }
            catch (WorkdayNotFoundException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
            return response;
        }
    }
}