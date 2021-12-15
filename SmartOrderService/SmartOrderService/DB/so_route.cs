namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_route
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_route()
        {
            so_cellar_notice = new HashSet<so_cellar_notice>();
            so_route_revisions = new HashSet<so_inventory_revisions>();
            so_route_category = new HashSet<so_route_category>();
            so_route_customer = new HashSet<so_route_customer>();
            so_user_route = new HashSet<so_user_route>();
            so_user_notice_recharge_route = new HashSet<so_user_notice_recharge_route>();
        }

        [Key]
        public int routeId { get; set; }

        public int branchId { get; set; }

        [Required]
        [StringLength(50)]
        public string code { get; set; }

        [Required]
        public string name { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public virtual so_branch so_branch { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_cellar_notice> so_cellar_notice { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_inventory_revisions> so_route_revisions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_route_category> so_route_category { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_route_customer> so_route_customer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_user_route> so_user_route { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_user_notice_recharge_route> so_user_notice_recharge_route { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<so_authentication_log> AuthenticationLogs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_route_customer_vario> RouteCustomerVario { get; set; }
    }
}
