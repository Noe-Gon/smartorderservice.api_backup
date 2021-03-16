
namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_tracking_configuration
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]

        [Key]
        public int tracking_configurationId { set; get; }
        public bool isDefault { set; get; }
        public bool status { set; get; }
        public bool level_precision { set; get; }
        public long interval { set; get; }
        public DateTime createdon { set; get; }
        public DateTime modifiedon { set; get; }
        public int distance { set; get; }
        
    }
}