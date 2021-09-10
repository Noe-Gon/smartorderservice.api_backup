using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class SaleDetailResponse
    {
        public double Price;
        public int Amount;
        public int CreditAmount;
        public double Import;
        public int ProductId;
    }
}