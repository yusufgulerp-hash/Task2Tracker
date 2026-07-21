using Task2Tracker.Domain.Entities;

namespace Task2Tracker.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken);

    Task RevokeAllUserTokensAsync(
        Guid userId,
        CancellationToken cancellationToken);

    void Add(RefreshToken refreshToken);
}