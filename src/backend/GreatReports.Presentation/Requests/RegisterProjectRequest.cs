namespace GreatReports.Presentation.Requests;

public record RegisterProjectRequest(Guid ClientCompanyId, string Name, string Description);
