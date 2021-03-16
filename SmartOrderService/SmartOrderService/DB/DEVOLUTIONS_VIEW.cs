namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DEVOLUTIONS_VIEW
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int deliveryId { get; set; }

        public int? saleId { get; set; }

        [StringLength(50)]
        public string product_code { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string user_code { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string branch_code { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(50)]
        public string customer_code { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(50)]
        public string inventory_code { get; set; }

        [Key]
        [Column(Order = 5, TypeName = "date")]
        public DateTime inventory_date { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(50)]
        public string reason_code { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int promotion_code { get; set; }

        [Key]
        [Column(Order = 8)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int replacement_code { get; set; }

        public int? amount { get; set; }

        public int? credit { get; set; }

        [Key]
        [Column(Order = 9)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int detail_type { get; set; }

        [Key]
        [Column(Order = 10)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int status { get; set; }
    }
}
