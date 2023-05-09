namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_product()
        {
            so_collect_bottle_detail = new HashSet<so_collect_bottle_detail>();
            so_container_detail = new HashSet<so_container_detail>();
            so_delivery_detail = new HashSet<so_delivery_detail>();
            so_delivery_promotion_article_detail = new HashSet<so_delivery_promotion_article_detail>();
            so_delivery_promotion_detail = new HashSet<so_delivery_promotion_detail>();
            so_equivalence_article = new HashSet<so_equivalence_article>();
            so_equivalence_product = new HashSet<so_equivalence_product>();
            so_inventory_detail = new HashSet<so_inventory_detail>();
            so_inventory_detail_article = new HashSet<so_inventory_detail_article>();
            so_inventory_product_container = new HashSet<so_inventory_product_container>();
            so_loan_order = new HashSet<so_loan_order>();
            so_loan_sale = new HashSet<so_loan_sale>();
            so_price_list_products_detail = new HashSet<so_price_list_products_detail>();
            so_user_devolutions = new HashSet<so_user_devolutions>();
            so_product_article = new HashSet<so_product_article>();
            so_promotion_detail_article = new HashSet<so_promotion_detail_article>();
            so_product_bottle = new HashSet<so_product_bottle>();
            so_product_company = new HashSet<so_product_company>();
            so_product_article1 = new HashSet<so_product_article>();
            so_product_bottle1 = new HashSet<so_product_bottle>();
            so_product_category_branch = new HashSet<so_product_category_branch>();
            so_products_discount_list_detail = new HashSet<so_products_discount_list_detail>();
            so_promotion_detail_product = new HashSet<so_promotion_detail_product>();
            so_sale_detail = new HashSet<so_sale_detail>();
            so_sale_promotion_detail = new HashSet<so_sale_promotion_detail>();
            so_reception_bottle_detail = new HashSet<so_reception_bottle_detail>();
            so_delivery_combo_details = new HashSet<so_delivery_combo_detail>();
            so_sale_combo_details = new HashSet<so_sale_combo_detail>();
        }

        [Key]
        public int productId { get; set; }

        public int brandId { get; set; }

        [StringLength(50)]
        public string code { get; set; }

        [Required]
        public string name { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool is_returnable { get; set; }

        public int pieces { get; set; }

        public bool status { get; set; }

        public double factor { get; set; }

        public int? type { get; set; }

        [StringLength(250)]
        public string barcode { get; set; }

        [StringLength(250)]
        public string reference { get; set; }

        public int? billing_dataId { get; set; }

        public virtual so_billing_data so_billing_data { get; set; }

        public virtual so_brand so_brand { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_collect_bottle_detail> so_collect_bottle_detail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_container_detail> so_container_detail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_delivery_detail> so_delivery_detail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_delivery_promotion_article_detail> so_delivery_promotion_article_detail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_delivery_promotion_detail> so_delivery_promotion_detail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_equivalence_article> so_equivalence_article { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_equivalence_product> so_equivalence_product { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_inventory_detail> so_inventory_detail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_inventory_detail_article> so_inventory_detail_article { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_inventory_product_container> so_inventory_product_container { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_loan_order> so_loan_order { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_loan_sale> so_loan_sale { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_price_list_products_detail> so_price_list_products_detail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_user_devolutions> so_user_devolutions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_product_article> so_product_article { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_promotion_detail_article> so_promotion_detail_article { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_product_bottle> so_product_bottle { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_product_company> so_product_company { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_product_article> so_product_article1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_product_bottle> so_product_bottle1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_product_category_branch> so_product_category_branch { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_products_discount_list_detail> so_products_discount_list_detail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_promotion_detail_product> so_promotion_detail_product { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_sale_detail> so_sale_detail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_sale_promotion_detail> so_sale_promotion_detail { get; set; }

        public virtual so_product_tax so_product_tax { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_reception_bottle_detail> so_reception_bottle_detail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_delivery_combo_detail> so_delivery_combo_details { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_sale_combo_detail> so_sale_combo_details { get; set; } 

    }
}
