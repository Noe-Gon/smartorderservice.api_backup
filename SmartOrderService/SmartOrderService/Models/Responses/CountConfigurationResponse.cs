using SmartOrderService.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.Responses
{
    public class CountConfigurationResponse
    {

        public List<CountConfigurationDto> CountConfigurations { get; set; }
        public int TotalRecords { get; set; }
    }
}