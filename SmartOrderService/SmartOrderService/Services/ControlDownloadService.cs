using SmartOrderService.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Services
{
    public class ControlDownloadService
    {
        public const int MODEL_TYPE_PRE_SALE = 0;

        public const int MODEL_TYPE_SALE = 1;

        public const int MODEL_TYPE_BINNACLE_VISIT = 2;

        public const int MODEL_TYPE_DELIVERY_DEVOLUTION = 3;


        public so_control_download createControlDownload(int modelId, int userId,int modelType)
        {
            return new so_control_download()
            {
                userId = userId,
                modelId = modelId,
                model_type = modelType,
                process_type = 0,
                closed = true
            };
        }
    }
}