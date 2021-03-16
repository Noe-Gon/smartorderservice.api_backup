namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_user_notice_recharge
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_user_notice_recharge()
        {
            so_user_notice_recharge_route = new HashSet<so_user_notice_recharge_route>();
        }

        [Key]
        public int user_notice_rechargeId { get; set; }

        public int branchId { get; set; }

        [Required]
        [StringLength(250)]
        public string name { get; set; }

        [Required]
        [StringLength(250)]
        public string mail { get; set; }

        [Required]
        [StringLength(250)]
        public string phone_number { get; set; }

        public bool mail_enabled { get; set; }

        public bool phone_number_enabled { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public virtual so_branch so_branch { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_user_notice_recharge_route> so_user_notice_recharge_route { get; set; }
    }
}
