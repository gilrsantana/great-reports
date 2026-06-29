using GreatReports.Application.Common.CQRS;

namespace GreatReports.Application.UseCases.ClientContacts.Commands;

public record AddClientContactCommand(Guid ClientCompanyId, string Name, string Email, string ContactType) : ICommand<Guid>;
