using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Utils
{
    public static class DateUtils
    {
        public static DateTime getDateTime(String date) {

            DateTime dateTime;
            try
            {
                dateTime = DateTime.Parse(date);
            }catch(Exception e)
            {
                dateTime = new DateTime(0);
            }

            return dateTime;
        }
    }
}