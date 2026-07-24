using Task2Tracker.Domain.Entities;

namespace Task2Tracker.Application.Interfaces.Repositories;

public interface IProjectRepository
{
    Task<IReadOnlyList<Project>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Project?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Project?> GetByIdWithMembersAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Project>> SearchAsync(
        string text,
        CancellationToken cancellationToken = default);

    Task<Project?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    void Add(Project project);

    void Delete(Project project);
}