namespace PointofSaleModels.PGDatabaseModels
{
    public class UserRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
    }
}
