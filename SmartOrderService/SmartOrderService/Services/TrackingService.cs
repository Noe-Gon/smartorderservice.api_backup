using SmartOrderService.DB;
using SmartOrderService.Mappers;
using SmartOrderService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class TrackingService
    {
        private SmartOrderModel db = new SmartOrderModel();

        public void RegisterPoint(TrackingDto dto) {

            var User = new UserService().getUser(dto.UserCode, dto.BranchCode);

            var Points = dto.Points;

            foreach (var Point in Points) {
                DateTime Date = DateTime.Parse(Point.Date);

                var point = new so_tracking()
                {
                    UserId = User.UserId,
                    Accuracy = Point.Accuracy,
                    CreatedBy = User.UserId,
                    CreatedOn = DateTime.Now,
                    Latitude = Point.Latitude,
                    Longitude = Point.Longitude,
                    Date = Date,
                    Sequence = Point.Sequence,
                    Status = true,
                    LevelBattery = Point.LevelBattery
                };

                db.so_tracking.Add(point);
            }
                       
            db.SaveChanges();
        }

        public TrackingConfigurationDto getConfiguration(string UserCode, string BranchCode)
        {
            var user = db.so_user.Where(u => u.code == UserCode && u.so_branch.code == BranchCode && u.status).FirstOrDefault();

            if (user == null)
                throw new CustomExceptions.NoUserFoundException();

            var configuration = user.so_tracking_configuration;

            if (configuration == null)
            {
                configuration= db.so_tracking_configuration.Where(tc => tc.isDefault).FirstOrDefault();
            }


            TrackingConfigurationDto dto = new TrackingConfigurationMapper().toEntity(configuration);
            return dto;

        }

        public List<TrackingConfigurationDto> getAll()
        {
            List<so_tracking_configuration> configurations = db.so_tracking_configuration.Where(tc => tc.status).ToList();

            List<TrackingConfigurationDto> dtos = new List<TrackingConfigurationDto>();

            if (configurations != null)
            {
                TrackingConfigurationMapper mapper = new TrackingConfigurationMapper();
                foreach (so_tracking_configuration configuration in configurations)
                {
                    dtos.Add(mapper.toEntity(configuration));
                }
            }

            return dtos;
        }

    }
}