using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;
using PointofSaleModels.DatabaseContexts;

namespace ImportService.Services
{
    public class UserLoginMigrationService(SqlServerDbContext sqlServerDbContext) : IUserLoginMigrationService
    {
        public async Task MigrateAsync(PostgresDbContext pgDb, int companyId = 0, CancellationToken ct = default)
        {
            var userLogins = await sqlServerDbContext.UserLogins
                                                     .AsNoTracking()
                                                     .Where(x => x.CompanyId == companyId)
                                                     .ToListAsync(cancellationToken: ct);

            pgDb.UserLogins.AddRange(userLogins.Select(x => new PointofSaleModels.Entities.UserLogin
            {
                UserId = x.UserId,
                CompanyId = x.CompanyId,
                Name = x.Name ?? string.Empty,
                Username = x.Username ?? string.Empty,
                Password = x.Password ?? string.Empty,
                RoleId = x.RoleId,
                IsEnabled = x.IsEnabled ?? false,
                IsActive = x.IsActive ?? false,
                EmailAddress = x.EmailAddress ?? string.Empty,
            }));

            var userRoles = await sqlServerDbContext.UserRoles
                                                    .AsNoTracking()
                                                    .Where(x => x.CompanyId == companyId)
                                                    .Where(x => x.RoleId != 0)
                                                    .ToListAsync(cancellationToken: ct);
            if (userRoles != null)
                pgDb.UserRoles.AddRange(userRoles.Select(x => new PointofSaleModels.Entities.UserRole
                {
                    RoleId = x.RoleId,
                    RoleName = x.RoleName ?? string.Empty,
                    CompanyId = x.CompanyId,
                    IsActive = x.IsActive ?? false,
                }));

            var userBranchMappings = await sqlServerDbContext.UserBranchMappings
                                                            .AsNoTracking()
                                                            .Join(sqlServerDbContext.BranchMasters, a => a.BranchId, b => b.BranchId, (a, b) => new { Mappings = a, Branch = b })
                                                            .Where(x => x.Branch.CompanyId == companyId)
                                                            .Select(x => x.Mappings)
                                                            .ToListAsync(cancellationToken: ct);

            if (userBranchMappings != null)
                pgDb.UserBranchMappings.AddRange(userBranchMappings);
        }
    }
}
