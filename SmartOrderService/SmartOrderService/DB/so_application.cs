namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_application
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int applicationId { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public string name_installer { get; set; }

        [Required]
        [StringLength(200)]
        public string version { get; set; }

        [Required]
        public string download_url { get; set; }

        [Required]
        public string ws_url { get; set; }

        [Required]
        public string package { get; set; }

        public bool status { get; set; }
    }
}
