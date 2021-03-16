namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_branch
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_branch()
        {
            so_collect_bottle = new HashSet<so_collect_bottle>();
            so_products_price_list = new HashSet<so_products_price_list>();
            so_branch_articles = new HashSet<so_branch_articles>();
            so_branch_products_discount_list = new HashSet<so_branch_products_discount_list>();
            so_branch_reason_devolution = new HashSet<so_branch_reason_devolution>();
            so_branch_reason_failed_transaction = new HashSet<so_branch_reason_failed_transaction>();
            so_branch_reason_replacement = new HashSet<so_branch_reason_replacement>();
            so_customer_promotion_config = new HashSet<so_customer_promotion_config>();
            so_global_promotion = new HashSet<so_global_promotion>();
            so_process_branch = new HashSet<so_process_branch>();
            so_product_bottle = new HashSet<so_product_bottle>();
            so_product_category_branch = new HashSet<so_product_category_branch>();
            so_route = new HashSet<so_route>();
            so_user = new HashSet<so_user>();
            so_user_portal_branch = new HashSet<so_user_portal_branch>();
            so_tag = new HashSet<so_tag>();
            so_user_notice_recharge = new HashSet<so_user_notice_recharge>();
        }

        [Key]
        public int branchId { get; set; }

        public int companyId { get; set; }

        [Required]
        [StringLength(50)]
        public string code { get; set; }

        [Required]
        public string name { get; set; }

        public int tax_percent { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        public int closuretypeId { get; set; }

        public int siteId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_collect_bottle> so_collect_bottle { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_products_price_list> so_products_price_list { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_branch_articles> so_branch_articles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_branch_products_discount_list> so_branch_products_discount_list { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_branch_reason_devolution> so_branch_reason_devolution { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_branch_reason_failed_transaction> so_branch_reason_failed_transaction { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_branch_reason_replacement> so_branch_reason_replacement { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_customer_promotion_config> so_customer_promotion_config { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_global_promotion> so_global_promotion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_process_branch> so_process_branch { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_product_bottle> so_product_bottle { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_product_category_branch> so_product_category_branch { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_route> so_route { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_user> so_user { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_user_portal_branch> so_user_portal_branch { get; set; }

        public virtual so_branch_tax so_branch_tax { get; set; }

        public virtual so_company so_company { get; set; }

        public virtual so_site so_site { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_tag> so_tag { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_user_notice_recharge> so_user_notice_recharge { get; set; }
    }
}
