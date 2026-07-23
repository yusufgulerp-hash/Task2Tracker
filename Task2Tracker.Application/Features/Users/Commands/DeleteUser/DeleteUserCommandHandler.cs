using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Users.Commands.DeleteUser;

public sealed class DeleteUserCommandHandler
    : IRequestHandler<DeleteUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteUserCommandHandler(
        IUserRepository userRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        // Kullanıcı silme, kendi hesabını silme özelliği tasarlanmadığı için
        // (şimdilik) yalnızca Admin'e açık bir işlem.
        if (!_currentUser.IsAdmin)
        {
            throw new ForbiddenException(
                "Kullanıcı silme yetkiniz yok.");
        }

        var user = await _userRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        _userRepository.Delete(user);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}