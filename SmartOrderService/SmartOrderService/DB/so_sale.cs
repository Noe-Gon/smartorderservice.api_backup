namespace SmartOrderService.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class so_sale
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public so_sale()
        {
            facturas_so_sale = new HashSet<facturas_so_sale>();
            so_delivery_sale = new HashSet<so_delivery_sale>();
            so_loan_sale = new HashSet<so_loan_sale>();
            so_sale_inventory = new HashSet<so_sale_inventory>();
            so_sale_replacement = new HashSet<so_sale_replacement>();
            so_sale_detail = new HashSet<so_sale_detail>();
            so_sale_promotion = new HashSet<so_sale_promotion>();
        }

        [Key]
        public int saleId { get; set; }

        public int customerId { get; set; }

        public int userId { get; set; }
        public int? deliveryId { get; set; }
        public int? inventoryId { get; set; }

        public DateTime date { get; set; }

        public double total_cash { get; set; }

        public double total_credit { get; set; }

        public string tag { get; set; }

        public int state { get; set; }

        public DateTime? createdon { get; set; }

        public int? createdby { get; set; }

        public DateTime? modifiedon { get; set; }

        public int? modifiedby { get; set; }

        public bool status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<facturas_so_sale> facturas_so_sale { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_invoice_opefactura> so_invoice_opefactura { get; set; }

        public virtual so_customer so_customer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_delivery_sale> so_delivery_sale { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_loan_sale> so_loan_sale { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_sale_inventory> so_sale_inventory { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_sale_replacement> so_sale_replacement { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_sale_detail> so_sale_detail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<so_sale_promotion> so_sale_promotion { get; set; }

        public virtual so_user so_user { get; set; }

        public virtual so_delivery so_delivery { get; set; }

        public virtual so_inventory so_inventory { get; set; }
    }
}
