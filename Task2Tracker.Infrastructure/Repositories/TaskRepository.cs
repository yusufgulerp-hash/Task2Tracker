using Microsoft.EntityFrameworkCore;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Domain.Entities;
using Task2Tracker.Infrastructure.Persistence;
using Task2Tracker.Domain.Enums;

namespace Task2Tracker.Infrastructure.Repositories;

public sealed class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskItem>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Tasks
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TaskItem>> SearchAsync(
        string searchText,
        CancellationToken cancellationToken)
    {
        return await _context.Tasks
            .AsNoTracking()
               .Where(x =>
            EF.Functions.ILike(x.Title, $"%{searchText}%") || (x.Description != null &&
             EF.Functions.ILike(x.Description, $"%{searchText}%")))
        .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Tasks
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task Add(TaskItem task)
    {
        await _context.Tasks.AddAsync(task);
    }

    public void Delete(TaskItem task)
    {
        _context.Tasks.Remove(task);
    }

    public async Task<bool> HasOpenTasksByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        return await _context.Tasks
            .AnyAsync(
                x => x.ProjectId == projectId && x.Status != TaskProgressStatus.Done,
                cancellationToken);
    }
}
