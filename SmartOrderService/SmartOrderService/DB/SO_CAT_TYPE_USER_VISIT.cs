namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SO_CAT_TYPE_USER_VISIT
    {
        public int? id_type_visit { get; set; }

        [StringLength(40)]
        public string desc_type_visit { get; set; }

        [Key]
        [Column(Order = 0)]
        public bool is_pre_sale { get; set; }

        [Key]
        [Column(Order = 1)]
        public bool is_sale { get; set; }

        [Key]
        [Column(Order = 2)]
        public bool is_delivery { get; set; }

        [Key]
        [Column(Order = 3)]
        public bool with_inventory { get; set; }

        [Key]
        [Column(Order = 4)]
        public bool with_delivery { get; set; }
    }
}
