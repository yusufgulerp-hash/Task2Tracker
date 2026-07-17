using Task2Tracker.Domain.Entities;

namespace Task2Tracker.Application.Common.Interfaces;

public interface ITaskRepository
{
    Task<List<TaskItem>> GetAllAsync(CancellationToken cancellationToken);

    Task<List<TaskItem>> SearchAsync(
        string searchText,
        CancellationToken cancellationToken);

    Task<TaskItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task Add(TaskItem task);

    void Delete(TaskItem task);

    Task<bool> HasOpenTasksByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken);
}