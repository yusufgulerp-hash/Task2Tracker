using Task2Tracker.Domain.Common;

namespace Task2Tracker.Domain.Entities;

public class ProjectMember : BaseEntity
{
    public Guid ProjectId { get; private set; }
    public Guid UserId { get; private set; }

    protected ProjectMember() { }

    // internal: bir ProjectMember yalnızca Project aggregate'i tarafından,
    // Project.AddMember(...) üzerinden oluşturulabilir. Dışarıdan
    // "new ProjectMember(...)" ile keyfi bir üyelik kaydı yaratılamaz —
    // invariant'ın (aynı kullanıcı bir projeye iki kez üye olamaz) tek
    // giriş noktası Project'tir.
    internal ProjectMember(Guid projectId, Guid userId)
    {
        if (projectId == Guid.Empty)
            throw new ArgumentException("Geçerli bir proje ID'si belirtilmelidir.", nameof(projectId));

        if (userId == Guid.Empty)
            throw new ArgumentException("Geçerli bir kullanıcı ID'si belirtilmelidir.", nameof(userId));

        Id = Guid.NewGuid();
        ProjectId = projectId;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }
}
