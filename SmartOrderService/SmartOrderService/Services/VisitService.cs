using SmartOrderService.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartOrderService.Models.DTO;
using System.Data.Entity;
using SmartOrderService.CustomExceptions;
using SmartOrderService.Models.Responses;

namespace SmartOrderService.Services
{
    public class VisitService
    {
        SmartOrderModel db = new SmartOrderModel();
        public List<VisitDto> getVisits(int userId)
        {

            var soUser = db.so_user.Where(u => u.userId == userId).FirstOrDefault();
            int day = (int)DateTime.Today.DayOfWeek;
            day++;

            if (soUser.type == so_user.POAC_TYPE || soUser.type == so_user.CCEH_TYPE)
            {
                InventoryService inventoryService = new InventoryService();
                try
                {
                    userId = inventoryService.SearchDrivingId(userId);
                }
                catch (RelatedDriverNotFoundException e)
                {}

                so_inventory inventory = inventoryService.getCurrentInventory(userId, null);

                List<VisitDto> visits = new List<VisitDto>();

                var customers = new List<int>();

                if (inventory != null)
                {
                    int inventoryId = inventory.inventoryId;

                    customers = db.so_delivery.Where(d => d.inventoryId.Equals(inventoryId) && d.status).Select(c => c.customerId).ToList();
                }

                var routeVisits = db.so_user_route
                .Join(db.so_route_customer,
                    userRoute => userRoute.routeId,
                    customerRoute => customerRoute.routeId,
                    (userRoute, customerRoute) => new { userRoute.userId, customerRoute.customerId, customerRoute.day, customerRoute.order, customerRoute.status, userRouteStatus = userRoute.status }
                )
                .Where(
                    v => v.userId.Equals(userId)
                    //&& !customers.Contains(v.customerId)
                    && v.userRouteStatus
                    && v.status
                    && day.Equals(v.day)
                )
                    .Select(c => new { c.customerId, c.order });
                foreach (var data in routeVisits)
                {


                    int order = data.order;

                    if (inventory != null && inventory.status)
                    {
                        var delivery = db.so_delivery.Where(d => d.customerId == data.customerId && d.status && d.inventoryId == inventory.inventoryId).FirstOrDefault();
                        if (delivery != null && delivery.visit_order != null && delivery.visit_order != 0)
                        {
                            order = (int)delivery.visit_order;
                        }
                    }

                    VisitDto dto = new VisitDto()
                    {
                        CustomerId = data.customerId,
                        Order = order,
                        Visited = db.so_binnacle_visit.Where(bv => bv.customerId == data.customerId &&
                            DbFunctions.TruncateTime(bv.createdon) == DbFunctions.TruncateTime(DateTime.Now))
                            .FirstOrDefault() != null
                    };

                    visits.Add(dto);

                }

                var otherVisits = customers.Where(c => !routeVisits.Select(rv => rv.customerId).Contains(c));

                foreach (var otherVisit in customers)
                {
                    if (routeVisits.Select(rv => rv.customerId).Contains(otherVisit))
                    {
                        VisitDto dtotemp = visits.Where(v => v.CustomerId == otherVisit).FirstOrDefault();
                        if (dtotemp != null)
                            dtotemp.Visited = false;
                    }
                    else
                    {

                        VisitDto dto = new VisitDto()
                        {
                            CustomerId = otherVisit,
                            Order = 0,
                            Visited = false
                        };
                        visits.Add(dto);
                    }


                }

                //visits.Select(v => rou)

                //db.so_binnacle_visit.Where(bv => visits.Contains(bv.customerId)).


                return visits;
            }
            else
            {

                so_inventory inventory = new InventoryService().getCurrentInventory(userId, null);

                List<VisitDto> visits = new List<VisitDto>();


                if (inventory != null)
                {
                    int inventoryId = inventory.inventoryId;

                    var customers = db.so_delivery.Where(d => d.inventoryId.Equals(inventoryId) && d.status).Select(c => c.customerId).ToList();

                    var routeVisits = db.so_user_route
                        .Join(db.so_route_customer,
                            userRoute => userRoute.routeId,
                            customerRoute => customerRoute.routeId,
                            (userRoute, customerRoute) => new { userRoute.userId, customerRoute.customerId, customerRoute.day, customerRoute.order }
                        )
                        .Where(
                            v => v.userId.Equals(userId)
                            && customers.Contains(v.customerId)
                            && day.Equals(v.day)
                        )
                            .Select(c => new { c.customerId, c.order });

                    foreach (var customerId in customers)
                    {

                        var data = routeVisits.Where(r => r.customerId == customerId).FirstOrDefault();

                        int order = data != null ? data.order : 0;

                        if (inventory != null && inventory.status)
                        {
                            var delivery = db.so_delivery.Where(d => d.customerId == customerId && d.status && d.inventoryId == inventory.inventoryId).FirstOrDefault();
                            if (delivery != null && delivery.visit_order != null && delivery.visit_order != 0)
                            {
                                order = (int)delivery.visit_order;
                            }
                        }


                        VisitDto dto = new VisitDto()
                        {
                            CustomerId = customerId,
                            Order = order
                        };


                        visits.Add(dto);
                    }
                }

                return visits;
            }

        }

        public List<GetTeamVisitResponse> getTeamVisits(int userId, int? inventoryId)
        {
            RoleTeamService roleTeamService = new RoleTeamService();
            RouteTeamService routeTeamService = new RouteTeamService();
            InventoryService inventoryService = new InventoryService();
            var impulsor = inventoryService.SearchDrivingId(userId);

            if (inventoryId == null || inventoryId == 0)
                inventoryId = inventoryService.getCurrentInventory(userId, null).inventoryId;

            var soUser = db.so_user.Where(u => u.userId == userId).FirstOrDefault();
            var date = DateTime.Today; 
            
            var routeId = routeTeamService.searchRouteId(impulsor);
            var workDay = routeTeamService.GetWorkdayByUserAndDate(impulsor, DateTime.Today);

            var visits = db.so_route_team_travels_visits
                .Where(x => x.workDayId == workDay.work_dayId && x.routeId == routeId)
                .Select(x => x.So_Binnacle_Visit)
                .Select(x => new GetTeamVisitResponse
                {
                    VisitId = x.binnacleId,
                    CustomerId = x.customerId,
                    UserId = x.userId,
                    CheckIn = x.checkin,
                    CheckOut = x.checkout,
                    LatitudeIn = x.latitudein,
                    LatitudeOut = x.latitudeout,
                    LongitudeIn = x.longitudein,
                    LongitudeOut = x.longitudeout,
                    ReasonFailed = x.so_binnacle_reason_failed.Count > 0 ? x.so_binnacle_reason_failed.FirstOrDefault().reasonId : (int?)null,
                    Scanned = x.scanned
                })
                .ToList();

            return visits;
        }

        public int getDay(DateTime datetime)
        {
            switch (datetime.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return 1;
                case DayOfWeek.Monday:
                    return 2;
                case DayOfWeek.Tuesday:
                    return 3;
                case DayOfWeek.Wednesday:
                    return 4;
                case DayOfWeek.Thursday:
                    return 5;
                case DayOfWeek.Friday:
                    return 6;
                case DayOfWeek.Saturday:
                    return 7;
                default:
                    return 0;
            }
        }
    }
}