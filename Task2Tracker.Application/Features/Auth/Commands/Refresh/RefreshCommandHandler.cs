using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Auth.DTOs;
using Task2Tracker.Domain.Entities;

namespace Task2Tracker.Application.Features.Auth.Commands.Refresh;

public sealed class RefreshCommandHandler
    : IRequestHandler<RefreshCommand, AuthResponseDto>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IApplicationDbContext _context;

    public RefreshCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IJwtService jwtService,
        IApplicationDbContext context)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
        _context = context;
    }

    public async Task<AuthResponseDto> Handle(
        RefreshCommand request,
        CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenRepository
            .GetByTokenAsync(request.RefreshToken, cancellationToken);

        if (refreshToken is null)
            throw new UnauthorizedException("Geçersiz refresh token.");

        if (refreshToken.IsRevoked)
        {
            await _refreshTokenRepository.RevokeAllUserTokensAsync(
                refreshToken.UserId, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedException(
                "Güvenlik ihlali tespit edildi. Lütfen tekrar giriş yapın.");
        }

        if (refreshToken.IsExpired)
            throw new UnauthorizedException(
                "Refresh token süresi dolmuş. Lütfen tekrar giriş yapın.");

        refreshToken.Revoke();

        var newAccessToken = _jwtService.GenerateAccessToken(refreshToken.User);
        var newRefreshTokenString = _jwtService.GenerateRefreshToken();
        var newRefreshToken = new RefreshToken(
            newRefreshTokenString,
            refreshToken.UserId);

        _refreshTokenRepository.Add(newRefreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(
            newAccessToken,
            newRefreshTokenString,
            refreshToken.UserId,
            refreshToken.User.Email,
            refreshToken.User.Role.ToString());
    }
}