using GreatReports.Domain.Enums;
using GreatReports.Shared;

namespace GreatReports.Domain.Entities;

public class ScheduledEmail : BaseEntity
{
    public Guid GroupId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string CronExpression { get; private set; } = string.Empty;
    public ReportFrequency Frequency { get; private set; }
    public int? SpecificDayOfMonth { get; private set; }

    // EF Core constructor
    private ScheduledEmail() : base()
    {
    }

    private ScheduledEmail(Guid groupId, string name, string cronExpression, ReportFrequency frequency, int? specificDayOfMonth) : base()
    {
        GroupId = groupId;
        Name = name;
        CronExpression = cronExpression;
        Frequency = frequency;
        SpecificDayOfMonth = specificDayOfMonth;
    }

    public static Result<ScheduledEmail> Create(Guid groupId, string name, string cronExpression, ReportFrequency frequency, int? specificDayOfMonth = null)
    {
        if (groupId == Guid.Empty)
        {
            return Result.Failure<ScheduledEmail>(new Error("ScheduledEmail.InvalidGroup", "O ID do grupo associado é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<ScheduledEmail>(new Error("ScheduledEmail.InvalidName", "O nome do agendamento é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(cronExpression))
        {
            return Result.Failure<ScheduledEmail>(new Error("ScheduledEmail.InvalidCron", "A expressão cron do agendamento é obrigatória."));
        }

        if (frequency == ReportFrequency.SpecificDay)
        {
            if (!specificDayOfMonth.HasValue || specificDayOfMonth.Value < 1 || specificDayOfMonth.Value > 31)
            {
                return Result.Failure<ScheduledEmail>(new Error("ScheduledEmail.InvalidSpecificDay", "Para a frequência de dia específico, o dia do mês deve estar entre 1 e 31."));
            }
        }

        return new ScheduledEmail(groupId, name, cronExpression, frequency, specificDayOfMonth);
    }
}
