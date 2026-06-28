using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.Groups.Commands;

public record CreateGroupCommand(
    Guid GroupLeaderId,
    Guid ClientCompanyId,
    Guid ProjectId,
    string Name,
    string Timezone) : ICommand<Guid>;

public class CreateGroupCommandHandler : ICommandHandler<CreateGroupCommand, Guid>
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClientCompanyRepository _clientCompanyRepository;
    private readonly IProjectRepository _projectRepository;

    public CreateGroupCommandHandler(
        IGroupRepository groupRepository,
        IUserRepository userRepository,
        IClientCompanyRepository clientCompanyRepository,
        IProjectRepository projectRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
        _clientCompanyRepository = clientCompanyRepository;
        _projectRepository = projectRepository;
    }

    public async Task<Result<Guid>> HandleAsync(CreateGroupCommand command, CancellationToken cancellationToken = default)
    {
        var groupLeader = await _userRepository.GetByIdAsync(command.GroupLeaderId, cancellationToken);
        if (groupLeader == null)
        {
            return Result.Failure<Guid>(new Error("User.GroupLeaderNotFound", "Líder de grupo não encontrado."));
        }

        var clientCompany = await _clientCompanyRepository.GetByIdAsync(command.ClientCompanyId, cancellationToken);
        if (clientCompany == null)
        {
            return Result.Failure<Guid>(new Error("ClientCompany.NotFound", "Empresa cliente não encontrada."));
        }

        var project = await _projectRepository.GetByIdAsync(command.ProjectId, cancellationToken);
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
        await _groupRepository.AddAsync(group, cancellationToken);
        await _groupRepository.SaveChangesAsync(cancellationToken);

        return group.Id;
    }
}
