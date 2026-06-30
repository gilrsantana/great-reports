using GreatReports.Domain.Enums;
using GreatReports.Shared;

namespace GreatReports.Domain.Entities;

public class DailyActivity : BaseEntity
{
    public Guid PartnerId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Theme { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public DateTime ReferenceDate { get; private set; }
    public ActivityStatus Status { get; private set; }
    public bool IsBlocked { get; private set; }
    public bool IsPublished { get; private set; }
    public string? SummarizedContent { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    // EF Core constructor
    private DailyActivity() : base()
    {
    }

    private DailyActivity(Guid partnerId, string title, string theme, string content, DateTime referenceDate, ActivityStatus status, bool isBlocked) : base()
    {
        PartnerId = partnerId;
        Title = title;
        Theme = theme;
        Content = content;
        ReferenceDate = referenceDate;
        Status = status;
        IsBlocked = isBlocked;
        IsPublished = false;
    }

    public static Result<DailyActivity> Create(Guid partnerId, string title, string theme, string content, DateTime referenceDate, ActivityStatus status, bool isBlocked)
    {
        if (partnerId == Guid.Empty)
        {
            return Result.Failure<DailyActivity>(new Error("DailyActivity.InvalidPartner", "O ID do parceiro associado é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure<DailyActivity>(new Error("DailyActivity.InvalidTitle", "O título da atividade é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(theme))
        {
            return Result.Failure<DailyActivity>(new Error("DailyActivity.InvalidTheme", "O tema da atividade é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            return Result.Failure<DailyActivity>(new Error("DailyActivity.InvalidContent", "O conteúdo detalhado da atividade é obrigatório."));
        }

        if (referenceDate == default)
        {
            return Result.Failure<DailyActivity>(new Error("DailyActivity.InvalidReferenceDate", "A data de referência da atividade é obrigatória."));
        }

        return new DailyActivity(partnerId, title, theme, content, referenceDate, status, isBlocked);
    }

    public Result Update(string title, string theme, string content, DateTime referenceDate, ActivityStatus status, bool isBlocked)
    {
        if (IsPublished)
        {
            return Result.Failure(new Error("DailyActivity.AlreadyPublished", "Esta atividade já foi publicada e não pode ser editada."));
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure(new Error("DailyActivity.InvalidTitle", "O título da atividade é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(theme))
        {
            return Result.Failure(new Error("DailyActivity.InvalidTheme", "O tema da atividade é obrigatório."));
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            return Result.Failure(new Error("DailyActivity.InvalidContent", "O conteúdo detalhado da atividade é obrigatório."));
        }

        if (referenceDate == default)
        {
            return Result.Failure(new Error("DailyActivity.InvalidReferenceDate", "A data de referência da atividade é obrigatória."));
        }

        Title = title;
        Theme = theme;
        Content = content;
        ReferenceDate = referenceDate;
        Status = status;
        IsBlocked = isBlocked;

        base.Update();
        return Result.Success();
    }

    public void Publish()
    {
        IsPublished = true;
        base.Update();
    }

    public void SetProcessed(string summarizedContent)
    {
        SummarizedContent = summarizedContent;
        ProcessedAt = DateTime.UtcNow;
        base.Update();
    }
}
