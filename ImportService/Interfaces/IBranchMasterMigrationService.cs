namespace ImportService.Interfaces
{
    public interface IBranchMasterMigrationService
    {
        Task<int> MigrateBranchMasterAsync(int companyId, CancellationToken ct = default);
    }
}
