using Microsoft.EntityFrameworkCore;
using Task2Tracker.Application.Interfaces.Repositories;
using Task2Tracker.Domain.Entities;
using Task2Tracker.Infrastructure.Persistence;

namespace Task2Tracker.Infrastructure.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Project>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Project?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> SearchAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        text = text.Trim();

        return await _context.Projects
            .AsNoTracking()
            .Where(x => x.Name.Contains(text))
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Project?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .FirstOrDefaultAsync(
                x => x.Name == name,
                cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .AnyAsync(
                x => x.Name == name,
                cancellationToken);
    }

    public void Add(Project project)
    {
        _context.Projects.Add(project);
    }

    public void Delete(Project project)
    {
        _context.Projects.Remove(project);
    }
}