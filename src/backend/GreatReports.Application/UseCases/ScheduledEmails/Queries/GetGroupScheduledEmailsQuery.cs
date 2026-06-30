using GreatReports.Application.Common.CQRS;

namespace GreatReports.Application.UseCases.ScheduledEmails.Queries;

public record GetGroupScheduledEmailsQuery(Guid GroupId) : IQuery<IReadOnlyList<ScheduledEmailDto>>;
