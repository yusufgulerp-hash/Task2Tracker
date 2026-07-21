using Task2Tracker.Domain.Entities;
using Task2Tracker.Domain.Enums;

namespace Task2Tracker.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> SearchAsync(
        string text,
        CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByStatusAsync(
    UserStatus status,
    CancellationToken cancellationToken = default);

    void Add(User user);

    void Delete(User user);

    Task<bool> ExistsByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);
}