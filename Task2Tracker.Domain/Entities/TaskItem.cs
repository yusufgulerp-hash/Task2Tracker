using Task2Tracker.Domain.Common;
using Task2Tracker.Domain.Enums;

namespace Task2Tracker.Domain.Entities;

public class TaskItem : BaseEntity
{
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public TaskProgressStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }

    // Görevi ilerletmeyi engelleyen bir durum varsa (birinden onay
    // bekleniyor, bir bağımlılık tamamlanmadı vb.) buraya not düşülür.
    public string? BlockerNote { get; private set; }

    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;

    public Guid? UserId { get; private set; }
    public User? User { get; private set; }

    protected TaskItem() { }

    public TaskItem(string title, string? description, TaskPriority priority, Guid projectId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Görev başlığı boş bırakılamaz.", nameof(title));

        if (projectId == Guid.Empty)
            throw new ArgumentException("Görev mutlaka geçerli bir projeye bağlı olmalıdır.", nameof(projectId));

        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Priority = priority;
        ProjectId = projectId;
        Status = TaskProgressStatus.ToDo;
        CreatedAt = DateTime.UtcNow;
    }

    public void AssignUser(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Göreve atanacak geçerli bir kullanıcı ID'si belirtilmelidir.", nameof(userId));

        UserId = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UnassignUser()
    {
        UserId = null;
        User = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(TaskProgressStatus newStatus)
    {
        if (Status == newStatus) return;

        switch (Status)
        {
            case TaskProgressStatus.ToDo:
                if (newStatus == TaskProgressStatus.Done)
                {
                    throw new InvalidOperationException($"Geçersiz Durum Geçişi: '{Status}' durumundaki bir görev, üzerinde çalışılmadan doğrudan '{newStatus}' yapılamaz.");
                }
                break;

            case TaskProgressStatus.InProgress:
                break;

            case TaskProgressStatus.Done:
                if (newStatus == TaskProgressStatus.ToDo)
                {
                    throw new InvalidOperationException($"Geçersiz Durum Geçişi: Yeniden açılan '{Status}' bir görev doğrudan '{newStatus}' aşamasına alınamaz. Önce '{TaskProgressStatus.InProgress}' yapılmalıdır.");
                }
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(newStatus), "Tanımlanmayan bir görev durumu algılandı.");
        }

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Güncellenen görev başlığı boş olamaz.", nameof(title));

        Title = title;
        Description = description;

        if (Status == TaskProgressStatus.Done)
        {
            Status = TaskProgressStatus.InProgress;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePriority(TaskPriority newPriority)
    {
        Priority = newPriority;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetBlocker(string? blockerNote)
    {
        // Boş/whitespace bir metin gelirse "blocker yok" anlamına gelsin —
        // client'ın boş string mi null mu gönderdiğini ayırt etmesine
        // gerek kalmasın.
        BlockerNote = string.IsNullOrWhiteSpace(blockerNote) ? null : blockerNote;
        UpdatedAt = DateTime.UtcNow;
    }
}