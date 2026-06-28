using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.Projects.Commands;

public record RegisterProjectCommand(Guid ClientCompanyId, string Name, string Description) : ICommand<Guid>;

public class RegisterProjectCommandHandler : ICommandHandler<RegisterProjectCommand, Guid>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IClientCompanyRepository _clientCompanyRepository;

    public RegisterProjectCommandHandler(
        IProjectRepository projectRepository,
        IClientCompanyRepository clientCompanyRepository)
    {
        _projectRepository = projectRepository;
        _clientCompanyRepository = clientCompanyRepository;
    }

    public async Task<Result<Guid>> HandleAsync(RegisterProjectCommand command, CancellationToken cancellationToken = default)
    {
        var clientCompany = await _clientCompanyRepository.GetByIdAsync(command.ClientCompanyId, cancellationToken);
        if (clientCompany == null)
        {
            return Result.Failure<Guid>(new Error("ClientCompany.NotFound", "Empresa cliente não encontrada."));
        }

        var entityResult = Project.Create(command.ClientCompanyId, command.Name, command.Description);
        if (entityResult.IsFailure)
        {
            return Result.Failure<Guid>(entityResult.Error);
        }

        var project = entityResult.Value;
        await _projectRepository.AddAsync(project, cancellationToken);
        await _projectRepository.SaveChangesAsync(cancellationToken);

        return project.Id;
    }
}
