using GreatReports.Shared;

namespace GreatReports.Domain.Entities;

public class ClientCompany : BaseEntity
{
    public Guid ProviderCompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;

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
