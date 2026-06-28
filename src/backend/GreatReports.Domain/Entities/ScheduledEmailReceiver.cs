using GreatReports.Shared;

namespace GreatReports.Domain.Entities;

public class ScheduledEmailReceiver : BaseEntity
{
    public Guid ScheduledEmailId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? ClientContactId { get; private set; }

    // EF Core constructor
    private ScheduledEmailReceiver() : base()
    {
    }

    private ScheduledEmailReceiver(Guid scheduledEmailId, Guid userId, Guid? clientContactId) : base()
    {
        ScheduledEmailId = scheduledEmailId;
        UserId = userId;
        ClientContactId = clientContactId;
    }

    public static Result<ScheduledEmailReceiver> Create(Guid scheduledEmailId, Guid userId, Guid? clientContactId = null)
    {
        if (scheduledEmailId == Guid.Empty)
        {
            return Result.Failure<ScheduledEmailReceiver>(new Error("ScheduledEmailReceiver.InvalidScheduledEmail", "O ID do agendamento de email associado é obrigatório."));
        }

        if (userId == Guid.Empty)
        {
            return Result.Failure<ScheduledEmailReceiver>(new Error("ScheduledEmailReceiver.InvalidUser", "O ID do usuário destinatário é obrigatório."));
        }

        return new ScheduledEmailReceiver(scheduledEmailId, userId, clientContactId);
    }
}
