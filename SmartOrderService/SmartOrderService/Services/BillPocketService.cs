using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Models.Message;
using SmartOrderService.Models.Requests;
using SmartOrderService.Models.Responses;
using SmartOrderService.UnitOfWork;
using SmartOrderService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SmartOrderService.Services
{
    public class BillPocketService : IDisposable
    {
        public static BillPocketService Create() => new BillPocketService();

        private UoWConsumer UoWConsumer { get; set; }

        public BillPocketService()
        {
            UoWConsumer = new UoWConsumer();
        }

        public ResponseBase<BillPocketTokensResponse> GetTokensByUserId(int routeId)
        {
            int? branchId = UoWConsumer.RouteRepository.Get(x => x.routeId == routeId).Select(x => x.branchId).FirstOrDefault();
            var token = UoWConsumer.ConfigurationWorkByCloudRepository.Get(x => x.branchId == branchId).Select(x => x.BillPocket_TokenUsuario).FirstOrDefault();
            if (token != null)
            {
                return ResponseBase<BillPocketTokensResponse>.Create(new BillPocketTokensResponse
                {
                    BillPocket_TokenUsuario = token
                });
            }
            else
            {
                return ResponseBase<BillPocketTokensResponse>.Create(new List<string>
                {
                    "No se cuenta con una cuenta de BillPocket para esta ruta."
                });
            }
        }

        public ResponseBase<CheckBillPocketSalesResponse> CheckBillPocketSales(CheckBillPocketSalesRequest request)
        {
            #region Validation
            if (request.WorkDayId == null)
                throw new ArgumentNullException("El WorkDayId es requerido");
            #endregion

            CheckOngoinTravels(request.WorkDayId);

            so_work_day workDay = UoWConsumer.WorkDayRepository.Get(x => x.work_dayId == request.WorkDayId && x.date_end != null).FirstOrDefault();

            if (workDay == null)
                throw new WorkdayNotFoundException("Jornada finalizada.");

            var sales = GetBillpocketSales(request.WorkDayId, request.RouteId, request.UserId);

            return ResponseBase<CheckBillPocketSalesResponse>.Create(new CheckBillPocketSalesResponse()
            {
                TotalSales = sales.Count(),
                HasSales = sales.Count() > 0
            });
        }

        private List<so_sale> GetBillpocketSales(Guid workdayId, int routeId, int? userId)
        {
            List<int> inventories = UoWConsumer.RouteTeamTravelsEmployeesRepository.Get(x => x.work_dayId == workdayId)
               .Select(x => x.inventoryId)
               .Distinct()
               .ToList();

            if (inventories.Count == 0)
                throw new EntityNotFoundException("No se encontraron viajes para la jornada " + workdayId.ToString());

            so_work_day workDay = UoWConsumer.WorkDayRepository.Get(x => x.work_dayId == workdayId).FirstOrDefault();

            if(workDay == null)
                throw new EntityNotFoundException("No se encontraró la jornada " + workdayId.ToString());

            Expression<Func<so_sale, bool>> filter = x => x.status && inventories.Contains(x.inventoryId.Value) && workDay.date_start <= x.date;

            if (userId == null)
            {
                List<int> users = UoWConsumer.RouteTeamRepository.Get(x => x.routeId == routeId)
                    .Select(x => x.userId)
                    .ToList();

                filter.And(x => users.Contains(x.userId));
            }
            else
                filter.And(x => x.userId == userId);

            var sales = UoWConsumer.SaleRepository.Get(filter).ToList();
            List<int> salesIds = sales.Select(x => x.saleId).ToList();
            //Se busca las de billpocket
            var billSales = UoWConsumer.SaleAdditionalDataRepository.Get(x => salesIds.Contains(x.saleId) && x.paymentMethod == "tarjeta").Select(x => x.saleId).ToList();

            filter.And(x => billSales.Contains(x.saleId));

            return sales.Where(x => billSales.Contains(x.saleId)).ToList();
        }

        public void CheckOngoinTravels(Guid workDayId)
        {
            //Si existe carga abierta evitar el envio
            var cargas = UoWConsumer.RouteTeamTravelsEmployeesRepository
                .Get(x => x.work_dayId == workDayId && x.active)
                .FirstOrDefault();

            //if (cargas != null)
            //    throw new InventoryInProgressException("Existe una carga abierta");
        }


        public ResponseBase<MsgResponseBase> SendBillPocketReport(SendBillPocketReportRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
                throw new ArgumentNullException("El Email es requerido.");

            CheckOngoinTravels(request.WorkDayId);

            so_work_day workDay = UoWConsumer.WorkDayRepository.Get(x => x.work_dayId == request.WorkDayId && x.date_end == null).FirstOrDefault();

            if (workDay == null)
                throw new WorkdayNotFoundException("Jornada finalizada.");

            var sales = GetBillpocketSales(request.WorkDayId, request.RouteId, request.UserId);

            if (sales.Count() == 0)
                throw new EntityNotFoundException("No se encontraron ventas");

            so_route route = UoWConsumer.RouteRepository.Get(x => x.routeId == request.RouteId).FirstOrDefault();

            if (route == null)
                throw new EntityNotFoundException("No se encontró la ruta");

            var user = UoWConsumer.RouteTeamRepository.Get(x => x.userId == request.UserId).FirstOrDefault();

            so_billpocket_report_log reportData = new so_billpocket_report_log()
            {
                TotalAmount = sales.Sum(x => x.total_cash),
                RouteId = request.RouteId,
                SendDate = DateTime.Now,
                TotalSales = sales.Count(),
                UserId = request.UserId,
                WorkDayId = request.WorkDayId,
                createdby = request.UserId ?? 2777,
                createdon = DateTime.Now
            };

            //Enviar el email
            var emailService = new EmailService();
            emailService.SendBillPocketReportEmail(new SendBillPocketReportEmailRequest()
            {
                TotalAmount = reportData.TotalAmount,
                BranchName = route.so_branch.name,
                Email = request.Email,
                RouteName = route.code + " - " + route.name,
                SendDate = reportData.SendDate,
                TotalSales = reportData.TotalSales,
                UserRole = user.roleTeamId == 1 ? "Impulsor" : "Ayudante",
                WorkDayDate = workDay.date_start.Value
            });

            //Registrar el envio
            UoWConsumer.BillPocketReportLogReporitory.Insert(reportData);
            UoWConsumer.Save();

            return ResponseBase<MsgResponseBase>.Create(new MsgResponseBase()
            {
                Msg = "OK"
            });
        }

        public void Dispose()
        {
            this.UoWConsumer.Dispose();
            this.UoWConsumer = null;
        }
    }
}
