using GreatReports.Domain.Enums;
using GreatReports.Shared;
using System.Text.RegularExpressions;

namespace GreatReports.Domain.Entities;

public sealed partial class ClientContact : BaseEntity
{
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    public Guid ClientCompanyId { get; }
    public string Name { get; } = string.Empty;
    public string Email { get; } = string.Empty;
    public ContactType Type { get; }

    // EF Core constructor
    private ClientContact() : base()
    {
    }

    private ClientContact(Guid clientCompanyId, string name, string email, ContactType type) : base()
    {
        ClientCompanyId = clientCompanyId;
        Name = name;
        Email = email;
        Type = type;
    }

    public static Result<ClientContact> Create(Guid clientCompanyId, string name, string email, ContactType type)
    {
        if (clientCompanyId == Guid.Empty)
        {
            return Result.Failure<ClientContact>(new Error("ClientContact.InvalidClientCompany", "O ID da empresa cliente associada é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<ClientContact>(new Error("ClientContact.InvalidName", "O nome do contato é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure<ClientContact>(new Error("ClientContact.InvalidEmail", "O e-mail do contato é obrigatório."));
        }

        if (!EmailRegex().IsMatch(email))
        {
            return Result.Failure<ClientContact>(new Error("ClientContact.InvalidEmailFormat", "O e-mail fornecido está em um formato inválido."));
        }

        return new ClientContact(clientCompanyId, name, email, type);
    }
}
