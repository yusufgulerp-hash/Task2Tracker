using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;
using Task2Tracker.Application.Features.Auth.DTOs;
using Task2Tracker.Domain.Enums;

namespace Task2Tracker.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(
            request.Email,
            cancellationToken);

        if (user is null ||
            !_passwordService.VerifyPassword(
                request.Password,
                user.PasswordHash))
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

        var accessToken = _jwtService.GenerateAccessToken(user);

        return new AuthResponseDto(
            accessToken,
            user.Id,
            user.Email,
            user.Role.ToString());
    }
}