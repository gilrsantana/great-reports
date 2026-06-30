using GreatReports.Application.Common.CQRS;

namespace GreatReports.Application.UseCases.ScheduledEmails.Commands;

public record AddScheduledEmailReceiverCommand(
    Guid ScheduledEmailId,
    Guid UserId,
    Guid? ClientContactId) : ICommand;
