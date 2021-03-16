using AutoMapper;
using SmartOrderService.CustomExceptions;
using SmartOrderService.DB;
using SmartOrderService.Models;
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

    }
}