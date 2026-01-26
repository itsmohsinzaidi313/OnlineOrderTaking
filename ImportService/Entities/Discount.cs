using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImportService.Entities
{
    [Table("Discount")]
    public class Discount
    {
        [Key]
        [Column("DiscountId")]
        public int DiscountId { get; set; }

        [Column("DiscountPercent", TypeName = "double precision")]
        public double DiscountPercent { get; set; }

        [Column("DiscountTimeStart", TypeName = "time")]
        public TimeSpan DiscountTimeStart { get; set; }

        [Column("DiscountTimeEnd", TypeName = "time")]
        public TimeSpan DiscountTimeEnd { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; }

        [Column("DiscountName", TypeName = "varchar(255)")]
        public string? DiscountName { get; set; }

        [Column("IsOpen")]
        public bool IsOpen { get; set; }

        [Column("IsActiveInWeb")]
        public bool? IsActiveInWeb { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Column("IsAreaWise")]
        public bool IsAreaWise { get; set; }

        [Column("IsDayWise")]
        public bool IsDayWise { get; set; }

        [Column("Priority")]
        public int Priority { get; set; }

        [Column("IsActiveInPOS")]
        public bool IsActiveInPOS { get; set; }

        [Column("DiscountTypeId")]
        public int? DiscountTypeId { get; set; }

        [Column("CompanyId")]
        public int? CompanyId { get; set; }

        [Column("IsActiveInMobile")]
        public bool? IsActiveInMobile { get; set; }

        [Column("IsActiveInODMS")]
        public bool? IsActiveInODMS { get; set; }

        [Column("IsPercentage")]
        public bool IsPercentage { get; set; }

        [Column("IsAutoDiscount")]
        public bool IsAutoDiscount { get; set; }

        [Column("DiscountCapStart", TypeName = "numeric(18,2)")]
        public decimal DiscountCapStart { get; set; }

        [Column("DiscountCapEnd", TypeName = "numeric(18,2)")]
        public decimal DiscountCapEnd { get; set; }

        [Column("IsVoucher")]
        public bool IsVoucher { get; set; }

        [Column("FirstTimeUserOnly")]
        public bool FirstTimeUserOnly { get; set; }

        [Column("DeliveryChangesWaiveOff")]
        public bool DeliveryChangesWaiveOff { get; set; }

        [Column("MaxCount")]
        public int MaxCount { get; set; }

        [Column("MaxCountPerUser")]
        public int MaxCountPerUser { get; set; }

        [Column("MinOrderAmount")]
        public int MinOrderAmount { get; set; }

        [Column("ApplyWithOtherDiscounts")]
        public bool ApplyWithOtherDiscounts { get; set; }

        [Column("VocherCodeStart", TypeName = "varchar(100)")]
        public string? VocherCodeStart { get; set; }

        [Column("VocherCodeEnd", TypeName = "varchar(100)")]
        public string? VocherCodeEnd { get; set; }

    }
}
