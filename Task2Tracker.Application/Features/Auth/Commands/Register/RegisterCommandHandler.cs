using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Auth.DTOs;
using Task2Tracker.Application.Interfaces.Repositories;
using Task2Tracker.Domain.Entities;

namespace Task2Tracker.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, RegisterResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly IApplicationDbContext _context;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IApplicationDbContext context)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _context = context;
    }

    public async Task<RegisterResponseDto> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(
                request.Email,
                cancellationToken))
        {
            throw new ConflictException(
                "Bu e-posta adresi zaten kullanılmaktadır.");
        }

        var passwordHash =
            _passwordService.HashPassword(request.Password);

        var user = new User(
            request.FirstName,
            request.LastName,
            request.Email,
            passwordHash);

        _userRepository.Add(user);

        await _context.SaveChangesAsync(cancellationToken);

        return new RegisterResponseDto(
            user.Id,
            user.Email,
            user.Status.ToString(),
            "Kayıt başarılı. Hesabınız admin onayı bekliyor.");
    }
}