using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.ScheduledEmails.Commands;
using GreatReports.Domain.Entities;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.ScheduledEmails.CommandHandlers;

public class CreateScheduledEmailCommandHandler(
    IScheduledEmailRepository scheduledEmailRepository,
    IGroupRepository groupRepository) : ICommandHandler<CreateScheduledEmailCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CreateScheduledEmailCommand command, CancellationToken cancellationToken = default)
    {
        var group = await groupRepository.GetByIdAsync(command.GroupId, cancellationToken);
        if (group == null)
        {
            return Result.Failure<Guid>(new Error("Group.NotFound", "Grupo não encontrado."));
        }

        var result = ScheduledEmail.Create(
            command.GroupId,
            command.Name,
            command.CronExpression,
            command.Frequency,
            command.SpecificDayOfMonth);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        var scheduledEmail = result.Value;
        await scheduledEmailRepository.AddAsync(scheduledEmail, cancellationToken);
        await scheduledEmailRepository.SaveChangesAsync(cancellationToken);

        return scheduledEmail.Id;
    }
}
