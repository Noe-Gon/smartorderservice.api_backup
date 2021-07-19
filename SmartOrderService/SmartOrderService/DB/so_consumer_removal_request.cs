using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_consumer_removal_request
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public int LimitDays { get; set; }
        public string Reason { get; set; }
        public int Status { get; set; }

        #region Relations
        public int ConsumerId { get; set; }
        public so_consumer Consumer { get; set; }

        public int UserId { get; set; }
        public so_user User { get; set; }
        #endregion
    }
}