using GreatReports.Domain.Entities;

namespace GreatReports.Application.Common.Interfaces;

public interface IProviderCompanyRepository : IRepository<ProviderCompany>
{
    Task<bool> ExistsByTaxIdAsync(string taxId, CancellationToken cancellationToken = default);
}
