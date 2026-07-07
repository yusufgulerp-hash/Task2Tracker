using Task2Tracker.Domain.Common;

namespace Task2Tracker.Domain.Entities;

public class User : BaseEntity
{
    private readonly List<Project> _projects = new();
    private readonly List<TaskItem> _tasks = new();

    public string Username { get; private set; } = null!;
    public string Email { get; private set; } = null!;

    public IReadOnlyCollection<Project> Projects => _projects.AsReadOnly();
    public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

    protected User() { }

    public User(string username, string email)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Kullanıcı adı boş bırakılamaz.", nameof(username));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-posta adresi boş bırakılamaz.", nameof(email));

        Id = Guid.NewGuid();
        Username = username;
        Email = email;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string username, string email)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Güncellenen kullanıcı adı boş olamaz.", nameof(username));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Güncellenen e-posta adresi boş olamaz.", nameof(email));

        Username = username;
        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }
}