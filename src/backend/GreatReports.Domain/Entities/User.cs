using GreatReports.Shared;
using System.Text.RegularExpressions;

namespace GreatReports.Domain.Entities;

public sealed partial class User : BaseEntity
{
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    public Guid ProviderCompanyId { get; }
    public string DisplayName { get; } = string.Empty;
    public string Email { get; } = string.Empty;
    public bool EmailConfirmed { get; private set; }

    // EF Core constructor
    private User() : base()
    {
    }

    private User(Guid providerCompanyId, string displayName, string email) : base()
    {
        ProviderCompanyId = providerCompanyId;
        DisplayName = displayName;
        Email = email;
        EmailConfirmed = false;
    }

    public static Result<User> Create(Guid providerCompanyId, string displayName, string email)
    {
        if (providerCompanyId == Guid.Empty)
        {
            return Result.Failure<User>(new Error("User.InvalidProviderCompany", "O ID do provedor associado é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            return Result.Failure<User>(new Error("User.InvalidDisplayName", "O nome exibido do usuário é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Failure<User>(new Error("User.InvalidEmail", "O e-mail do usuário é obrigatório."));
        }

        if (!EmailRegex().IsMatch(email))
        {
            return Result.Failure<User>(new Error("User.InvalidEmailFormat", "O e-mail fornecido está em um formato inválido."));
        }

        return new User(providerCompanyId, displayName, email);
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        Update();
    }
}
