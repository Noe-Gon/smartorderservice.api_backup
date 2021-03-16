using AutoMapper;
using SmartOrderService.DB;
using SmartOrderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class BillingDataService
    {

        private SmartOrderModel db = new SmartOrderModel();

        public List<BillingDataDto> getBillingDatas()
        {
            

            var dbBillingDatas =db.so_billing_data.ToList();

            List<BillingDataDto> billingDatas = Mapper.Map<List<BillingDataDto>>(dbBillingDatas); 

            return billingDatas;

        }


    }
}