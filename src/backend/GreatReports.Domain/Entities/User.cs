using GreatReports.Shared;
using System.Text.RegularExpressions;

namespace GreatReports.Domain.Entities;

public class User : BaseEntity
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public Guid ProviderCompanyId { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public bool EmailConfirmed { get; private set; }
    public string? VerificationToken { get; private set; }

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

        if (!EmailRegex.IsMatch(email))
        {
            return Result.Failure<User>(new Error("User.InvalidEmailFormat", "O e-mail fornecido está em um formato inválido."));
        }

        return new User(providerCompanyId, displayName, email);
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
