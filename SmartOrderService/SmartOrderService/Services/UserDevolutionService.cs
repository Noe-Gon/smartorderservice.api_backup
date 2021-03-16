using AutoMapper;
using SmartOrderService.DB;
using SmartOrderService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class UserDevolutionService
    {
        private SmartOrderModel db = new SmartOrderModel();
        public UserDevolutionDto createDevolution(UserDevolutionDto dto,int userId)
        {
            var date = DateTime.Now;

            var devolution = new so_user_devolutions() {
                productId = dto.ProductId,
                inventoryId = dto.InventoryId,
                user_reason_devolutionId = dto.UserReasonDevolutionId,
                status = true,
                createdBy=userId,
                createdOn = date,
                modifiedOn = date,
                modifiedBy = userId,
                amount= dto.Amount,
            };

            db.so_user_devolutions.Add(devolution);

            db.SaveChanges();

            dto.UserDevolutionId = devolution.user_devolutionId;

            return dto;
        }

        public  List<UserReasonDevolutionDto> getReasons()
        {
            List<UserReasonDevolutionDto> reasonsDto;

            var reasons = db.so_user_reason_devolutions.Where(r => r.status).ToList();

            reasonsDto = Mapper.Map<List<UserReasonDevolutionDto>>(reasons);

            return reasonsDto;
        }

        public void deleteDevolution(int UserDevolutionId)
        {
           var devolution = db.so_user_devolutions.Where(d => d.user_devolutionId == UserDevolutionId).FirstOrDefault();

            if (devolution != null)
            {
                db.so_user_devolutions.Remove(devolution);
                db.SaveChanges();
            }
        }

        public void deleteInventoryDevolution(int inventoryId)
        {
            var devolutions = db.so_user_devolutions.Where(d => d.inventoryId == inventoryId);

            if (devolutions.Any())
            {
                db.so_user_devolutions.RemoveRange(devolutions);
                db.SaveChanges();
            }
        }
    }
}