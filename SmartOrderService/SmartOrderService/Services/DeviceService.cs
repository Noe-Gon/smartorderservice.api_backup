using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartOrderService.Models.DTO;
using SmartOrderService.DB;
using SmartOrderService.CustomExceptions;

namespace SmartOrderService.Services
{
    public class DeviceService
    {
        SmartOrderModel db = new SmartOrderModel();

        public Device RegisterDevice(Device DeviceDto)
        {
            String UserCode = DeviceDto.UserCode;
            String BranchCode = DeviceDto.BranchCode;
            var Code = DeviceDto.DeviceCode;
            Guid Token = Guid.NewGuid();
            var Time = DateTime.Now;

            int UserId = db.so_user.
                Join(db.so_branch,
                u => u.branchId,
                b => b.branchId,
                (u, b) => new { u.userId, userCode = u.code, branchCode = b.code,UserStatus =u.status,BranchStatus =b.status }
                ).Where(
                    r => r.userCode.Equals(UserCode)
                    && r.branchCode.Equals(BranchCode)
                    && r.UserStatus
                    && r.BranchStatus
                ).Select (r => r.userId).FirstOrDefault();

            
            var DeviceRegistered = db.so_device.Where(d => d.code.Equals(Code) && d.status).FirstOrDefault();

            if (UserId == 0)  throw new NoUserFoundException();

            if (DeviceRegistered != null)
            {
                if (DeviceRegistered.userId.Equals(UserId))
                {
                    DeviceDto.Token = DeviceRegistered.token;
                }
                else
                {
                    int currentUserId = DeviceRegistered.userId;
                    var WorkDay = db.so_work_day.Where(w => w.userId== currentUserId && !w.date_end.HasValue).FirstOrDefault();

                    if (WorkDay != null) { throw new WorkdayStartedException(); }
                    else {
                        //deshabilitamos el registro anterior
                        DeviceRegistered.userId = UserId;
                        DeviceRegistered.token = Token;
                        DeviceRegistered.modifiedby = UserId;
                        DeviceRegistered.modifiedon = Time;
                        DeviceRegistered.status = true;
                    }
                }
            }
            else
            {
                //validar que el usuario no tenga una sesión iniciada

               var WorkDay = db.so_work_day.Where(w => w.userId == UserId && !w.date_end.HasValue).FirstOrDefault();

                if (WorkDay != null) throw new WorkdayStartedException();

                db.so_device.Add(
                    new so_device()
                    {
                        userId = UserId,
                        code = Code,
                        token = Token,
                        createdon = Time,
                        modifiedon = Time,
                        createdby = UserId,
                        modifiedby = UserId,
                        status = true
                        
                    }
                    );
                

                
            }

            DeviceDto.Token = Token;

            db.SaveChanges();


            return DeviceDto;

        }
    }
}