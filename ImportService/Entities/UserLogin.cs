namespace ImportService.Entities
{
    public class UserLogin
    {
        public int? UserId { get; set; }
        public int? CompanyId { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int? RoleId { get; set; }
        public bool? IsEnabled { get; set; }
        public bool? IsActive { get; set; }
        public string? EmailAddress { get; set; }
    }
}
