using SmartOrderService.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class DigitalTicketConfigurationService
    {
        private SmartOrderModel db = new SmartOrderModel();

        public string GetPrivacityNotice()
        {
            var digitalTicketConf = db.so_digital_ticket_configuration.Where(x => x.status).FirstOrDefault();

            return digitalTicketConf == null ? string.Empty : digitalTicketConf.url_aviso_privacidad;
        }
    }
}