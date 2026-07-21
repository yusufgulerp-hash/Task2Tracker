using MediatR;
using Task2Tracker.Application.Common.Interfaces;

namespace Task2Tracker.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IApplicationDbContext _context;

    public LogoutCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IApplicationDbContext context)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _context = context;
    }

    public async Task Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        // Kullanıcının tüm refresh token'larını iptal et
        await _refreshTokenRepository.RevokeAllUserTokensAsync(
            request.UserId, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}