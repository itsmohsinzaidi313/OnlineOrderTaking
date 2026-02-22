namespace ImportService.Entities
{
    public class UserBranchMapping
    {
        public int UserBranchId { get; set; }
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public bool IsActive { get; set; }
    }
}
