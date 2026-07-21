using Microsoft.EntityFrameworkCore;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Domain.Entities;
using Task2Tracker.Infrastructure.Persistence;

namespace Task2Tracker.Infrastructure.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
    }

    public async Task RevokeAllUserTokensAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        // Tek sorgu ile tüm aktif token'ları iptal et
        // UPDATE "RefreshTokens" SET "IsRevoked" = true WHERE "UserId" = @userId
        await _context.RefreshTokens
            .Where(x => x.UserId == userId && !x.IsRevoked)
            .ExecuteUpdateAsync(
                s => s.SetProperty(x => x.IsRevoked, true),
                cancellationToken);
    }

    public void Add(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
    }
}