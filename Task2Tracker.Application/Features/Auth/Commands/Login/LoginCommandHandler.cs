using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;
using Task2Tracker.Application.Features.Auth.DTOs;
using Task2Tracker.Domain.Entities;
using Task2Tracker.Domain.Enums;

namespace Task2Tracker.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly IApplicationDbContext _context;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordService passwordService,
        IJwtService jwtService,
        IApplicationDbContext context)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _context = context;
    }

    public async Task<AuthResponseDto> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(
            request.Email, cancellationToken);

        if (user is null || !_passwordService.VerifyPassword(
                request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException(
                "E-posta veya şifre hatalı.");
        }

        if (user.Status != UserStatus.Active)
        {
            throw new UnauthorizedException(
                user.Status switch
                {
                    UserStatus.PendingApproval =>
                        "Hesabınız admin onayı bekliyor.",

                    UserStatus.Rejected =>
                        "Hesabınız reddedildi.",

                    UserStatus.Blocked =>
                        "Hesabınız bloke edildi.",

                    _ =>
                        "Hesabınız aktif değil."
                });
        }

        await _refreshTokenRepository.RevokeAllUserTokensAsync(
            user.Id, cancellationToken);

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshTokenString = _jwtService.GenerateRefreshToken();
        var refreshToken = new RefreshToken(refreshTokenString, user.Id);

        _refreshTokenRepository.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(
            accessToken,
            refreshTokenString,
            user.Id,
            user.Email,
            user.Role.ToString());
    }
}