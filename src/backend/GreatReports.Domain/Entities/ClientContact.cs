using GreatReports.Domain.Enums;
using GreatReports.Shared;
using System.Text.RegularExpressions;

namespace GreatReports.Domain.Entities;

public class ClientContact : BaseEntity
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Guid ClientCompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public bool EmailConfirmed { get; private set; }
    public string? VerificationToken { get; private set; }
    public ContactType Type { get; private set; }

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
        EmailConfirmed = false;
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

        if (!EmailRegex.IsMatch(email))
        {
            return Result.Failure<ClientContact>(new Error("ClientContact.InvalidEmailFormat", "O e-mail fornecido está em um formato inválido."));
        }

        return new ClientContact(clientCompanyId, name, email, type);
    }

    public void GenerateVerificationToken()
    {
        VerificationToken = Guid.NewGuid().ToString("N");
        Update();
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        VerificationToken = null;
        Update();
    }
}
