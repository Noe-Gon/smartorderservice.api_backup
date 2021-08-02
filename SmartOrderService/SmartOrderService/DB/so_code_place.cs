using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SmartOrderService.DB
{
    public class so_code_place
    {
        [Column("codePlaceId")]
        public int Id { get; set; }

        [MaxLength(200)]
        [Column("name")]
        public string Name { get; set; }

        [Column("status")]
        public bool Status { get; set; }

        [Column("createdon")]
        public DateTime? CreatedDate { get; set; }

        [Column("createdby")]
        public int? CreatedBy { get; set; }

        [Column("modifiedon")]
        public DateTime? ModifiedDate { get; set; }

        [Column("modifiedby")]
        public int? ModifiedBy { get; set; }

        #region virtuals
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ICollection<so_customer_additional_data> CustomerAdditionalData {get;set;}
        #endregion
    }
}