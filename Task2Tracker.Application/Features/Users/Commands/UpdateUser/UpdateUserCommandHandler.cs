using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler
    : IRequestHandler<UpdateUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        // Bir kullanıcı yalnızca kendi profilini güncelleyebilir;
        // Admin ise herkesi güncelleyebilir.
        if (!_currentUser.IsAdmin && _currentUser.UserId != request.Id)
        {
            throw new ForbiddenException(
                "Başka bir kullanıcının bilgilerini güncelleme yetkiniz yok.");
        }

        var user = await _userRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        if (!string.Equals(
                user.Email,
                request.Email,
                StringComparison.OrdinalIgnoreCase))
        {
            var existingUser = await _userRepository.GetByEmailAsync(
                request.Email,
                cancellationToken);

            if (existingUser is not null &&
                existingUser.Id != user.Id)
            {
                throw new ConflictException(
                    "This email in use.");
            }
        }

        user.UpdateDetails(
            request.FirstName,
            request.LastName,
            request.Email);

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}