using GreatReports.Shared;

namespace GreatReports.Domain.Entities;

public sealed class ProviderCompany : BaseEntity
{
    public string Name { get; } = string.Empty;
    public string TaxId { get; } = string.Empty;

    // EF Core constructor
    private ProviderCompany() : base()
    {
    }

    private ProviderCompany(string name, string taxId) : base()
    {
        Name = name;
        TaxId = taxId;
    }

    public static Result<ProviderCompany> Create(string name, string taxId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<ProviderCompany>(new Error("ProviderCompany.InvalidName", "O nome do provedor é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(taxId))
        {
            return Result.Failure<ProviderCompany>(new Error("ProviderCompany.InvalidTaxId", "O CNPJ/Identificação fiscal do provedor é obrigatório."));
        }

        return new ProviderCompany(name, taxId);
    }
}
