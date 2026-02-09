using Grpc.Core;
using ImportService.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Db = PointofSaleModels.PGDatabaseModels;

namespace ImportService
{
    internal class BranchDataService(IDbContextFactory<SqlServerDbContext> contextFactory) : BranchDataGrpc.BranchDataGrpcBase
    {
        public override async Task<BranchDataResponse> GetBranchData(BranchDataRequest request, ServerCallContext context)
        {
            var connectionString = await GetData(request);
            return await base.GetBranchData(request, context);
        }

        private async Task<string> GetData(BranchDataRequest request)
        {
            using var context = contextFactory.CreateDbContext();
            var user = await context.UserLogins.FirstOrDefaultAsync(u => u.Username == request.Username);
            if(user == null)
            {
                throw new Exception($"User with username {request.Username} not found.");
            }
            var branchMapping = await context.UserBranchMappings.FirstOrDefaultAsync(x => x.UserId == user.UserId);
            if(branchMapping == null)
            {
                throw new Exception($"Branch mapping for user {request.Username} not found.");
            }
            return branchMapping.BranchId.ToString();
        }
    }
}
