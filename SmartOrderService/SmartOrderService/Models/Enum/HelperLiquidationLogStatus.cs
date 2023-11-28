using SmartOrderService.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Enum
{
    public class HelperLiquidationLogStatus
    {
        public const string UNDEFINED = "UNDEFINED";
        public const string ROUTE_NOT_FOUND = "ROUTE_NOT_FOUND";
        public const string WORK_DAY_NOT_FOUND = "WORK_DAY_NOT_FOUND";
        public const string CONFIGURATION_VALUE_NOT_FOUND = "CONFIGURATION_VALUE_NOT_FOUND";
        public const string UNAUTHORISED = "UNAUTHORISED";
        public const string RUNNING = "RUNNING";
        public const string INTERNAL_SERVER_ERROR = "INTERNAL_SERVER_ERROR";
        public const string SUCCEEDED = "SUCCEEDED";
        public const string FAILED = "FAILED";
        public const string TIMED_OUT = "TIMED_OUT";
        public const string ABORTED = "ABORTED";

        public static int GetLiquidationStatusId(UoWConsumer uoW, string code)
        {
            var liquidationStatus = uoW.LiquidationLogStatusRepository
                .Get(x => x.Code == code)
                .FirstOrDefault();

            if (liquidationStatus == null)
                liquidationStatus = uoW.LiquidationLogStatusRepository
                    .Get(x => x.Code == UNDEFINED)
                    .FirstOrDefault();

            return liquidationStatus.Id;
        }
    }
}