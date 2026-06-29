using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Groups.Commands;
using GreatReports.Domain.Entities;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.Groups.CommandHandlers;

public class CreateGroupCommandHandler(
    IGroupRepository groupRepository,
    IUserRepository userRepository,
    IClientCompanyRepository clientCompanyRepository,
    IProjectRepository projectRepository) : ICommandHandler<CreateGroupCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CreateGroupCommand command, CancellationToken cancellationToken = default)
    {
        var groupLeader = await userRepository.GetByIdAsync(command.GroupLeaderId, cancellationToken);
        if (groupLeader == null)
        {
            return Result.Failure<Guid>(new Error("User.GroupLeaderNotFound", "Líder de grupo não encontrado."));
        }

        var clientCompany = await clientCompanyRepository.GetByIdAsync(command.ClientCompanyId, cancellationToken);
        if (clientCompany == null)
        {
            return Result.Failure<Guid>(new Error("ClientCompany.NotFound", "Empresa cliente não encontrada."));
        }

        var project = await projectRepository.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project == null)
        {
            return Result.Failure<Guid>(new Error("Project.NotFound", "Projeto não encontrado."));
        }

        var entityResult = Group.Create(
            command.GroupLeaderId,
            command.ClientCompanyId,
            command.ProjectId,
            command.Name,
            command.Timezone);

        if (entityResult.IsFailure)
        {
            return Result.Failure<Guid>(entityResult.Error);
        }

        var group = entityResult.Value;
        await groupRepository.AddAsync(group, cancellationToken);
        await groupRepository.SaveChangesAsync(cancellationToken);

        return group.Id;
    }
}
