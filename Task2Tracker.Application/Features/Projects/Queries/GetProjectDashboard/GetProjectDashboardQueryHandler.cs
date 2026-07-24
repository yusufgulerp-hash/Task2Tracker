using MediatR;
using Microsoft.EntityFrameworkCore;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Projects.DTOs;
using Task2Tracker.Application.Features.Tasks.DTOs;

namespace Task2Tracker.Application.Features.Projects.Queries.GetProjectDashboard;

public sealed class GetProjectDashboardQueryHandler
    : IRequestHandler<GetProjectDashboardQuery, ProjectDashboardDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetProjectDashboardQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ProjectDashboardDto> Handle(
        GetProjectDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .Where(p => p.Id == request.ProjectId)
            .Select(p => new { p.Id, p.Name })
            .FirstOrDefaultAsync(cancellationToken);

        if (project is null)
        {
            throw new NotFoundException("Proje bulunamadı.");
        }

        if (!_currentUser.IsAdmin)
        {
            var isMember = await _context.ProjectMembers.AnyAsync(
                m => m.ProjectId == request.ProjectId && m.UserId == _currentUser.UserId,
                cancellationToken);

            if (!isMember)
            {
                throw new ForbiddenException("Bu projenin panosunu görme yetkiniz yok.");
            }
        }

        // Üyeler ve task'lar ayrı ayrı çekilip aşağıda in-memory gruplanıyor.
        // Tek bir Join+GroupBy sorgusu yerine bunu tercih ettik çünkü bu
        // ölçekte (bir projenin üyeleri/task'ları) performans farkı önemsiz,
        // ama EF Core'un GroupBy'ı SQL'e çevirmesi sık sık sorun çıkarıyor
        // (bir önceki hatamızı hatırla) — iki basit sorgu her zaman çalışır.
        var members = await _context.ProjectMembers
            .Where(m => m.ProjectId == request.ProjectId)
            .Join(
                _context.Users,
                m => m.UserId,
                u => u.Id,
                (m, u) => new { u.Id, u.FirstName, u.LastName, u.Email })
            .OrderBy(x => x.FirstName)
            .ToListAsync(cancellationToken);

        var tasks = await _context.Tasks
            .Where(t => t.ProjectId == request.ProjectId)
            .Select(t => new TaskListItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                BlockerNote = t.BlockerNote,
                Status = t.Status,
                Priority = t.Priority,
                ProjectId = t.ProjectId,
                UserId = t.UserId
            })
            .ToListAsync(cancellationToken);

        var memberIds = members.Select(m => m.Id).ToHashSet();

        var memberDtos = members
            .Select(m => new ProjectMemberWithTasksDto(
                m.Id,
                m.FirstName,
                m.LastName,
                m.Email,
                tasks.Where(t => t.UserId == m.Id).ToList()))
            .ToList();

        // "Sahipsiz" task'lar: hiç kimseye atanmamış olanlar VEYA (task
        // ataması henüz proje üyeliğiyle sınırlandırılmadığı için) projenin
        // artık üyesi olmayan birine atanmış olanlar. Bu ikinci durum,
        // sırada bekleyen "Task tarafı yetkilendirme" maddesi tamamlanınca
        // artık oluşamayacak, ama geçmiş veri için burada da ele alınıyor.
        var unassignedTasks = tasks
            .Where(t => t.UserId is null || !memberIds.Contains(t.UserId.Value))
            .ToList();

        return new ProjectDashboardDto(
            project.Id,
            project.Name,
            memberDtos,
            unassignedTasks);
    }
}
