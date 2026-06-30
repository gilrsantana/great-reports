namespace GreatReports.Presentation.Requests;

public record CreateGroupRequest(
    Guid GroupLeaderId,
    Guid ClientCompanyId,
    Guid ProjectId,
    string Name,
    string Timezone);
