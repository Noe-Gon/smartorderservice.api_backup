using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Models.DTO
{
    public class ApplicationDto
    {

        public int ApplicationId {
            get; set;
        }

        public string Name { get; set; }
        public string NameInstaller { get; set; }
        public string DownloadUrl { get; set; }
        public string WSUrl { get; set; }

        public string Package { get; set; }
        public bool Status { get; set; }
        public int Version { get; set; }


    }
}