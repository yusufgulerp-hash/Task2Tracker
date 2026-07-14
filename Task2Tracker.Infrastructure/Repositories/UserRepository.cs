using Microsoft.EntityFrameworkCore;
using Task2Tracker.Application.Interfaces.Repositories;
using Task2Tracker.Domain.Entities;
using Task2Tracker.Infrastructure.Persistence;

namespace Task2Tracker.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<User>> SearchAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(user =>
                EF.Functions.ILike(user.FirstName, $"%{text}%") ||
                EF.Functions.ILike(user.LastName, $"%{text}%") ||
                EF.Functions.ILike(user.Email, $"%{text}%"))
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(
                x => x.Email == email,
                cancellationToken);
    }

    public void Add(User user)
    {
        _context.Users.Add(user);
    }
    public void Delete(User user)
    {
        _context.Users.Remove(user);
    }

    public async Task<bool> ExistsByEmailAsync(
     string email,
     CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(
            user => user.Email == email,
            cancellationToken);
    }
}