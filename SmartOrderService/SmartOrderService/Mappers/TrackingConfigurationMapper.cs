using SmartOrderService.DB;
using SmartOrderService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Mappers
{
    public class TrackingConfigurationMapper : IMapper<so_tracking_configuration, TrackingConfigurationDto>
    {
        public TrackingConfigurationDto toEntity(so_tracking_configuration model)
        {
            TrackingConfigurationDto dto = new TrackingConfigurationDto();
            dto.Distance = model.distance;
            dto.Id = model.tracking_configurationId;
            dto.Interval = model.interval;
            dto.HighPrecision = model.level_precision;
            return dto;
        }

        public so_tracking_configuration toModel(TrackingConfigurationDto entity)
        {
            throw new NotImplementedException();
        }
    }
}