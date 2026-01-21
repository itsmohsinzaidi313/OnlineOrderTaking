namespace ImportService.Interfaces
{
    public interface IPaymentModeMigrationService
    {
        Task<int> MigratePaymentModesAsync(int companyId, CancellationToken ct = default);
    }
}


