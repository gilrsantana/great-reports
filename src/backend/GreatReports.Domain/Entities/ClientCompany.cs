using GreatReports.Shared;

namespace GreatReports.Domain.Entities;

public sealed class ClientCompany : BaseEntity
{
    public Guid ProviderCompanyId { get; }
    public string Name { get; } = string.Empty;

    // EF Core constructor
    private ClientCompany() : base()
    {
    }

    private ClientCompany(Guid providerCompanyId, string name) : base()
    {
        ProviderCompanyId = providerCompanyId;
        Name = name;
    }

    public static Result<ClientCompany> Create(Guid providerCompanyId, string name)
    {
        if (providerCompanyId == Guid.Empty)
        {
            return Result.Failure<ClientCompany>(new Error("ClientCompany.InvalidProvider", "O ID do provedor associado é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<ClientCompany>(new Error("ClientCompany.InvalidName", "O nome da empresa cliente é obrigatório."));
        }

        return new ClientCompany(providerCompanyId, name);
    }
}
