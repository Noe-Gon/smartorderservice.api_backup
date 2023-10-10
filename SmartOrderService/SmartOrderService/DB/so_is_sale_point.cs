using System;
using System.ComponentModel.DataAnnotations;

namespace SmartOrderService.DB
{
    public class so_is_sale_point
    {
        [Key]
        public int isSalePointId { get; set; }
        [Required]
        public string branchCode { get; set; }
        [Required]
        public bool isSalePoint { get; set; }
        [Required]
        public DateTime createdon { get; set; }
        public int? createdby { get; set; }
        [Required]
        public DateTime modifiedon { get; set; }
        public int? modifiedby { get; set; }
    }
}