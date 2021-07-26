using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Models.Enum;
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
    public class ConsumerService : IDisposable
    {
        public static ConsumerService Create() => new ConsumerService();

        private UoWConsumer UoWConsumer { get; set; }

        public ConsumerService()
        {
            UoWConsumer = new UoWConsumer();
        }

        public ResponseBase<InsertConsumerResponse> InsertConsumer(InsertConsumerRequest request)
        {
            try
            {
                var route = UoWConsumer.RouteRepository
                    .GetByID(request.RouteId);

                if(route == null)
                    return ResponseBase<InsertConsumerResponse>
                    .Create(new List<string>() { "No se encontró la ruta" });

                var newCustomer = new so_customer
                {
                    name = request.Name,
                    createdby = request.UserId,
                    createdon = DateTime.Now,
                    email = request.Email,
                    latitude = request.Latitude,
                    longitude = request.Longitude,
                    code = request.CFECode,
                    contact = request.Contact,
                    status = true
                };

                var newCustomerAdditionalData= new so_customer_additional_data
                {
                    Customer = newCustomer,
                    Phone = request.Phone,
                    Phone_2 = request.Phone_2,
                    Email_2 = request.Email_2,
                    Status = (int)Consumer.STATUS.CONSUMER,
                    AcceptedTermsAndConditions = false,
                    IsMailingActive = true,
                    IsSMSActive = false,
                    CodePlace = request.CodePlace,
                    CounterVisitsWithoutSales = 0,
                    InteriorNumber = request.InteriorNumber,
                    Neighborhood = request.Neighborhood,
                    ReferenceCode = request.ReferenceCode
                };

                var newCustomerData = new so_customer_data
                {
                    so_customer = newCustomer,
                    route_code = Convert.ToInt32(route.code),
                    branch_code = Convert.ToInt32(route.so_branch.code),
                    address_number = request.ExternalNumber,
                    address_number_cross1 = request.Crossroads,
                    address_number_cross2 = request.Crossroads_2,
                    address_street = request.Street,
                    status = true
                };

                //Creación de los días
                var newListRouteCustomer = new List<so_route_customer>();
                foreach (var day in request.Days)
                {
                    newListRouteCustomer.Add(new so_route_customer
                    {
                        routeId = request.RouteId,
                        createdby = request.UserId,
                        createdon = DateTime.Now,
                        day = day,
                        order = 0,
                        so_customer = newCustomer,
                        status = true
                    });

                }

                UoWConsumer.CustomerRepository.Insert(newCustomer);

                //Generar el link para cancelar el envio de correo
                Guid id = Guid.NewGuid();
                var cancelEmail = new so_portal_links_log
                {
                    CustomerId = newCustomer.customerId,
                    CreatedDate = DateTime.Today,
                    Id = id,
                    LimitDays = 0,
                    Status = (int)PortalLinks.STATUS.PENDING,
                    Type = (int)PortalLinks.TYPE.EMAIL_DEACTIVATION
                };
                string cancelEmailURL = ConfigurationManager.AppSettings["ApiV2Url"] + "" + id;
                //Se envia el Correo

                //Se notifica al CMR

                UoWConsumer.CustomerAdditionalDataRepository.Insert(newCustomerAdditionalData);
                UoWConsumer.CustomerDataRepository.Insert(newCustomerData);
                UoWConsumer.RouteCustomerRepository.InsertByRange(newListRouteCustomer);
                UoWConsumer.Save();
                return ResponseBase<InsertConsumerResponse>.Create(new InsertConsumerResponse()
                {
                    Msg = "Se guardo con exito"
                });
            }
            catch (Exception e)
            {
                return ResponseBase<InsertConsumerResponse>
                    .Create(new List<string>() { e.Message });
            }
            
        }

        public ResponseBase<UpdateConsumerResponse> UpdateConsumer(UpdateConsumerRequest request)
        {
            try
            {
                var updateCustomer = UoWConsumer.CustomerRepository
                    .Get(x => x.customerId == request.CustomerId && x.status)
                    .FirstOrDefault();

                if (updateCustomer == null)
                    return ResponseBase<UpdateConsumerResponse>.Create(new List<string>()
                    {
                        "No se encontró al cliente"
                    });

                updateCustomer.name = request.Name ?? updateCustomer.name;
                updateCustomer.email = request.Email ?? updateCustomer.email;
                updateCustomer.latitude = request.Latitude ?? updateCustomer.latitude;
                updateCustomer.longitude = request.Longitude ?? updateCustomer.longitude;
                updateCustomer.modifiedon = DateTime.Now;
                updateCustomer.modifiedby = request.UserId;
                updateCustomer.code = request.CFECode ?? updateCustomer.code;
                updateCustomer.contact = request.Contact ?? updateCustomer.contact;

                var updateCustomerAdditionalData = updateCustomer.CustomerAdditionalData
                    .FirstOrDefault();

                updateCustomerAdditionalData.Email_2 = request.Email_2 ?? updateCustomerAdditionalData.Email_2;
                updateCustomerAdditionalData.Phone = request.Phone ?? updateCustomerAdditionalData.Phone;
                updateCustomerAdditionalData.Phone_2 = request.Phone_2 ?? updateCustomerAdditionalData.Phone_2;
                updateCustomerAdditionalData.CodePlace = request.CodePlace ?? updateCustomerAdditionalData.CodePlace;
                updateCustomerAdditionalData.ReferenceCode = request.ReferenceCode ?? updateCustomerAdditionalData.ReferenceCode;
                updateCustomerAdditionalData.InteriorNumber = request.InteriorNumber ?? updateCustomerAdditionalData.InteriorNumber;
                updateCustomerAdditionalData.Neighborhood = request.Neighborhood ?? updateCustomerAdditionalData.Neighborhood;

                if (!request.IsActive)
                    updateCustomerAdditionalData.Status = (int)Consumer.STATUS.DEACTIVATED;

                var updateCustomerData = updateCustomer.so_customer_data
                    .Where(x => x.status)
                    .FirstOrDefault();

                updateCustomerData.address_number = request.ExternalNumber ?? updateCustomerData.address_number;
                updateCustomerData.address_number_cross1 = request.Crossroads ?? updateCustomerData.address_number_cross1;
                updateCustomerData.address_number_cross2 = request.Crossroads_2 ?? updateCustomerData.address_number_cross2;
                updateCustomerData.address_street = request.Street ?? updateCustomerData.address_street;

                //Ágregar y eliminar dias
                var daysInRoute = UoWConsumer.RouteCustomerRepository
                    .Get(x => x.customerId == request.CustomerId && x.routeId == request.RouteId)
                    .ToList();

                var deleteDaysInRoute = daysInRoute
                    .Where(x => !request.Days.Contains(x.day))
                    .ToList();

                var newDaysInRoute = new List<so_route_customer>();

                foreach (var day in request.Days)
                {
                    if (daysInRoute.Where(x => x.day == day).FirstOrDefault() == null)
                        newDaysInRoute.Add(new so_route_customer
                        {
                            routeId = request.RouteId,
                            createdby = request.UserId,
                            createdon = DateTime.Now,
                            day = day,
                            order = 0,
                            customerId = request.CustomerId,
                            status = true
                        });
                }
                UoWConsumer.RouteCustomerRepository.InsertByRange(newDaysInRoute);
                UoWConsumer.RouteCustomerRepository.DeleteByRange(deleteDaysInRoute);
                UoWConsumer.CustomerRepository.Update(updateCustomer);
                UoWConsumer.CustomerDataRepository.Update(updateCustomerData);
                UoWConsumer.CustomerAdditionalDataRepository.Update(updateCustomerAdditionalData);

                UoWConsumer.Save();

                //Notificar al CRM

                return ResponseBase<UpdateConsumerResponse>.Create(new UpdateConsumerResponse()
                {
                    Msg = "Se ha actualizado con exito"
                });
            }
            catch (Exception e)
            {
                return ResponseBase<UpdateConsumerResponse>.Create(new List<string>()
                {
                    e.Message
                });
            }
        }

        public ResponseBase<ConsumerRemovalResponse> ConsumerRemovalRequestRequest(ConsumerRemovalRequestRequest request)
        {
            try
            {
                var customer = UoWConsumer.CustomerRepository
                    .Get(x => x.customerId == request.CustomerId && x.status)
                    .FirstOrDefault();

                if (customer == null)
                    return ResponseBase<ConsumerRemovalResponse>.Create(new List<string>()
                    {
                        "No se encontró al cliente"
                    });

                var id = Guid.NewGuid();
                var newCustomerRemovalRequest = new so_customer_removal_request
                {
                    Id = id,
                    UserId = request.UserId,
                    LimitDays = 0,
                    Date = DateTime.Today,
                    CustomerId = customer.customerId,
                    Reason = request.Reason,
                    Status = (int)ConsumerRemovalRequest.STATUS.PENDING
                };

                UoWConsumer.CustomerRemovalRequestRepository.Insert(newCustomerRemovalRequest);
                UoWConsumer.Save();

                return ResponseBase<ConsumerRemovalResponse>.Create(new ConsumerRemovalResponse()
                {
                    Msg = "Se ha hecho la petición con exito"
                });
            }
            catch (Exception e)
            {
                return ResponseBase<ConsumerRemovalResponse>.Create(new List<string>()
                {
                    e.Message
                });
            }
        }

        public ResponseBase<List<GetConsumersResponse>> GetConsumers(GetConsumersRequest request)
        {
            try
            {
                var soUser = UoWConsumer.UserRepository.Get(u => u.userId == request.userId).FirstOrDefault();
                int day = (int)DateTime.Today.DayOfWeek;
                day++;
                string daysWithoutSalesToDisableString = ConfigurationManager.AppSettings["DaysWithoutSalesToDisable"];
                int daysWithoutSalesToDisable = string.IsNullOrEmpty(daysWithoutSalesToDisableString) ? 3 : Convert.ToInt32(daysWithoutSalesToDisableString);


                InventoryService inventoryService = new InventoryService();
                try
                {
                    request.userId = inventoryService.SearchDrivingId(request.userId);
                }
                catch (RelatedDriverNotFoundException e)
                { }

                so_inventory inventory = inventoryService.getCurrentInventory(request.userId, null);

                List<GetConsumersResponse> visits = new List<GetConsumersResponse>();

                var customers = new List<int>();

                if (inventory != null)
                {
                    int inventoryId = inventory.inventoryId;

                    customers = UoWConsumer.DeliveryRepository
                        .Get(d => d.inventoryId.Equals(inventoryId) && d.status)
                        .Select(c => c.customerId)
                        .ToList();
                }

                var routeVisits = UoWConsumer.UserRouteRepository.GetAll()
                .Join(UoWConsumer.RouteCustomerRepository.GetAll(),
                    userRoute => userRoute.routeId,
                    customerRoute => customerRoute.routeId,
                    (userRoute, customerRoute) => new { userRoute.userId, customerRoute.customerId, customerRoute.day, customerRoute.order, customerRoute.status, userRouteStatus = userRoute.status, routeId = userRoute.routeId }
                )
                .Where(
                    v => v.userId.Equals(request.userId)
                    && v.userRouteStatus
                    && v.status
                    && day.Equals(v.day)
                ).Select(c => new { c.customerId, c.order, c.routeId });

                foreach (var data in routeVisits)
                {
                    int order = data.order;

                    var customerAdditionalDataAux = UoWConsumer.CustomerRepository
                        .Get(x => x.customerId == data.customerId)
                        .Select(x => x.CustomerAdditionalData)
                        .FirstOrDefault();

                    if (customerAdditionalDataAux == null || customerAdditionalDataAux.Count() == 0)
                    {
                        continue;
                    }
                    var customerAdditionalData = customerAdditionalDataAux.FirstOrDefault();
                    var customer = customerAdditionalData.Customer;
                    var customerData = customer.so_customer_data.FirstOrDefault();

                    if (inventory != null && inventory.status)
                    {
                        var delivery = UoWConsumer.DeliveryRepository
                            .Get(d => d.customerId == data.customerId && d.status && d.inventoryId == inventory.inventoryId)
                            .FirstOrDefault();
                        if (delivery != null && delivery.visit_order != null && delivery.visit_order != 0)
                        {
                            order = (int)delivery.visit_order;
                        }
                    }

                    List<int> daysInRoute = UoWConsumer.RouteCustomerRepository
                        .Get(x => x.routeId == data.routeId && x.status && x.customerId == customer.customerId)
                        .Select(x => x.day)
                        .ToList();

                    GetConsumersResponse dto = new GetConsumersResponse()
                    {
                        CustomerId = data.customerId,
                        Order = order,
                        Visited = UoWConsumer.BinnacleVisitRepository.Get(bv => bv.customerId == data.customerId &&
                            DbFunctions.TruncateTime(bv.createdon) == DbFunctions.TruncateTime(DateTime.Now))
                            .FirstOrDefault() != null,
                        Name = customer.name,
                        CFECode = customer.code,
                        CodePlace = customerAdditionalData.CodePlace,
                        Contact = customerAdditionalData.Customer.contact,
                        Crossroads = customerData != null ? customerData.address_number_cross1 : string.Empty,
                        Crossroads_2 = customerData != null ? customerData.address_number_cross2 : string.Empty,
                        Email = customer.email,
                        Email_2 = customerAdditionalData.Email_2,
                        ExternalNumber = customerData.address_number,
                        InteriorNumber = customerAdditionalData.InteriorNumber,
                        Latitude = customer.latitude,
                        Longitude = customer.longitude,
                        Neighborhood = customerAdditionalData.Neighborhood,
                        Phone = customerAdditionalData.Phone,
                        Phone_2 = customerAdditionalData.Phone_2,
                        ReferenceCode = customerAdditionalData.ReferenceCode,
                        RouteId = data.routeId,
                        Street = customerData.address_street,
                        Days = daysInRoute,
                        CounterVisitsWithoutSales = customerAdditionalData.CounterVisitsWithoutSales,
                        IsActive = customerAdditionalData.Status == (int)Consumer.STATUS.CONSUMER
                    };

                    visits.Add(dto);

                }

                return ResponseBase<List<GetConsumersResponse>>.Create(visits);
            }
            catch (Exception e)
            {
                return ResponseBase<List<GetConsumersResponse>>.Create( new List<string>()
                {
                    e.Message
                });
            }
        }

        public int SearchDrivingId(int actualUserId)
        {
            so_route_team teamRoute = UoWConsumer.RouteTeamRepository
                .Get(i => i.userId == actualUserId)
                .FirstOrDefault();

            if (teamRoute == null)
            {
                throw new RelatedDriverNotFoundException(actualUserId);
            }
            int DrivingId = UoWConsumer.RouteTeamRepository
                .Get(i => i.routeId == teamRoute.routeId && i.roleTeamId == (int)ERolTeam.Impulsor)
                .Select(x => x.userId)
                .FirstOrDefault();

            return DrivingId;
        }
        public void Dispose()
        {
            this.UoWConsumer.Dispose();
            this.UoWConsumer = null;
        }
    }
}