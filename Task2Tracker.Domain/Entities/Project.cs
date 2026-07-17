using Task2Tracker.Domain.Common;

namespace Task2Tracker.Domain.Entities;

public class Project : BaseEntity
{
    private readonly List<TaskItem> _tasks = new();

    public string Name { get; private set; } = null!;

    public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

    protected Project() { }

    public Project(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Proje adı boş bırakılamaz.", nameof(name));

        Id = Guid.NewGuid();
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Updated project name cannot be empty.", nameof(name));

        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }
}