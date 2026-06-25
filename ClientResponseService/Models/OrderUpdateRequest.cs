namespace ClientResponseService.Models
{
    public class OrderUpdateRequest
    {
        public int? OrderStatusId { get; set; }
        public int? BranchTransferId { get; set; }
        public int? RiderId { get; set; }
        public int? DeliveryTime { get; set; }
    }
}
