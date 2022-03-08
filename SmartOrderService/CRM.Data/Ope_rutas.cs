namespace CRM.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Ope_rutas
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

        [StringLength(150)]
        public string ope_jefeidName { get; set; }

        [StringLength(150)]
        public string ope_jefedistidName { get; set; }

        [StringLength(100)]
        public string ope_densidadidName { get; set; }

        [StringLength(100)]
        public string ope_geografiaidName { get; set; }

        [StringLength(150)]
        public string Ope_CedisIdName { get; set; }

        [StringLength(160)]
        public string OrganizationIdName { get; set; }

        [StringLength(50)]
        public string Ope_TipoRutaIdName { get; set; }

        [StringLength(100)]
        public string ope_tiporutartmidName { get; set; }

        [StringLength(100)]
        public string ope_rutaIdName { get; set; }

        [StringLength(100)]
        public string Ope_ListaPreciosIdName { get; set; }

        [Key]
        [Column(Order = 0)]
        public Guid Ope_rutasId { get; set; }

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

        public int? Ope_idbijefe { get; set; }

        public int? Ope_idbiruta { get; set; }

        public int? Ope_idcedis { get; set; }

        public int? Ope_idlistprec { get; set; }

        public int? Ope_idruta { get; set; }

        public int? Ope_idtiporuta { get; set; }

        [StringLength(100)]
        public string Ope_nomrutaab { get; set; }

        public int? Ope_rutaope { get; set; }

        public Guid? Ope_CedisId { get; set; }

        public Guid? ope_densidadid { get; set; }

        public Guid? ope_jefeid { get; set; }

        public Guid? ope_geografiaid { get; set; }

        public Guid? Ope_ListaPreciosId { get; set; }

        public Guid? ope_rutaId { get; set; }

        public Guid? Ope_TipoRutaId { get; set; }

        public Guid? ope_tiporutartmid { get; set; }

        public int? ope_cuc_cvario { get; set; }

        public Guid? ope_jefedistid { get; set; }
    }
}
