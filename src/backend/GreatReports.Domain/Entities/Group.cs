using GreatReports.Shared;

namespace GreatReports.Domain.Entities;

public class Group : BaseEntity
{
    public Guid GroupLeaderId { get; private set; }
    public Guid ClientCompanyId { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Timezone { get; private set; } = string.Empty;

    // EF Core constructor
    private Group() : base()
    {
    }

    private Group(Guid groupLeaderId, Guid clientCompanyId, Guid projectId, string name, string timezone) : base()
    {
        GroupLeaderId = groupLeaderId;
        ClientCompanyId = clientCompanyId;
        ProjectId = projectId;
        Name = name;
        Timezone = timezone;
    }

    public static Result<Group> Create(Guid groupLeaderId, Guid clientCompanyId, Guid projectId, string name, string timezone)
    {
        if (groupLeaderId == Guid.Empty)
        {
            return Result.Failure<Group>(new Error("Group.InvalidGroupLeader", "O ID do líder do grupo é obrigatório."));
        }

        if (clientCompanyId == Guid.Empty)
        {
            return Result.Failure<Group>(new Error("Group.InvalidClientCompany", "O ID da empresa cliente associada é obrigatório."));
        }

        if (projectId == Guid.Empty)
        {
            return Result.Failure<Group>(new Error("Group.InvalidProject", "O ID do projeto associado é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Group>(new Error("Group.InvalidName", "O nome do grupo é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(timezone))
        {
            return Result.Failure<Group>(new Error("Group.InvalidTimezone", "O fuso horário é obrigatório."));
        }

        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timezone);
        }
        catch (Exception)
        {
            return Result.Failure<Group>(new Error("Group.TimezoneNotFound", "O fuso horário fornecido é inválido ou não foi encontrado."));
        }

        return new Group(groupLeaderId, clientCompanyId, projectId, name, timezone);
    }
}
