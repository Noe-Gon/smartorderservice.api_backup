﻿using Algoritmos.Data.Enums;
using Algoritmos.Data.UnitofWork;
using CRM.Data.UnitOfWork;
using RestSharp;
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
        private UoWCRM UoWCRM { get; set; }
        public UoWEmBeAlgoritmo UoWEmBeAlgoritmo { get; set; }

        public ConsumerService()
        {
            UoWConsumer = new UoWConsumer();
            UoWCRM = new UoWCRM();
            UoWEmBeAlgoritmo = new UoWEmBeAlgoritmo();

        }

        public ResponseBase<InsertConsumerResponse> InsertConsumer(InsertConsumerRequest request)
        {
            try
            {
                var route = UoWConsumer.RouteRepository
                    .GetByID(request.RouteId);

                if (route == null)
                    return ResponseBase<InsertConsumerResponse>
                    .Create(new List<string>() { "No se encontró la ruta" });

                if (request.CodePlace == 0)
                    request.CodePlace = null;

                if (request.CodePlace != null)
                {
                    var codePlace = UoWConsumer.CodePlaceRepository
                        .Get(x => x.Id == request.CodePlace && x.Status)
                        .FirstOrDefault();

                    if (codePlace == null)
                        return ResponseBase<InsertConsumerResponse>
                        .Create(new List<string>() { "No se encontró la ubicación del código" });
                }

                var existCustomer = UoWConsumer.CustomerRepository
                    .Get(x => x.code == request.CFECode && x.status)
                    .FirstOrDefault();

                if (existCustomer != null)
                    throw new DuplicateEntityException("Ya existe un consumidor con ese CFE");

                var newCustomer = new so_customer
                {
                    name = request.Name,
                    createdby = 2777,
                    createdon = DateTime.Now,
                    email = request.Email,
                    latitude = request.Latitude,
                    longitude = request.Longitude,
                    code = request.CFECode,
                    contact = request.Name,
                    status = true,
                };

                string address = "";
                address += string.IsNullOrEmpty(request.Street) ? "" : "C." + request.Street;
                address += string.IsNullOrEmpty(request.ExternalNumber) ? "" : " #" + request.ExternalNumber;
                address += string.IsNullOrEmpty(request.Crossroads) ? "" : " X " + request.Crossroads;
                address += string.IsNullOrEmpty(request.Crossroads_2) ? "" : " Y " + request.Crossroads_2;
                newCustomer.address = address;

                var newCustomerAdditionalData = new so_customer_additional_data
                {
                    Customer = newCustomer,
                    Phone = request.Phone,
                    Phone_2 = request.Phone_2,
                    Email_2 = request.Email_2,
                    Status = (int)Consumer.STATUS.CONSUMER,
                    AcceptedTermsAndConditions = false,
                    IsMailingActive = false,
                    IsSMSActive = false,
                    CodePlaceId = request.CodePlace,
                    CounterVisitsWithoutSales = 0,
                    InteriorNumber = request.InteriorNumber,
                    NeighborhoodId = request.Neighborhood,
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
                        status = true,
                        visit_type = 1
                    });

                }

                UoWConsumer.CustomerRepository.Insert(newCustomer);

                //Agregar el Price List
                var customerIds = UoWConsumer.RouteCustomerRepository
                    .Get(x => x.routeId == request.RouteId && x.so_customer.code.Length > 10)
                    .Select(x => x.customerId);

                if (customerIds.Count() == 0)
                    return ResponseBase<InsertConsumerResponse>.Create(new List<string>()
                    {
                        "No hay usuarios para esa ruta"
                    });

                var productPriceListId = UoWConsumer.CustomerProductPriceList
                    .Get(x => customerIds.Contains(x.customerId))
                    .Select(x => x.products_price_listId)
                    .FirstOrDefault();

                var newCustomerProductPriceList = new so_customer_products_price_list
                {
                    createdby = 2777,
                    createdon = DateTime.Now,
                    customerId = newCustomer.customerId,
                    modifiedby = 2777,
                    modifiedon = DateTime.Now,
                    products_price_listId = productPriceListId,
                    status = true
                };

                UoWConsumer.CustomerProductPriceList.Insert(newCustomerProductPriceList);

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
                string cancelEmailURL = ConfigurationManager.AppSettings["PortalUrl"] + "Consumer/CancelTicketDigital/" + id;

                //Generar Link de Aceptación de terminos y consiciones
                Guid termsId = Guid.NewGuid();
                var termsEmail = new so_portal_links_log
                {
                    CustomerId = newCustomer.customerId,
                    CreatedDate = DateTime.Today,
                    Id = termsId,
                    LimitDays = 0,
                    Status = (int)PortalLinks.STATUS.PENDING,
                    Type = (int)PortalLinks.TYPE.TERMSANDCONDITIONS_ACCEPT
                };
                string termsEmailURL = ConfigurationManager.AppSettings["PortalUrl"] + "Consumer/TermsAndConditions/" + termsId;

                //Se envia el Correo
                var emailService = new EmailService();

                emailService.SendWellcomeEmail(new WellcomeEmailRequest
                {
                    CustomerName = newCustomer.name,
                    TermsAndConditionLink = termsEmailURL,
                    CustomerEmail = newCustomer.email,
                    CanceledLink = cancelEmailURL
                });

                //Se notifica al CMR
                var CRMService = new CRMService();
                
                var crmRequest = new CRMConsumerRequest
                {
                    Name = newCustomer.name,
                    Email = newCustomer.email,
                    Phone = newCustomerAdditionalData.Phone,
                    CFECode = newCustomer.code,
                    CountryId = request.CountryId,
                    StateId = request.StateId,
                    MunicipalityId = request.MunicipalityId,
                    Neighborhood = newCustomerAdditionalData.NeighborhoodId,
                    InteriorNumber = newCustomerAdditionalData.InteriorNumber,
                    ExternalNumber = newCustomerData.address_number,
                    Crossroads = newCustomerData.address_number_cross1,
                    Crossroads_2 = newCustomerData.address_number_cross2,
                    Street = newCustomerData.address_street,
                    Latitude = newCustomer.latitude,
                    Longitude = newCustomer.longitude,
                    Address = address,
                    Days = request.Days,
                    EntityId = null
                };
                newCustomerAdditionalData.Code = CRMService.ConsumerToCRM(crmRequest, CRMService.TypeCreate, Method.POST);

                UoWConsumer.CustomerAdditionalDataRepository.Insert(newCustomerAdditionalData);
                UoWConsumer.CustomerDataRepository.Insert(newCustomerData);
                UoWConsumer.RouteCustomerRepository.InsertByRange(newListRouteCustomer);
                UoWConsumer.PortalLinksLogRepository.Insert(termsEmail);
                UoWConsumer.PortalLinksLogRepository.Insert(cancelEmail);
                UoWConsumer.Save();

                var response = new InsertConsumerResponse
                {
                    CustomerId = newCustomer.customerId,
                    CFECode = request.CFECode,
                    CodePlace = request.CodePlace,
                    CountryId = request.CountryId,
                    Crossroads = request.Crossroads,
                    Crossroads_2 = request.Crossroads_2,
                    Days = request.Days,
                    Email = request.Email,
                    Email_2 = request.Email_2,
                    ExternalNumber = request.ExternalNumber,
                    InteriorNumber = request.InteriorNumber,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    MunicipalityId = request.MunicipalityId,
                    Name = request.Name,
                    Neighborhood = request.Neighborhood,
                    Phone = request.Phone,
                    Phone_2 = request.Phone_2,
                    ReferenceCode = request.ReferenceCode,
                    RouteId = request.RouteId,
                    StateId = request.StateId,
                    Status = request.Status,
                    Street = request.Street,
                    UserId = request.UserId
                };

                return ResponseBase<InsertConsumerResponse>.Create(response);
            }
            catch (DuplicateEntityException e)
            {
                throw new DuplicateEntityException(e.Message);
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

                var existCustomer = UoWConsumer.CustomerRepository
                   .Get(x => x.code == request.CFECode && x.status)
                   .FirstOrDefault();

                if (existCustomer != null)
                    throw new DuplicateEntityException("Ya existe un consumidor con ese CFE");

                var route = UoWConsumer.RouteRepository
                    .GetByID(request.RouteId);

                if (route == null)
                    return ResponseBase<UpdateConsumerResponse>
                    .Create(new List<string>() { "No se encontró la ruta" });

                if (request.CodePlace == 0)
                    request.CodePlace = null;

                if (request.CodePlace != null)
                {
                    var codePlace = UoWConsumer.CodePlaceRepository
                        .Get(x => x.Id == request.CodePlace && x.Status)
                        .FirstOrDefault();

                    if (codePlace == null)
                        return ResponseBase<UpdateConsumerResponse>
                        .Create(new List<string>() { "No se encontró la ubicación del código" });
                }

                updateCustomer.name = request.Name ?? updateCustomer.name;
                updateCustomer.email = request.Email ?? updateCustomer.email;
                updateCustomer.latitude = request.Latitude ?? updateCustomer.latitude;
                updateCustomer.longitude = request.Longitude ?? updateCustomer.longitude;
                updateCustomer.modifiedon = DateTime.Now;
                updateCustomer.modifiedby = request.UserId;
                updateCustomer.code = request.CFECode ?? updateCustomer.code;
                updateCustomer.contact = request.Name ?? updateCustomer.name;

                var updateCustomerAdditionalData = updateCustomer.CustomerAdditionalData
                    .FirstOrDefault();
                var customerAdditionalDateAux = updateCustomerAdditionalData;

                if (updateCustomerAdditionalData == null)
                {
                    var newCustomerAdditionalData = new so_customer_additional_data
                    {
                        Customer = updateCustomer,
                        Phone = request.Phone,
                        Phone_2 = request.Phone_2,
                        Email_2 = request.Email_2,
                        Status = (int)Consumer.STATUS.CONSUMER,
                        AcceptedTermsAndConditions = false,
                        IsMailingActive = false,
                        IsSMSActive = false,
                        CodePlaceId = request.CodePlace,
                        CounterVisitsWithoutSales = 0,
                        InteriorNumber = request.InteriorNumber,
                        NeighborhoodId = request.Neighborhood,
                        ReferenceCode = request.ReferenceCode,
                        Code = null
                    };

                    if (!request.IsActive)
                        newCustomerAdditionalData.Status = (int)Consumer.STATUS.DEACTIVATED;

                    customerAdditionalDateAux = newCustomerAdditionalData;

                    UoWConsumer.CustomerAdditionalDataRepository.Insert(newCustomerAdditionalData);
                }
                else
                {
                    updateCustomerAdditionalData.Email_2 = request.Email_2 ?? updateCustomerAdditionalData.Email_2;
                    updateCustomerAdditionalData.Phone = request.Phone ?? updateCustomerAdditionalData.Phone;
                    updateCustomerAdditionalData.Phone_2 = request.Phone_2 ?? updateCustomerAdditionalData.Phone_2;
                    updateCustomerAdditionalData.CodePlaceId = request.CodePlace ?? updateCustomerAdditionalData.CodePlaceId;
                    updateCustomerAdditionalData.ReferenceCode = request.ReferenceCode ?? updateCustomerAdditionalData.ReferenceCode;
                    updateCustomerAdditionalData.InteriorNumber = request.InteriorNumber ?? updateCustomerAdditionalData.InteriorNumber;
                    updateCustomerAdditionalData.NeighborhoodId = request.Neighborhood ?? updateCustomerAdditionalData.NeighborhoodId;

                    if (!request.IsActive)
                        updateCustomerAdditionalData.Status = (int)Consumer.STATUS.DEACTIVATED;

                    UoWConsumer.CustomerAdditionalDataRepository.Update(updateCustomerAdditionalData);
                }
                

                var updateCustomerData = updateCustomer.so_customer_data
                    .Where(x => x.status)
                    .FirstOrDefault();

                if(updateCustomerData == null)
                {
                    var newCustomerDate = new so_customer_data
                    {
                        so_customer = updateCustomer,
                        route_code = Convert.ToInt32(route.code),
                        branch_code = Convert.ToInt32(route.so_branch.code),
                        address_number = request.ExternalNumber,
                        address_number_cross1 = request.Crossroads,
                        address_number_cross2 = request.Crossroads_2,
                        address_street = request.Street,
                        status = true
                    };
                    updateCustomerData = newCustomerDate;
                    UoWConsumer.CustomerDataRepository.Insert(newCustomerDate);
                }
                else
                {
                    updateCustomerData.address_number = request.ExternalNumber ?? updateCustomerData.address_number;
                    updateCustomerData.address_number_cross1 = request.Crossroads ?? updateCustomerData.address_number_cross1;
                    updateCustomerData.address_number_cross2 = request.Crossroads_2 ?? updateCustomerData.address_number_cross2;
                    updateCustomerData.address_street = request.Street ?? updateCustomerData.address_street;

                    UoWConsumer.CustomerDataRepository.Update(updateCustomerData);
                }
                

                string address = "";
                address += string.IsNullOrEmpty(request.Street) ? updateCustomerData.address_street : "C." + request.Street;
                address += string.IsNullOrEmpty(request.ExternalNumber) ? updateCustomerData.address_number : " #" + request.ExternalNumber;
                address += string.IsNullOrEmpty(request.Crossroads) ? updateCustomerData.address_number_cross1 : " X " + request.Crossroads;
                address += string.IsNullOrEmpty(request.Crossroads_2) ? updateCustomerData.address_number_cross2 : " Y " + request.Crossroads_2;
                updateCustomer.address = address;

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
                            status = true,
                            visit_type = 1
                        });
                }
                UoWConsumer.RouteCustomerRepository.InsertByRange(newDaysInRoute);
                UoWConsumer.RouteCustomerRepository.DeleteByRange(deleteDaysInRoute);
                UoWConsumer.CustomerRepository.Update(updateCustomer);

                UoWConsumer.Save();

                //Notificar al CRM
                var CRMService = new CRMService();
                var crmRequest = new CRMConsumerRequest
                {
                    Name = updateCustomer.name,
                    Email = updateCustomer.email,
                    Phone = customerAdditionalDateAux.Phone,
                    CFECode = updateCustomer.code,
                    CountryId = request.CountryId,
                    StateId = request.StateId,
                    MunicipalityId = request.MunicipalityId,
                    Neighborhood = customerAdditionalDateAux.NeighborhoodId,
                    InteriorNumber = customerAdditionalDateAux.InteriorNumber,
                    ExternalNumber = updateCustomerData.address_number,
                    Crossroads = updateCustomerData.address_number_cross1,
                    Crossroads_2 = updateCustomerData.address_number_cross2,
                    Street = updateCustomerData.address_street,
                    Latitude = updateCustomer.latitude,
                    Longitude = updateCustomer.longitude,
                    Address = address,
                    Days = request.Days,
                    EntityId = customerAdditionalDateAux.Code
                };

                if(customerAdditionalDateAux.Code != null)
                    CRMService.ConsumerToCRM(crmRequest, CRMService.TypeUpdate, Method.POST);

                return ResponseBase<UpdateConsumerResponse>.Create(new UpdateConsumerResponse()
                {
                    Msg = "Se ha actualizado con exito"
                });
            }
            catch (DuplicateEntityException e)
            {
                throw new DuplicateEntityException(e.Message);
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

                //Comprobar si exita una petición activa para ese usuario
                var previousRequestExist = UoWConsumer.CustomerRemovalRequestRepository
                    .Get(x => x.CustomerId == request.CustomerId && (x.Status == (int)ConsumerRemovalRequest.STATUS.PENDING))
                    .FirstOrDefault();

                if (previousRequestExist != null)
                    return ResponseBase<ConsumerRemovalResponse>.Create(new List<string>()
                    {
                        "Ya existe una petición para ese usuario"
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

                //Enviar correo al supervisor
                //Obtener a los leaderes
                try
                {
                    var info = UoWConsumer.UserRepository
                        .Get(x => x.userId == request.UserId)
                        .Select(x => new
                        {
                            name = x.name,
                            routeId = x.so_user_route.FirstOrDefault().so_route.name,
                            branchCode = x.so_user_route.FirstOrDefault().so_route.so_branch.code
                        })
                        .FirstOrDefault();

                    int branchCode = Convert.ToInt32(info.branchCode);
                    var leaderEmails = UoWEmBeAlgoritmo.UsuariosRepository
                            .Get(x => x.Estatus && x.IdRole == (int)UsuariosRole.CODES.LEADERCEDIS && !string.IsNullOrEmpty(x.Email) && x.IdCedis == branchCode)
                            .Select(x => x.Email)
                            .ToList();

                    //Generar Cuerpo del correo
                    var emailService = new EmailService();

                    var table = new List<SendRemovalRequestTable>();
                    table.Add(new SendRemovalRequestTable
                    {
                        ConsumerName = customer.name,
                        ImpulsorName = info.name,
                        CFECode = customer.code,
                        Date = DateTime.Now,
                        Reason = request.Reason,
                        Route = info.routeId
                    });

                    emailService.SendRemovalRequestEmail(new SendRemovalRequestEmailRequest
                    {
                        LeaderEmail = leaderEmails,
                        Table = table
                    });
                }
                catch (Exception)
                {
                }

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
                catch (RelatedDriverNotFoundException)
                { }

                so_inventory inventory = inventoryService.GetCurrentInventory(request.userId, null);

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
                    (userRoute, customerRoute) => new { userRoute.userId, customerRoute.customerId, customerRoute.so_customer, customerRoute.day, customerRoute.order, customerRoute.status, userRouteStatus = userRoute.status, routeId = userRoute.routeId, HasAdditionalData = customerRoute.so_customer.CustomerAdditionalData.Count() != 0 }
                )
                .Where(
                    v => v.userId.Equals(request.userId)
                    && v.userRouteStatus
                    && v.status
                    && day.Equals(v.day)
                    //&& v.HasAdditionalData
                ).Select(c => new { c.customerId, c.order, c.routeId, c.so_customer });

                foreach (var data in routeVisits)
                {
                    int order = data.order;

                    var customerAdditionalDataAux = UoWConsumer.CustomerRepository
                        .Get(x => x.customerId == data.customerId)
                        .Select(x => x.CustomerAdditionalData)
                        .FirstOrDefault();

                    var customerAdditionalData = customerAdditionalDataAux.FirstOrDefault();
                    var customer = data.so_customer;
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
                        CodePlace = customerAdditionalData == null ? null : customerAdditionalData.CodePlaceId,
                        Contact = customerAdditionalData == null ? string.Empty : customerAdditionalData.Customer.contact,
                        Crossroads = customerData != null ? customerData.address_number_cross1 : string.Empty,
                        Crossroads_2 = customerData != null ? customerData.address_number_cross2 : string.Empty,
                        Email = customer.email,
                        Email_2 = customerAdditionalData == null ? string.Empty : customerAdditionalData.Email_2,
                        ExternalNumber = customerData != null ? customerData.address_number : string.Empty,
                        InteriorNumber = customerAdditionalData == null ? string.Empty : customerAdditionalData.InteriorNumber,
                        Latitude = customer.latitude,
                        Longitude = customer.longitude,
                        Neighborhood = customerAdditionalData == null ? null : customerAdditionalData.NeighborhoodId,
                        Phone = customerAdditionalData == null ? string.Empty : customerAdditionalData.Phone,
                        Phone_2 = customerAdditionalData == null ? string.Empty : customerAdditionalData.Phone_2,
                        ReferenceCode = customerAdditionalData == null ? string.Empty : customerAdditionalData.ReferenceCode,
                        RouteId = data.routeId,
                        Street = customerData != null ? customerData.address_street : string.Empty,
                        Days = daysInRoute,
                        CounterVisitsWithoutSales = customerAdditionalData == null ? 0 : customerAdditionalData.CounterVisitsWithoutSales,
                        IsActive = customerAdditionalData == null ? false : customerAdditionalData.Status == (int)Consumer.STATUS.CONSUMER,
                        IsMailingActive = customerAdditionalData == null ? false : customerAdditionalData.IsMailingActive,
                        IsSMSActive = customerAdditionalData == null ? false : customerAdditionalData.IsSMSActive,
                        IsTermsAndConditionsAccepted = customerAdditionalData == null ? false : customerAdditionalData.AcceptedTermsAndConditions,
                        CanBeRemoved = customerAdditionalData == null ? false : customerAdditionalData.CounterVisitsWithoutSales >= daysWithoutSalesToDisable
                    };

                    if (customerAdditionalData == null ? false : customerAdditionalData.NeighborhoodId != null)
                    {
                        var ubication = UoWCRM.ColoniasRepository
                            .Get(x => x.Ope_coloniaId == customerAdditionalData.NeighborhoodId)
                            .Select(x => new
                            {
                                CountryId = x.ope_PaisId,
                                StateId = x.ope_EstadoId,
                                TownId = x.Ope_MunicipioId
                            }).FirstOrDefault();

                        dto.StateId = ubication.StateId;
                        dto.CountryId = ubication.CountryId;
                        dto.TownId = ubication.TownId;
                    }

                    visits.Add(dto);

                }

                return ResponseBase<List<GetConsumersResponse>>.Create(visits);
            }
            catch (Exception e)
            {
                return ResponseBase<List<GetConsumersResponse>>.Create(new List<string>()
                {
                    e.Message
                });
            }
        }

        public ResponseBase<ResendTicketDigitalResponse> ResendTicketDigital(ResendTicketDigitalRequest request)
        {
            try
            {
                //Obtención del sale
                var sale = UoWConsumer.SaleRepository
                    .GetByID(request.SaleId);

                if (sale == null)
                    return ResponseBase<ResendTicketDigitalResponse>.Create(new List<string>()
                    {
                        "No se encontró una venta con ese id"
                    });

                if (sale.so_customer.CustomerAdditionalData == null)
                    return ResponseBase<ResendTicketDigitalResponse>.Create(new List<string>()
                    {
                        "No cuenta con la información necesaria para enviar el Ticket"
                    });

                var route = UoWConsumer.RouteCustomerRepository
                    .Get(x => x.customerId == sale.customerId)
                    .FirstOrDefault();

                var sendTicketDigitalEmail = new SendTicketDigitalEmailRequest
                {
                    CustomerName = sale.so_customer.name,
                    RouteAddress = Convert.ToString(route.routeId),
                    CustomerEmail = sale.so_customer.email,
                    CustomerFullName = sale.customerId + " - " + sale.so_customer.name + " " + sale.so_customer.address,
                    Date = sale.date,
                    PaymentMethod = request.PaymentMethod,
                    SellerName = sale.so_user.code + " - " + sale.so_user.name
                };

                var sales = new List<SendTicketDigitalEmailSales>();
                foreach (var detail in sale.so_sale_detail)
                {
                    sales.Add(new SendTicketDigitalEmailSales
                    {
                        Amount = detail.amount,
                        ProductName = detail.so_product.productId + " - " + detail.so_product.name,
                        TotalPrice = Convert.ToDouble(detail.amount) * Convert.ToDouble(detail.sale_price),
                        UnitPrice = Convert.ToDouble(detail.sale_price)
                    });
                }
                sendTicketDigitalEmail.Sales = sales;

                var emailService = new EmailService();
                var response = emailService.SendTicketDigitalEmail(sendTicketDigitalEmail);

                return ResponseBase<ResendTicketDigitalResponse>.Create(new ResendTicketDigitalResponse
                {
                    Msg = "Se ha enviado el correo"
                });
            }
            catch (Exception e)
            {
                return ResponseBase<ResendTicketDigitalResponse>.Create(new List<string>()
                {
                    e.Message
                });
            }
        }

        public ResponseBase<ReactivationTicketDigitalResponse> ReactivationTicketDigital(ReactivationTicketDigitalRequest request)
        {
            try
            {
                var customer = UoWConsumer.CustomerRepository
                    .GetByID(request.CustomerId);

                if (customer == null)
                    return ResponseBase<ReactivationTicketDigitalResponse>.Create(new List<string>()
                    {
                        "No se encontró al cliente"
                    });

                if (string.IsNullOrEmpty(request.CustomerEmail))
                    request.CustomerEmail = customer.email;

                if (string.IsNullOrEmpty(request.CustomerEmail))
                    return ResponseBase<ReactivationTicketDigitalResponse>.Create(new List<string>()
                    {
                        "No se encontró ningun email para el envio"
                    });

                //TermsAndConditios
                //Verificar si el cliente cuenta con uno activo
                var termsObject = UoWConsumer.PortalLinksLogRepository
                    .Get(x => x.CustomerId == request.CustomerId && x.Type == (int)PortalLinks.TYPE.TERMSANDCONDITIONS_ACCEPT && x.Status == (int)PortalLinks.STATUS.PENDING)
                    .FirstOrDefault();

                var emailInfo = new SendReactivationTicketDigitalRequest() {
                    CustomerEmail = request.CustomerEmail,
                    CustomerName = customer.name
                };

                if (termsObject == null)
                {
                    //Generar Link de Aceptación de terminos y consiciones
                    Guid termsId = Guid.NewGuid();
                    var termsEmail = new so_portal_links_log
                    {
                        CustomerId = customer.customerId,
                        CreatedDate = DateTime.Today,
                        Id = termsId,
                        LimitDays = 0,
                        Status = (int)PortalLinks.STATUS.PENDING,
                        Type = (int)PortalLinks.TYPE.TERMSANDCONDITIONS_ACCEPT
                    };
                    UoWConsumer.PortalLinksLogRepository.Insert(termsEmail);
                    UoWConsumer.Save();
                    emailInfo.TermsAndConditionLink = ConfigurationManager.AppSettings["PortalUrl"] + "Consumer/TermsAndConditions/" + termsId;

                }
                else
                    emailInfo.TermsAndConditionLink = ConfigurationManager.AppSettings["PortalUrl"] + "Consumer/TermsAndConditions/" + termsObject.Id;

                var emailService = new EmailService();
                var response = emailService.SendReactivationTicketDigital(emailInfo);

                if (response.Status)
                    return ResponseBase<ReactivationTicketDigitalResponse>.Create(new ReactivationTicketDigitalResponse
                    {
                        Msg = response.Data.Msg
                    });

                return ResponseBase<ReactivationTicketDigitalResponse>.Create(response.Errors);
            }
            catch (Exception e)
            {
                return ResponseBase<ReactivationTicketDigitalResponse>.Create(new List<string>()
                {
                    e.Message
                });
            }
        }

        public ResponseBase<List<GetCountriesResponse>> GetCountries()
        {
            try
            {
                var countries = UoWCRM.PaisesRepository
                    .Get(x => x.statecode == 0)
                    .OrderBy(x => x.Ope_name)
                    .Select(x => new GetCountriesResponse
                    {
                        Id = x.Ope_paisId,
                        Name = x.Ope_name
                    }).ToList();

                return ResponseBase<List<GetCountriesResponse>>.Create(countries);
            }
            catch (Exception e)
            {
                return ResponseBase<List<GetCountriesResponse>>.Create(new List<string>()
                {
                    e.Message
                });
            }
        }

        public ResponseBase<List<GetStatesResponse>> GetStates(GetStatesRequest request)
        {
            if (request == null)
                return ResponseBase<List<GetStatesResponse>>.Create(new List<string>()
                    {
                        "Es necesario proporcionar el Id del pais"
                    });

            var states = UoWCRM.EstadosRepository
            .Get(x => x.statecode == 0 && x.Ope_PaisId == request.CountryId)
            .OrderBy(x => x.Ope_name)
            .Select(x => new GetStatesResponse
            {
                Id = x.Ope_estadoId,
                Name = x.Ope_name
            }).ToList();

            return ResponseBase<List<GetStatesResponse>>.Create(states);
        }

        public ResponseBase<List<GetMunicipalitiesResponse>> GetMunicipalities(GetMunicipalitiesRequest request)
        {
            if (request == null)
                return ResponseBase<List<GetMunicipalitiesResponse>>.Create(new List<string>()
                {
                    "Este servicio require parametros"
                });

            if (request.StateId == null)
                return ResponseBase<List<GetMunicipalitiesResponse>>.Create(new List<string>()
                {
                    "Es necesario proporcionar el Id del Estado"
                });

            var towns = UoWCRM.MunicipiosRepository
                .Get(x => x.statecode == 0 && x.ope_PaisId == request.CountryId && x.Ope_EstadoId == request.StateId)
                .OrderBy(x => x.Ope_name)
                .Select(x => new GetMunicipalitiesResponse
                {
                    Id = x.Ope_municipioId,
                    Name = x.Ope_name
                }).ToList();

            return ResponseBase<List<GetMunicipalitiesResponse>>.Create(towns);
        }

        public ResponseBase<List<GetNeighborhoodsResponse>> GetNeighborhoods(GetNeighborhoodsRequest request)
        {
            if (request == null)
                return ResponseBase<List<GetNeighborhoodsResponse>>.Create(new List<string>()
                {
                    "Este servicio require parametros"
                });

            if (request.StateId == null)
                return ResponseBase<List<GetNeighborhoodsResponse>>.Create(new List<string>()
                {
                    "Es necesario proporcionar el Id del Estado"
                });

            if (request.MunicipalityId == null)
                return ResponseBase<List<GetNeighborhoodsResponse>>.Create(new List<string>()
                {
                    "Es necesario proporcionar el Id del Municipio"
                });

            var colonias = UoWCRM.ColoniasRepository
                .Get(x => x.statecode == 0 && x.ope_PaisId == request.CountryId && x.ope_EstadoId == request.StateId && x.Ope_MunicipioId == request.MunicipalityId)
                .OrderBy(x => x.Ope_name)
                .Select(x => new GetNeighborhoodsResponse
                {
                    Id = x.Ope_coloniaId,
                    Name = x.Ope_name
                })
                .ToList();

            return ResponseBase<List<GetNeighborhoodsResponse>>.Create(colonias);
        }

        public ResponseBase<RemoveConsumerResponse> RemoveConsumer(RemoveConsumerRequest request)
        {
            try
            {
                var customer = UoWConsumer.CustomerRepository
                    .Get(x => x.customerId == request.CustomerId && x.status)
                    .FirstOrDefault();

                if (customer == null)
                    return ResponseBase<RemoveConsumerResponse>.Create(new List<string>()
                    {
                        "No se encontró al cliente"
                    });

                var customerAdditionalData = customer.CustomerAdditionalData
                    .FirstOrDefault();

                //Notificar al CRM
                if(customerAdditionalData.Code != null)
                {
                    var CRMService = new CRMService();
                    var crmRequest = new CRMConsumerRequest
                    {
                        CFECode = null,
                        EntityId = customerAdditionalData.Code
                    };
                    CRMService.ConsumerToCRM(crmRequest, CRMService.TypeUpdate, Method.POST);
                }

                customer.status = false;
                UoWConsumer.CustomerRepository.Update(customer);
                UoWConsumer.Save();

                return ResponseBase<RemoveConsumerResponse>.Create(new RemoveConsumerResponse
                {
                    Msg = "Se elimino con exito"
                });
            }
            catch (Exception e)
            {
                return ResponseBase<RemoveConsumerResponse>.Create(new List<string>()
                {
                    e.Message
                });
            }
        }

        public ResponseBase<GetCustomerVarioResponse> GetCustomerVario(GetCustomerVarioRequest request)
        {
            int? customerId = UoWConsumer.RouteCustomerVarioRepository
                .Get(x => x.RouteId == request.RouteId && x.Status)
                .Select(x => x.CustomerId)
                .FirstOrDefault();

            if (customerId == null)
                return ResponseBase<GetCustomerVarioResponse>.Create(new List<string>()
                {
                    "La ruta no cuenta con cliente vario o este ha sido eliminado de la ruta"
                });

            var response = UoWConsumer.CustomerRepository
                .Get(x => x.customerId == customerId)
                .Select(x => new GetCustomerVarioResponse
                {
                    Address = x.address,
                    Code = x.code,
                    Contact = x.contact,
                    CustomerId = x.customerId,
                    Description = x.description,
                    Email = x.email,
                    Latitude = x.latitude ?? 0,
                    Longitude = x.longitude ?? 0,
                    Name = x.name,
                    Status = x.status,
                    Tags = x.so_tag.Select(t => t.tag).ToList(),
                    
                })
                .FirstOrDefault();

            if (response == null)
                return ResponseBase<GetCustomerVarioResponse>.Create(new List<string>()
                {
                    "Cliente no encontrado"
                });

            return ResponseBase<GetCustomerVarioResponse>.Create(response);
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