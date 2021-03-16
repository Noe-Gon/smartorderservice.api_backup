using SmartOrderService.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using SmartOrderService.Models.DTO;

namespace SmartOrderService.Services
{
    public class ReasonsService
    {

        private SmartOrderModel db = new SmartOrderModel();

        public List<ReasonDevolutionDto> getReasonDevolutions()
        {
            var reasons = db.so_reason_devolution.Where(r => r.status);

            return Mapper.Map<List<ReasonDevolutionDto>>(reasons);
        }
    }
}