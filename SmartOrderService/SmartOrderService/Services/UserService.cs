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
    public class UserService
    {

        SmartOrderModel db = new SmartOrderModel();
        public UserDto getUser(String UserCode, string BranchCode)
        {
            
            UserDto dto = null;

            var user = db.so_user.Where(u => u.code == UserCode && u.so_branch.code == BranchCode && u.status).FirstOrDefault();

            if (user == null)
                throw new NoUserFoundException();
            
            var route = db.so_user_route.Where(r => r.userId == user.userId && r.status).FirstOrDefault();

            int routeId = route != null ? route.routeId : 0;

            dto = Mapper.Map<UserDto>(user);
            dto.RouteId = routeId;

            return dto;
        }


        public UserDto getUserByToken(string token) {

            var guidToken = Guid.Parse(token);
            var device = db.so_device.Where(d => d.token.Equals(guidToken) && d.status).FirstOrDefault();

            if (device == null)
                throw new DeviceNotFoundException();

            UserDto user = Mapper.Map<UserDto>(device.so_user);


            return user;

        }

        public UserDto getUser(int userId)
        {
            var user = db.so_user.Where(u => u.userId == userId).FirstOrDefault();
            if (user == null)
            {
                throw new NoUserFoundException();
            }

            return Mapper.Map<UserDto>(user);
        }

        public bool updateTrackingConfiguration(string UserCode, string BranchCode, int trackingConfigurationId)
        {
            var user = db.so_user.Where(u => u.code == UserCode && u.so_branch.code == BranchCode && u.status).FirstOrDefault();

            if (user == null)
                throw new NoUserFoundException();

            user.tracking_configurationId = trackingConfigurationId;

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.so_user.Attach(user);
                    var entry = db.Entry(user);
                    entry.Property(tc => tc.tracking_configurationId).IsModified = true;
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    return false;
                }
                return true;
            }
        }
            
    }


}