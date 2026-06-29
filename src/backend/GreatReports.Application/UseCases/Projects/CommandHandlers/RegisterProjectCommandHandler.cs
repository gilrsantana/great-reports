using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Projects.Commands;
using GreatReports.Domain.Entities;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.Projects.CommandHandlers;

public class RegisterProjectCommandHandler(
    IProjectRepository projectRepository,
    IClientCompanyRepository clientCompanyRepository) : ICommandHandler<RegisterProjectCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(RegisterProjectCommand command, CancellationToken cancellationToken = default)
    {
        var clientCompany = await clientCompanyRepository.GetByIdAsync(command.ClientCompanyId, cancellationToken);
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
        await projectRepository.AddAsync(project, cancellationToken);
        await projectRepository.SaveChangesAsync(cancellationToken);

        return project.Id;
    }
}
