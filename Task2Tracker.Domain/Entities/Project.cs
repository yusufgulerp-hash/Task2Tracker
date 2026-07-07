using Task2Tracker.Domain.Common;

namespace Task2Tracker.Domain.Entities;

public class Project : BaseEntity
{
    private readonly List<User> _members = new();
    private readonly List<TaskItem> _tasks = new();

    public string Name { get; private set; } = null!;

    public IReadOnlyCollection<User> Members => _members.AsReadOnly();
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

    public void AddMember(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user), "Eklenecek kullanıcı boş olamaz.");

        if (_members.Any(m => m.Id == user.Id))
            return;

        _members.Add(user);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveMember(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Geçerli bir kullanıcı ID'si belirtilmelidir.", nameof(userId));

        var member = _members.FirstOrDefault(m => m.Id == userId);
        if (member != null)
        {
            _members.Remove(member);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void UpdateDetails(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Güncellenen proje adı boş olamaz.", nameof(name));

        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }
}