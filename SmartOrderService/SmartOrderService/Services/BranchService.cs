using AutoMapper;
using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Models;
using SmartOrderService.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class BranchService
    {

        private SmartOrderModel db = new SmartOrderModel();

        public BranchDto getBranch(int branchId) {

            var dbbranch = db.so_branch.Where(b => b.branchId == branchId).FirstOrDefault();

            if (dbbranch == null)
                throw new EntityNotFoundException();

            return Mapper.Map<BranchDto>(dbbranch);
            

        }

        public List<BranchDto> Get()
        {
            List<BranchDto> branches = new List<BranchDto>();
            var branchesEntity = db.so_branch.Where(b => b.status);

            if (branchesEntity.Any())
                branches = Mapper.Map<List<BranchDto>>(branchesEntity);
            else
                throw new KeyNotFoundException("No se encontraron cedis");

            return branches;
        }

        public ResponseBase<TimeSpan> GetLimitTime(int branchId)
        {
            var Limit = db.so_branch_limit_time.Where(x => x.branchId == branchId).FirstOrDefault();

            if (Limit == null)
                throw new EntityNotFoundException("No esta configurado una hora para el branch " + branchId);

            return ResponseBase<TimeSpan>.Create(Limit.limit_time);
        }

    }
}