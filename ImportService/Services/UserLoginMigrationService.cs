using ImportService.Data;
using ImportService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportService.Services
{
    public class UserLoginMigrationService(SqlServerDbContext sqlServerDbContext) : IUserLoginMigrationService
    {
        public async Task MigrateUserLoginAsync(int companyId, PostgresDbContext pgDb, CancellationToken ct = default)
        {
            var userLogins = await sqlServerDbContext.UserLogins
                                                     .Where(x => x.CompanyId == companyId)
                                                     .AsNoTracking()
                                                     .ToListAsync(cancellationToken: ct);
            pgDb.UserLogins.AddRange(userLogins);

            var userRoles = await sqlServerDbContext.UserRoles
                                                    .Where(x => x.CompanyId == companyId)
                                                    .AsNoTracking()
                                                    .ToListAsync(cancellationToken: ct);
            pgDb.UserRoles.AddRange(userRoles);

            var userBranchMappings = await sqlServerDbContext.UserBranchMappings
                                                            .Join(sqlServerDbContext.BranchMasters, a => a.BranchId, b => b.BranchId, (a, b) => new { Mappings = a, Branch = b })
                                                            .Where(x => x.Branch.CompanyId == companyId)
                                                            .AsNoTracking()
                                                            .Select(x => x.Mappings)
                                                            .ToListAsync(cancellationToken: ct);
            pgDb.UserBranchMappings.AddRange(userBranchMappings);
        }
    }
}
