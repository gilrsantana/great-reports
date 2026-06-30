using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.DailyActivities.Commands;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.DailyActivities.CommandHandlers;

public class UpdateDailyActivityCommandHandler(
    IDailyActivityRepository dailyActivityRepository,
    IGroupRepository groupRepository) : ICommandHandler<UpdateDailyActivityCommand>
{
    public async Task<Result> HandleAsync(UpdateDailyActivityCommand command, CancellationToken cancellationToken = default)
    {
        var activity = await dailyActivityRepository.GetByIdAsync(command.Id, cancellationToken);
        if (activity == null || activity.PartnerId != command.PartnerId)
        {
            return Result.Failure(new Error("DailyActivity.NotFound", "Atividade não encontrada."));
        }

        // Lockout rule checks
        var groups = await groupRepository.GetGroupsByPartnerIdAsync(command.PartnerId, cancellationToken);
        foreach (var group in groups)
        {
            var timezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(group.Timezone);
            var localTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezoneInfo);
            var localRefDate = command.ReferenceDate.Date;

            if (localTimeNow.Date > localRefDate || 
                (localTimeNow.Date == localRefDate && (localTimeNow.Hour > 23 || (localTimeNow.Hour == 23 && localTimeNow.Minute >= 50))))
            {
                return Result.Failure(new Error("DailyActivity.LockoutActive", "Não é permitido registrar ou alterar atividades após as 23:50 no fuso horário do grupo."));
            }
        }

        var updateResult = activity.Update(
            command.Title,
            command.Theme,
            command.Content,
            command.ReferenceDate,
            command.Status,
            command.IsBlocked);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        dailyActivityRepository.Update(activity);
        await dailyActivityRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
