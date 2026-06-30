using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.ClientContacts.Queries;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.ClientContacts.QueryHandlers;

public class GetClientContactsQueryHandler(IClientContactRepository clientContactRepository) : IQueryHandler<GetClientContactsQuery, IReadOnlyList<ClientContactDto>>
{
    public async Task<Result<IReadOnlyList<ClientContactDto>>> HandleAsync(GetClientContactsQuery query, CancellationToken cancellationToken = default)
    {
        var contacts = await clientContactRepository.GetByClientCompanyIdAsync(query.ClientCompanyId, cancellationToken);
        var dtos = contacts.Select(c => new ClientContactDto(c.Id, c.Name, c.Email, c.Type)).ToList();
        return Result<IReadOnlyList<ClientContactDto>>.Success(dtos);
    }
}
