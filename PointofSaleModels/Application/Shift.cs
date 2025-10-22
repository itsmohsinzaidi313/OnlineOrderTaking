namespace PointofSaleModels.Application
{
    public class Shift
    {
        public int? Id { get; set; }
        public string? ShiftNumber { get; set; } = string.Empty;
        public string? ShiftName { get; set; }
        public string? OpeningDate { get; set; } = string.Empty;
        public string? OpenedBy { get; set; }
        public int? BusinessDayId { get; set; }
        public string? BusinessDate { get; set; }
    }
}
