using GreatReports.Shared;

namespace GreatReports.Domain.Entities;

public class Project : BaseEntity
{
    public Guid ClientCompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    // EF Core constructor
    private Project() : base()
    {
    }

    private Project(Guid clientCompanyId, string name, string description) : base()
    {
        ClientCompanyId = clientCompanyId;
        Name = name;
        Description = description;
    }

    public static Result<Project> Create(Guid clientCompanyId, string name, string description)
    {
        if (clientCompanyId == Guid.Empty)
        {
            return Result.Failure<Project>(new Error("Project.InvalidClientCompany", "O ID da empresa cliente associada é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Project>(new Error("Project.InvalidName", "O nome do projeto é obrigatório."));
        }

        return new Project(clientCompanyId, name, description ?? string.Empty);
    }
}
