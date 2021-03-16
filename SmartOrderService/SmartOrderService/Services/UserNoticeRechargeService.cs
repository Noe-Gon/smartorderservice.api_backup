using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using SmartOrderService.DB;
using SmartOrderService.Models.DTO;

namespace SmartOrderService.Services
{
    public class UserNoticeRechargeService
    {
        SmartOrderModel db = new SmartOrderModel();

        public List<UserNoticeRechargeDto> GetByBranch(int branchId)
        {
            List<UserNoticeRechargeDto> usersDto = new List<UserNoticeRechargeDto>();
            var users = db.so_user_notice_recharge.Where(u => u.branchId == branchId && u.status);
            if (users.Any())
                usersDto = Mapper.Map<List<UserNoticeRechargeDto>>(users);
            else
                throw new KeyNotFoundException("No se encontraron usuarios para el cedisId=" + branchId);
            return usersDto;
        }

        public UserNoticeRechargeDto Get(int id)
        {
            UserNoticeRechargeDto userDto = new UserNoticeRechargeDto();
            var user = db.so_user_notice_recharge.FirstOrDefault(u => u.user_notice_rechargeId == id && u.status);
            if (user != null)
                userDto = Mapper.Map<UserNoticeRechargeDto>(user);
            else
                throw new KeyNotFoundException("No se encontro al usuario con id=" + id);

            return userDto;
        }

        public void Create(UserNoticeRechargeDto userDto)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    so_user_notice_recharge entity = new so_user_notice_recharge
                    {
                        branchId = userDto.BranchId,
                        name = userDto.Name,
                        mail = userDto.Mail,
                        phone_number = userDto.PhoneNumber,
                        mail_enabled = userDto.MailEnabled,
                        phone_number_enabled = userDto.PhoneNumberEnabled,
                        createdon = DateTime.Now,
                        createdby = 1,
                        modifiedon = DateTime.Now,
                        modifiedby = 1,
                        status = true
                    };

                    db.so_user_notice_recharge.Add(entity);
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                    userDto.Id = entity.user_notice_rechargeId;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public void AssignRoutes_(int id, List<int> routeIds)
        {
            List<so_user_notice_recharge_route> entitiesCollection = new List<so_user_notice_recharge_route>();

            foreach (int routeId in routeIds)
            {
                entitiesCollection.Add(new so_user_notice_recharge_route
                {
                    routeId = routeId,
                    user_notice_rechargeId = id,
                    createdon = DateTime.Now,
                    createdby = 1,
                    modifiedon = DateTime.Now,
                    modifiedby = 1,
                    status = true
                });
            }
            
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.so_user_notice_recharge_route.AddRange(entitiesCollection);
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                }
            }
        }

        public void Update(int id, UserNoticeRechargeDto user)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    so_user_notice_recharge entity = db.so_user_notice_recharge.FirstOrDefault(u => u.user_notice_rechargeId == id);
                    if (entity != null)
                    {
                        entity.name = user.Name;
                        entity.mail = user.Mail;
                        entity.phone_number = user.PhoneNumber;
                        entity.mail_enabled = user.MailEnabled;
                        entity.phone_number_enabled = user.PhoneNumberEnabled;
                        entity.modifiedon = DateTime.Now;
                        entity.modifiedby = 1;
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    else
                    {
                        throw new KeyNotFoundException("No se encontro al usuario con id=" + id);
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public void AssignRoutes(int id, List<int> routeIds)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    DeactivateUserNoticeRechargeRoute(id, routeIds);
                    ValidateUserNoticeRechargeRoute(id, routeIds);
                    db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public void Deactivate(int id)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    so_user_notice_recharge entity = db.so_user_notice_recharge.FirstOrDefault(u => u.user_notice_rechargeId == id);
                    if (entity != null)
                    {
                        entity.status = false;
                        foreach (var detail in entity.so_user_notice_recharge_route)
                        {
                            detail.status = false;
                            detail.modifiedon = DateTime.Now;
                            detail.modifiedby = 1;
                        }
                        db.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    else
                    {
                        throw new KeyNotFoundException("No se encontro al usuario con id=" + id);
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public void DeactivateUserNoticeRechargeRoute(int id, List<int> routeIds)
        {
            List<so_user_notice_recharge_route> entitiesCollection = db.so_user_notice_recharge_route.
                        Where(ur => ur.user_notice_rechargeId == id && !routeIds.Contains(ur.routeId) && ur.status).ToList();

            foreach (var entity in entitiesCollection)//inactiva
            {
                entity.status = false;
                entity.modifiedon = DateTime.Now;
                entity.modifiedby = 1;
            }
        }

        public void ValidateUserNoticeRechargeRoute(int id, List<int> routeIds)
        {
            foreach (int routeId in routeIds)
            {
                List<so_user_notice_recharge_route> entitiesCollection = db.so_user_notice_recharge_route.
                    Where(ur => ur.user_notice_rechargeId == id && ur.routeId == routeId).ToList();

                if (entitiesCollection.Any()) //Activar
                {
                    foreach (var entity in entitiesCollection)
                    {
                        if (!entity.status)
                        {
                            entity.status = true;
                            entity.modifiedon = DateTime.Now;
                            entity.modifiedby = 1;
                        }
                    }
                }
                else//insertar
                {
                    db.so_user_notice_recharge_route.Add(new so_user_notice_recharge_route
                    {
                        routeId = routeId,
                        user_notice_rechargeId = id,
                        createdon = DateTime.Now,
                        createdby = 1,
                        modifiedon = DateTime.Now,
                        modifiedby = 1,
                        status = true
                    });
                }
            }
        }
    }
}