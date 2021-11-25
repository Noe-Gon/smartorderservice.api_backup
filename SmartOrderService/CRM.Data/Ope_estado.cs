namespace CRM.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Ope_estado
    {
        [StringLength(160)]
        public string CreatedByName { get; set; }

        [StringLength(160)]
        public string CreatedByYomiName { get; set; }

        [StringLength(160)]
        public string CreatedOnBehalfByName { get; set; }

        [StringLength(160)]
        public string CreatedOnBehalfByYomiName { get; set; }

        [StringLength(160)]
        public string ModifiedByName { get; set; }

        [StringLength(160)]
        public string ModifiedByYomiName { get; set; }

        [StringLength(160)]
        public string ModifiedOnBehalfByName { get; set; }

        [StringLength(160)]
        public string ModifiedOnBehalfByYomiName { get; set; }

        [StringLength(100)]
        public string Ope_PaisIdName { get; set; }

        [StringLength(160)]
        public string OrganizationIdName { get; set; }

        [Key]
        [Column(Order = 0)]
        public Guid Ope_estadoId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public Guid? ModifiedBy { get; set; }

        public Guid? CreatedOnBehalfBy { get; set; }

        public Guid? ModifiedOnBehalfBy { get; set; }

        public Guid? OrganizationId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int statecode { get; set; }

        public int? statuscode { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] VersionNumber { get; set; }

        public int? ImportSequenceNumber { get; set; }

        public DateTime? OverriddenCreatedOn { get; set; }

        public int? TimeZoneRuleVersionNumber { get; set; }

        public int? UTCConversionTimeZoneCode { get; set; }

        [StringLength(100)]
        public string Ope_name { get; set; }

        public int? Ope_idestado { get; set; }

        public int? Ope_idpais { get; set; }

        public Guid? Ope_PaisId { get; set; }
    }
}
