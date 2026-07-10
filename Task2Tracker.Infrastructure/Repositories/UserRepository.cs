using Task2Tracker.Application.Interfaces.Repositories;
using Task2Tracker.Domain.Entities;
using Task2Tracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Task2Tracker.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }

    public void Add(User user)
    {
        _context.Users.Add(user);
    }
}