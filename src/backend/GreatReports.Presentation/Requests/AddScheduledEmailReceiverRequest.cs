namespace GreatReports.Presentation.Requests;

public record AddScheduledEmailReceiverRequest(
    Guid UserId,
    Guid? ClientContactId);
