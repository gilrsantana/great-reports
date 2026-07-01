using GreatReports.Shared;

namespace GreatReports.Domain.Entities;

public sealed class ProviderCompany : BaseEntity
{
    public string Name { get; } = string.Empty;
    public string TaxId { get; } = string.Empty;
    public Guid ManagerId { get; } = Guid.Empty;
    public User Manager { get; } = null!;

    // EF Core constructor
    private ProviderCompany() : base()
    {
    }

    private ProviderCompany(string name, string taxId, Guid managerId) : base()
    {
        Name = name;
        TaxId = taxId;
        ManagerId = managerId;
    }

    public static Result<ProviderCompany> Create(string name, string taxId, Guid managerId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<ProviderCompany>(new Error("ProviderCompany.InvalidName", "O nome do provedor é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(taxId))
        {
            return Result.Failure<ProviderCompany>(new Error("ProviderCompany.InvalidTaxId", "O CNPJ/Identificação fiscal do provedor é obrigatório."));
        }

        if (managerId == Guid.Empty)
        {
            return Result.Failure<ProviderCompany>(new Error("ProviderCompany.InvalidManagerId", "O gestor é obrigatório."));
        }

        return new ProviderCompany(name, taxId, managerId);
    }
}
