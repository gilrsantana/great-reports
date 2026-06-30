using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.ScheduledEmails.Queries;
using GreatReports.Domain.Entities;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.ScheduledEmails.QueryHandlers;

public class GetGroupScheduledEmailsQueryHandler(
    IScheduledEmailRepository scheduledEmailRepository,
    IScheduledEmailReceiverRepository receiverRepository,
    IUserRepository userRepository,
    IClientContactRepository clientContactRepository)
    : IQueryHandler<GetGroupScheduledEmailsQuery, IReadOnlyList<ScheduledEmailDto>>
{
    public async Task<Result<IReadOnlyList<ScheduledEmailDto>>> HandleAsync(
        GetGroupScheduledEmailsQuery query, CancellationToken cancellationToken = default)
    {
        var schedules = await scheduledEmailRepository.GetByGroupIdAsync(query.GroupId, cancellationToken);
        var scheduleIds = schedules.Select(s => s.Id).ToList();

        // Get all receivers for these schedules
        var allReceivers = await receiverRepository.GetAllAsync(cancellationToken);
        var receiversForSchedules = allReceivers.Where(r => scheduleIds.Contains(r.ScheduledEmailId)).ToList();

        // Fetch all users and contacts to map names and emails
        var users = await userRepository.GetAllAsync(cancellationToken);
        var userMap = users.ToDictionary(u => u.Id);

        var contacts = await clientContactRepository.GetAllAsync(cancellationToken);
        var contactMap = contacts.ToDictionary(c => c.Id);

        var dtos = schedules.Select(s =>
        {
            var scheduleReceivers = receiversForSchedules
                .Where(r => r.ScheduledEmailId == s.Id)
                .Select(r =>
                {
                    userMap.TryGetValue(r.UserId, out var user);
                    ClientContact? contact = null;
                    if (r.ClientContactId.HasValue)
                    {
                        contactMap.TryGetValue(r.ClientContactId.Value, out contact);
                    }

                    return new ScheduledEmailReceiverDto(
                        r.Id,
                        r.UserId,
                        user?.DisplayName ?? "Usuário Desconhecido",
                        user?.Email ?? string.Empty,
                        r.ClientContactId,
                        contact?.Name,
                        contact?.Email
                    );
                })
                .ToList();

            return new ScheduledEmailDto(
                s.Id,
                s.GroupId,
                s.Name,
                s.CronExpression,
                s.Frequency,
                s.SpecificDayOfMonth,
                scheduleReceivers
            );
        }).ToList();

        return Result.Success<IReadOnlyList<ScheduledEmailDto>>(dtos);
    }
}
