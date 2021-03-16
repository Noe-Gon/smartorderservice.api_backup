using AutoMapper;
using SmartOrderService.DB;
using SmartOrderService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class ApplicationService
    {
        private SmartOrderModel db = new SmartOrderModel();
        public List<ApplicationDto> getApplications()
        {

            var apps = db.so_application.Where(a => a.status);

            return Mapper.Map<List<ApplicationDto>>(apps);

        }
        public ApplicationDto getApplication(string PackageName)
        {

            var app = db.so_application.FirstOrDefault(a => a.status && a.package.Equals(PackageName));

            return Mapper.Map<ApplicationDto>(app);

        }
    }
}