namespace GreatReports.Domain.Entities;

public abstract class BaseEntity
{
    protected BaseEntity()
    {
        Id = Guid.CreateVersion7();
        CreatedAt = DateTime.UtcNow;
        Active = true;
    }

    public Guid Id { get; protected init; }
    public DateTime CreatedAt { get; protected init; }
    public DateTime? UpdatedAt { get; protected set; }
    public bool Active { get; protected set; }
    public DateTime? UnActivateDate { get; protected set; }

    public virtual void Update()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public virtual void Activate()
    {
        Active = true;
        UnActivateDate = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public virtual void UnActivate()
    {
        Active = false;
        UnActivateDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
