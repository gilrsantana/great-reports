namespace GreatReports.Presentation.Requests;

public record RegisterUserRequest(Guid ProviderCompanyId, string DisplayName, string Email, string Role);
