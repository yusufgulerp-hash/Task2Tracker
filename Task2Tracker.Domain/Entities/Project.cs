using Task2Tracker.Domain.Common;

namespace Task2Tracker.Domain.Entities;

public class Project : BaseEntity
{
    private readonly List<TaskItem> _tasks = new();
    private readonly List<ProjectMember> _members = new();

    public string Name { get; private set; } = null!;

    public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

    public IReadOnlyCollection<ProjectMember> Members => _members.AsReadOnly();

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

    public bool IsMember(Guid userId) => _members.Any(m => m.UserId == userId);

    public void AddMember(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Geçerli bir kullanıcı ID'si belirtilmelidir.", nameof(userId));

        if (IsMember(userId))
            throw new InvalidOperationException("Bu kullanıcı zaten projenin üyesi.");

        _members.Add(new ProjectMember(Id, userId));
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveMember(Guid userId)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId);

        if (member is null)
            throw new InvalidOperationException("Bu kullanıcı projenin üyesi değil.");

        _members.Remove(member);
        UpdatedAt = DateTime.UtcNow;
    }
}