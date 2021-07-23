using AutoMapper;
using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.DB.OpeCdBi;
using SmartOrderService.Models;
using SmartOrderService.Models.DTO;
using SmartOrderService.Models.Enum;
using SmartOrderService.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace SmartOrderService.Services
{
    public class CustomerService
    {

        private SmartOrderModel db = new SmartOrderModel();

        public List<CustomerDto> getAll(DateTime updated,int UserId) {

            List<CustomerDto> customers = new List<CustomerDto>();
            InventoryService inventoryService = new InventoryService();

            var UserRoute = db.so_user_route.Where(u => u.userId.Equals(UserId) && u.status).FirstOrDefault();
            var datas = db.so_route_customer.Where(
                c => c.routeId.Equals(UserRoute.routeId) 
                && c.status
                && c.visit_type > 0)
                .Select(c => c.so_customer)
                .Distinct()
                .ToList();

            try
            {
                UserId = inventoryService.SearchDrivingId(UserId);
            }
            catch (RelatedDriverNotFoundException e)
            {
            }

            var Inventory = inventoryService.getCurrentInventory(UserId,null);

            var CustomerToDeliver = new DeliveryService().getCustomersToDeliver(Inventory.inventoryId, UserId);

            var customerIds = datas.Select(c => c.customerId).ToList();

            CustomerToDeliver.RemoveAll(c => customerIds.Contains(c.customerId));

            datas.AddRange(CustomerToDeliver);
            
            if (datas.Any())
            {                
                var customerList = datas.Select(c => c.customerId).ToList();

                var tags = db.so_tag.Where(t => customerList.Contains(t.customerId) && t.status);

                customers = Mapper.Map<List<CustomerDto>>(datas);

                foreach(var customer in customers)
                {
                    var customerTags = tags.Where(t => t.customerId == customer.CustomerId).Select(t=> t.tag).ToList();

                    customer.Tags.AddRange(customerTags);
                    customer.IsFacturable = true;// facturables.Contains(customer.CustomerId);
                    
                }

            }

            return customers;
        }

        public List<CustomerDto> FindCustomers(CustomerRequest request)
        {

            var customers = new List<CustomerDto>();

            if (request.Code != null && request.Code.Length > 0)
            {
                var customer = FindCustomerByCode(request.Code);
                customers.Add(customer);
            }
            else
            {
                var date = Utils.DateUtils.getDateTime(request.LastUpdate);
                customers.AddRange(getAll(date, request.UserId));
            }

            return customers;
        }

        public CustomerVisitDto CreateVisit(CustomerVisitDto dto, int? CustomerId)
        {

            if (CustomerId == null)
                throw new CustomerException();

            var visit = Mapper.Map<so_binnacle_visit>(dto);

            visit.createdby = dto.UserId;
            visit.createdon = DateTime.Now;
            visit.modifiedby = dto.UserId;
            visit.modifiedon = DateTime.Now;
            visit.userId = (int)dto.UserId;
            visit.is_visit = true;
            visit.status = true;
            


            using (var dbContextTransaction = db.Database.BeginTransaction())
            {

                db.so_binnacle_visit.Add(visit);

                db.SaveChanges();

                dto.VisitId = visit.binnacleId;

                var ItemToDownload = new ControlDownloadService().createControlDownload(visit.binnacleId, (int)dto.UserId, ControlDownloadService.MODEL_TYPE_BINNACLE_VISIT);

                db.so_control_download.Add(ItemToDownload);

                db.SaveChanges();

                dbContextTransaction.Commit();

            }

            return dto;
        }

        public CustomerVisitDto CreateVisit(CustomerVisitDto dto, int UserId)
        {
            var visit = Mapper.Map<so_binnacle_visit>(dto);

            visit.createdby = UserId;
            visit.createdon = DateTime.Now;
            visit.modifiedby = UserId;
            visit.modifiedon = DateTime.Now;
            visit.userId = UserId;
            visit.is_visit = true;
            visit.status = true;


            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
               
                    db.so_binnacle_visit.Add(visit);

                    db.SaveChanges();

                    dto.VisitId = visit.binnacleId;

                    var ItemToDownload = new ControlDownloadService().createControlDownload(visit.binnacleId, UserId, ControlDownloadService.MODEL_TYPE_BINNACLE_VISIT);

                    db.so_control_download.Add(ItemToDownload);

                    db.SaveChanges();

                    dbContextTransaction.Commit();
              
            }

            return dto;
        }

        public CustomerVisitDto CreateTeamVisit(CustomerVisitDto dto, int UserId)
        {
            RoleTeamService roleTeamService = new RoleTeamService();
            RouteTeamService routeTeamService = new RouteTeamService();
            InventoryService inventoryService = new InventoryService();

            var visit = Mapper.Map<so_binnacle_visit>(dto);

            visit.createdby = UserId;
            visit.createdon = DateTime.Now;
            visit.modifiedby = UserId;
            visit.modifiedon = DateTime.Now;
            visit.userId = UserId;
            visit.is_visit = true;
            visit.status = true;


            using (var dbContextTransaction = db.Database.BeginTransaction())
            {

                db.so_binnacle_visit.Add(visit);

                db.SaveChanges();

                dto.VisitId = visit.binnacleId;

                var ItemToDownload = new ControlDownloadService().createControlDownload(visit.binnacleId, UserId, ControlDownloadService.MODEL_TYPE_BINNACLE_VISIT);

                db.so_control_download.Add(ItemToDownload);
                //Aumentar el contador de visitas sin ventas 
                var updateCustomerAdditionalData = db.so_customerr_additional_data
                            .Where(x => x.CustomerId == dto.CustomerId)
                            .FirstOrDefault();
                if (updateCustomerAdditionalData != null)
                    updateCustomerAdditionalData.CounterVisitsWithoutSales ++;

                db.SaveChanges();

                //Si es de un quipo hacer el guardado de so_reoute_team
                ERolTeam userRole = roleTeamService.getUserRole(visit.userId);
                if (userRole == ERolTeam.Ayudante || userRole == ERolTeam.Impulsor)
                {
                    var impulsorId = routeTeamService.SearchDrivingId(visit.userId);
                    var inventory = inventoryService.getCurrentInventory(impulsorId, DateTime.Today);
                    var routeId = routeTeamService.searchRouteId(visit.userId);
                    var workDay = routeTeamService.GetWorkdayByUserAndDate(impulsorId, DateTime.Today);

                    db.so_route_team_travels_visits.Add(new so_route_team_travels_visit
                    {
                        binnacleId = visit.binnacleId,
                        inventoryId = inventory.inventoryId,
                        routeId = routeId,
                        workDayId = workDay.work_dayId
                    });
                    db.SaveChanges();
                }

                dbContextTransaction.Commit();

            }

            return dto;
        }

        internal List<CustomerDataDto> getDataByRoute(int RouteId)
        {
            List<CustomerDataDto> customers = new List<CustomerDataDto>();

            var customerData = db.so_route
                .Join(db.so_route_customer, r => r.routeId, rc => rc.routeId, (r, rc) => new { r, rc })
                .Where(x => x.r.routeId == RouteId & x.r.status & x.rc.status)
                .Select(x => x.rc)
                .Join(db.so_customer, rc => rc.customerId, c => c.customerId, (rc, c) => new { c })
                .Where(y => y.c.status)
                .Select(y => y.c).Distinct()
                .Join(db.so_customer_data, c => c.customerId, cd => cd.customerId, (c, cd) => new { c, cd });

            foreach (var c in customerData)
                customers.Add(new CustomerDataDto { Id = c.c.customerId, Code = c.c.code, Name = c.c.name, Ftr = c.cd.ftr, BusinessName = c.cd.business_name, FiscalAddress = c.cd.fiscal_address, Status = c.cd.status });
            return customers;
        }

        internal void SetStatus(int CustomerId, CustomerDataDto Customer)
        {
            var customerData = db.so_customer_data.FirstOrDefault(c => c.customerId == CustomerId);
            if (customerData == null)
                throw new Exception("El cliente no existe");
            customerData.status = Customer.Status;
            customerData.modifiedon = DateTime.Now;
            db.SaveChanges();

        }

        public void SetUpBilling(int CustomerId, int BranchCode, int RouteCode, bool Enabled)
        {
            var CustomerData = db.so_customer_data.Where(c => c.customerId.Equals(CustomerId)
            && c.branch_code.Equals(BranchCode)
            && c.route_code.Equals(RouteCode)
            ).FirstOrDefault();

            if (CustomerData == null)
                throw new CustomerBillingDataNotFoundException();

            CustomerData.status = Enabled;

            db.SaveChanges();
        }

        public CustomerDto FindCustomerByCode(string CustomerCode)
        {
            var customer = db.so_customer.Where(c => c.code.Equals(CustomerCode)).FirstOrDefault();

            if (customer == null)
                throw new CustomerNotFoundException(CustomerCode);

            var CustomerDto = Mapper.Map<CustomerDto>(customer);

            return CustomerDto;
        }

        public List<InvoiceDataDto> FindCustomerInvoiceData(int customerId, string routeCode, string branchCode)
        {
            Dictionary<string, InvoiceDataDto> datas = new Dictionary<string, InvoiceDataDto>();

            var customer = db.so_customer.Where(c => c.customerId == customerId).FirstOrDefault();

            if (customer == null)
                throw new CustomerException("No se encontro al cliente");

            if (!customer.status)
                throw new CustomerException("El cliente " + customer.name + " esta desactivado en WBC");

            OpeCdBiModel opecdbi = new OpeCdBiModel();

            decimal customerCode = Convert.ToDecimal(customer.code);

            var query = opecdbi.Birutasupreg.Join(opecdbi.Biregistros,
                    brs => brs.idregistro,
                    br => br.idregistro,
                    (brs, br) => new { brs, br })
                    .Join(opecdbi.Birutascli,
                    entity => new { entity.brs.idrutacli, entity.brs.idcliente },
                    brc => new { brc.idrutacli, brc.idcliente },
                    (entity, brc) => new { entity, brc })
                    .Join(opecdbi.Birutas,
                    entity => entity.brc.idbiruta,
                    bru => bru.idbiruta,
                    (entity, bru) => new { entity, bru })

                    .GroupJoin(opecdbi.Biusocomprobantes,
                    entity => entity.entity.entity.brs.idbiusocomprobante,
                    buc => buc.idbiusocomprobante,
                    (entity, buc) =>  new { entity, buc }).Select(x => new { x.entity, x.buc }).DefaultIfEmpty()
                    .Join(opecdbi.AxRefSitios,
                    entity => entity.entity.bru.idcedis,
                    ars => ars.idcedis,
                    (entity, ars) => new { entity, ars });
                    /*.Join(opecdbi.BiRutasTipos,
                    entity => entity.entity.entity.bru.idbiruta,
                    brt => brt.idbiruta,
                    (entity, brt) => new { entity, brt });*/

            if(string.IsNullOrEmpty(routeCode) && string.IsNullOrEmpty(branchCode))
            {
                query = query.Where(entity => entity.entity.entity.entity.entity.brs.idcliente == customerCode &&
                                     entity.entity.entity.entity.entity.brs.activo);
            }
            else if(!string.IsNullOrEmpty(routeCode) && string.IsNullOrEmpty(branchCode))
            {
                var route = Convert.ToInt32(routeCode);
                var biruta = opecdbi.Birutas.Join(opecdbi.BiRutasTipos,
                    br => br.idbiruta,
                    brt => brt.idbiruta,
                    (br, brt) => new { br, brt})
                    .Where(br => br.br.idruta == route).OrderBy(br => br.brt.idtipoprod).Select(brt => brt).ToList();
                if (biruta == null || biruta.Count == 0)
                {
                    throw new RouteNotFoundException("No hay rutas con el codigo " + routeCode +" en bi");
                }
                else if(biruta.Select(brt => brt.brt.activo).ToList().Count == 0)
                {
                    throw new RouteNotFoundException("No hay ruta " + routeCode + " activa en bi");
                }
                query = query.Where(entity => entity.entity.entity.entity.entity.brs.idcliente == customerCode &&
                                     entity.entity.entity.entity.entity.brs.activo &&
                                     biruta.Select(brt => brt.brt.idtipoprod).Contains(entity.entity.entity.entity.entity.brs.idtipoprod));
            }
            else
            {
                var route = Convert.ToInt32(routeCode);
                var branch = Convert.ToInt32(branchCode);
               
                var biruta = opecdbi.Birutas.Join(opecdbi.BiRutasTipos,
                    br => br.idbiruta,
                    brt => brt.idbiruta,
                    (br, brt) => new { br, brt })
                    .Where(br => br.br.idruta == route && br.br.idcedis == branch && br.brt.activo).OrderBy(br => br.brt.idtipoprod).Select(brt => brt.brt.idtipoprod).ToList();

                if (biruta == null || biruta.Count == 0)
                {
                    throw new RouteNotFoundException("La ruta " + routeCode + " no existe o esta desactivada en el cedis " + branchCode + " en bi");
                }
                query = query.Where(entity => entity.entity.entity.entity.entity.brs.idcliente == customerCode &&
                                     entity.entity.entity.entity.entity.brs.activo &&
                                     biruta.Contains(entity.entity.entity.entity.entity.brs.idtipoprod));
            }

            query = query.OrderByDescending(entity => entity.entity.entity.entity.brc.credito);

            var registros = query
                    .Select(sel => new {
                        codope = sel.entity.entity.entity.entity.brs.codope,
                        cuc = sel.entity.entity.entity.entity.brs.idcliente,
                        idsupervisorcli = sel.entity.entity.entity.entity.brs.idsupervisorcli,
                        factwbc = sel.entity.entity.entity.entity.brs.factwbc,
                        registro = sel.entity.entity.entity.entity.br.registro,
                        razonsoc = sel.entity.entity.entity.entity.br.razonsoc,
                        ruta = sel.entity.entity.bru.idruta,
                        cedis = sel.entity.entity.bru.idcedis,
                        usocomprobante = sel.entity.buc.FirstOrDefault().idusocomprobante == null ? "" : sel.entity.buc.FirstOrDefault().idusocomprobante,
                        descripcioncomprobante = sel.entity.buc.FirstOrDefault().descripcion == null ? "": sel.entity.buc.FirstOrDefault().descripcion,
                        direccionfiscal = sel.entity.entity.entity.entity.br.direccion,
                        credito = sel.entity.entity.entity.brc.credito,
                        siteid = sel.ars.sitio,
                        inventsiteid = sel.ars.idcedis,
                        salesresponsible = sel.entity.entity.entity.entity.brs.idsupervisorcli
                    })
                    
                .ToList();
            


            foreach (var registro in registros)
            {
                if (!datas.ContainsKey(registro.registro))
                {
                    InvoiceDataDto data = new InvoiceDataDto();
                    data.BranchCode = Convert.ToString(registro.cedis);
                    data.BussinessName = registro.razonsoc;
                    data.CustomerRFC = registro.registro;
                    data.DescriptionUseCFDI = registro.descripcioncomprobante;
                    data.IsBillableByWBC = registro.factwbc > 0;
                    data.RouteCode = Convert.ToString(registro.ruta);
                    data.UseCFDI = registro.usocomprobante;
                    data.Codope = Convert.ToString(registro.codope);
                    data.Address = registro.direccionfiscal;
                    data.CreditApply = registro.credito;
                    data.SiteId = registro.siteid;
                    data.InventSiteId = registro.inventsiteid.ToString();
                    data.SalesResponsible = ((int) registro.salesresponsible).ToString();
                    datas.Add(registro.registro, data);

                }

            }

            var list = datas.Values.ToList();

            var creditAmount = list.Where(d => d.CreditApply).ToList().Count();
            var payAmount = list.Where(d => !d.CreditApply).ToList().Count();

            if(creditAmount != 0 && payAmount == 0)
            {
                return list;
            }else if(creditAmount == 0 && payAmount != 0)
            {
                return list;
            }else if(creditAmount != 0 && payAmount != 0)
            {
                return list.Where(d => d.CreditApply).ToList();
            }

            return datas.Values.ToList();
        }
    }
}