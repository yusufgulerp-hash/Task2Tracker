using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Users.Commands.ApproveUser;

public sealed class ApproveUserCommandHandler
    : IRequestHandler<ApproveUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;

    public ApproveUserCommandHandler(
        IUserRepository userRepository,
        IApplicationDbContext context)
    {
        _userRepository = userRepository;
        _context = context;
    }

    public async Task Handle(
        ApproveUserCommand request,
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

        user.Approve();

        await _context.SaveChangesAsync(cancellationToken);
    }
}