namespace SmartOrderService.Utils.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_customer_data
    {
        [Key]
        [Column(Order =0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int customerId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int branch_code { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int route_code { get; set; }

        public bool? credit_apply { get; set; }

        [StringLength(250)]
        public string ftr { get; set; }

        [StringLength(250)]
        public string business_name { get; set; }

        [StringLength(250)]
        public string fiscal_address { get; set; }

        [StringLength(250)]
        public string trade_name { get; set; }

        public int? vat_display { get; set; }

        public int? stps_display { get; set; }

        public int? stps_fee_display { get; set; }

        public int? stps_snack_display { get; set; }

        [StringLength(250)]
        public string payment_method { get; set; }

        [StringLength(50)]
        public string account_ended { get; set; }

        [StringLength(250)]
        public string payment_condition { get; set; }

        public int? pay_days { get; set; }

        [StringLength(250)]
        public string country { get; set; }

        [StringLength(250)]
        public string state { get; set; }

        [StringLength(250)]
        public string town { get; set; }

        [StringLength(250)]
        public string suburb { get; set; }

        [StringLength(250)]
        public string address_street { get; set; }

        [StringLength(250)]
        public string address_number { get; set; }

        [StringLength(250)]
        public string address_number_int { get; set; }

        [StringLength(250)]
        public string postal_code { get; set; }

        [StringLength(250)]
        public string address_number_cross1 { get; set; }

        [StringLength(250)]
        public string address_number_cross2 { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public virtual so_customer so_customer { get; set; }
    }
}
