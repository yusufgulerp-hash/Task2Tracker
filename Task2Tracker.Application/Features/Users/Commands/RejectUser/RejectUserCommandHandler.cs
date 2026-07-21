using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Users.Commands.RejectUser;

public sealed class RejectUserCommandHandler
    : IRequestHandler<RejectUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;

    public RejectUserCommandHandler(
        IUserRepository userRepository,
        IApplicationDbContext context)
    {
        _userRepository = userRepository;
        _context = context;
    }

    public async Task Handle(
        RejectUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(
            request.UserId,
            cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(
                "Kullanıcı bulunamadı.");
        }

        user.Reject();

        await _context.SaveChangesAsync(cancellationToken);
    }
}