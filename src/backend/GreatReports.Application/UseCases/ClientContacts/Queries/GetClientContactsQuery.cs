using GreatReports.Application.Common.CQRS;
using GreatReports.Domain.Enums;

namespace GreatReports.Application.UseCases.ClientContacts.Queries;

public record ClientContactDto(Guid Id, string Name, string Email, ContactType Type);

public record GetClientContactsQuery(Guid ClientCompanyId) : IQuery<IReadOnlyList<ClientContactDto>>;
