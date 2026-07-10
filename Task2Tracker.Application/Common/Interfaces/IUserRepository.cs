using Task2Tracker.Domain.Entities;

namespace Task2Tracker.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);

    void Add(User user);
}