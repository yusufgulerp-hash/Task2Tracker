using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Users.Commands.DeleteUser;

public sealed class DeleteUserCommandHandler
    : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;

    public DeleteUserCommandHandler(
        IUserRepository userRepository,
        IApplicationDbContext context)
    {
        _userRepository = userRepository;
        _context = context;
    }

    public async Task Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User not found.");
        }

        _userRepository.Delete(user);

        await _context.SaveChangesAsync(cancellationToken);
    }
}