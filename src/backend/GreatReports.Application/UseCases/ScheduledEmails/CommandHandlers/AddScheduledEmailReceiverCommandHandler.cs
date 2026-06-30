using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.ScheduledEmails.Commands;
using GreatReports.Domain.Entities;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.ScheduledEmails.CommandHandlers;

public class AddScheduledEmailReceiverCommandHandler(
    IScheduledEmailReceiverRepository receiverRepository,
    IScheduledEmailRepository scheduledEmailRepository,
    IUserRepository userRepository,
    IClientContactRepository clientContactRepository) : ICommandHandler<AddScheduledEmailReceiverCommand>
{
    public async Task<Result> HandleAsync(AddScheduledEmailReceiverCommand command, CancellationToken cancellationToken = default)
    {
        var scheduledEmail = await scheduledEmailRepository.GetByIdAsync(command.ScheduledEmailId, cancellationToken);
        if (scheduledEmail == null)
        {
            return Result.Failure(new Error("ScheduledEmail.NotFound", "Agendamento de e-mail não encontrado."));
        }

        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user == null)
        {
            return Result.Failure(new Error("User.NotFound", "Usuário destinatário não encontrado."));
        }

        if (command.ClientContactId.HasValue)
        {
            var contact = await clientContactRepository.GetByIdAsync(command.ClientContactId.Value, cancellationToken);
            if (contact == null)
            {
                return Result.Failure(new Error("ClientContact.NotFound", "Contato cliente não encontrado."));
            }
        }

        var result = ScheduledEmailReceiver.Create(
            command.ScheduledEmailId,
            command.UserId,
            command.ClientContactId);

        if (result.IsFailure)
        {
            return result;
        }

        var receiver = result.Value;
        await receiverRepository.AddAsync(receiver, cancellationToken);
        await receiverRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
