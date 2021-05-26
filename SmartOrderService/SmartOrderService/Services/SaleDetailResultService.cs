using SmartOrderService.Models;
using SmartOrderService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class SaleDetailResultService
    {
        public SaleDetailResult SaleDeatailMapping(int amountSold,SaleDetail saleDetail)
        {
            SaleDetailResult saleDetailResult = new SaleDetailResult(saleDetail);
            saleDetailResult.AmountSold = amountSold;
            return saleDetailResult;
        }
    }
}