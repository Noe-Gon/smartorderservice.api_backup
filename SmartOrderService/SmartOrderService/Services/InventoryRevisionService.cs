using AutoMapper;
using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class InventoryRevisionService
    {

        private SmartOrderModel db = new SmartOrderModel();


        public RevisionDto CreateRevision(int RouteId,int InventoryId,int RevisionType,DateTime Date,int UserId)
        {
            var existRevision = db.so_inventory_revisions
                .Where(r => r.so_revision_states.value.Equals("1") 
                && r.status
                && r.routeId.Equals(RouteId)
                && r.inventoryId.Equals(InventoryId)
                )
                .Count()>0;

            //agregar si ya se visito a todos los clientes

            var currentInventory = db.so_inventory.Where(i => i.inventoryId.Equals(InventoryId)).FirstOrDefault();

            var inventoryService = new InventoryService();

            if (!inventoryService.inventoryFinish(currentInventory))
                throw new NoCustomerVisitException();

            if (existRevision)
                throw new InventoryRevisionException();

            var revision = new so_inventory_revisions() {
                routeId = RouteId,
                revision_typeId = RevisionType,
                inventoryId = InventoryId,
                date = Date,
                createdBy = UserId,
                createdOn = DateTime.Now,
                modifiedBy =UserId,
                modifiedOn =DateTime.Now,
                revision_stateId = 1,
                status = true,
                userId = UserId
                
            };

            db.so_inventory_revisions.Add(revision);
            db.SaveChanges();

            var dto = new RevisionDto() {

                InventoryRevisionId = revision.inventory_revisionId,
                CreatedOn = revision.createdOn.ToString(),
                RouteId = RouteId,
                InventoryId = InventoryId,
                RevisionType = RevisionType,
                RevisionState = 1
            };

            return dto;

        }

        public List<RouteRevisionState> getStates()
        {
            List<RouteRevisionState> states = null;

            var revisionStates = db.so_revision_states.ToList();

            states = Mapper.Map<List<RouteRevisionState>>(revisionStates);

            return states;
                
        }

        public List<RouteRevisionType> getTypes()
        {
            List<RouteRevisionType> types = null;


            var revisionTypes = db.so_revision_types.ToList();

            types = Mapper.Map<List<RouteRevisionType>>(revisionTypes);

            return types;
        }

        internal RevisionDto FindRevision(int id)
        {
            var revision = db.so_inventory_revisions.Where(r => r.status && r.inventory_revisionId == id).FirstOrDefault();

            if (revision == null)
                throw new InventoryRevisionException();

            var dto = new RevisionDto()
            {

                InventoryRevisionId = revision.inventory_revisionId,
                CreatedOn = revision.createdOn.ToString(),
                RouteId = revision.routeId,
                InventoryId = revision.inventoryId,
                RevisionType = revision.revision_typeId,
                RevisionState = revision.revision_stateId
            };

            return dto;

        }

        public void updateRevision(int inventoryId,int RevisionState)
        {


            var revision = 
                db.so_inventory_revisions
                .Where(r => r.inventoryId == inventoryId && r.status && r.revision_stateId == 1
                )
                .FirstOrDefault();

            if (revision == null) throw new InventoryRevisionException();

                revision.revision_stateId = RevisionState;
                revision.modifiedOn = DateTime.Now;

                db.SaveChanges();
            


        }
    }
}