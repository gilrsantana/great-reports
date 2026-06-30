using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.DailyActivities.Commands;
using GreatReports.Domain.Entities;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.DailyActivities.CommandHandlers;

public class CreateDailyActivityCommandHandler(
    IDailyActivityRepository dailyActivityRepository,
    IGroupRepository groupRepository) : ICommandHandler<CreateDailyActivityCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CreateDailyActivityCommand command, CancellationToken cancellationToken = default)
    {
        var groups = await groupRepository.GetGroupsByPartnerIdAsync(command.PartnerId, cancellationToken);
        foreach (var group in groups)
        {
            var timezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(group.Timezone);
            var localTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezoneInfo);
            var localRefDate = command.ReferenceDate.Date;

            if (localTimeNow.Date > localRefDate || 
                (localTimeNow.Date == localRefDate && (localTimeNow.Hour > 23 || (localTimeNow.Hour == 23 && localTimeNow.Minute >= 50))))
            {
                return Result.Failure<Guid>(new Error("DailyActivity.LockoutActive", "Não é permitido registrar ou alterar atividades após as 23:50 no fuso horário do grupo."));
            }
        }

        var entityResult = DailyActivity.Create(
            command.PartnerId,
            command.Title,
            command.Theme,
            command.Content,
            command.ReferenceDate,
            command.Status,
            command.IsBlocked);

        if (entityResult.IsFailure)
        {
            return Result.Failure<Guid>(entityResult.Error);
        }

        var activity = entityResult.Value;
        await dailyActivityRepository.AddAsync(activity, cancellationToken);
        await dailyActivityRepository.SaveChangesAsync(cancellationToken);

        return activity.Id;
    }
}
