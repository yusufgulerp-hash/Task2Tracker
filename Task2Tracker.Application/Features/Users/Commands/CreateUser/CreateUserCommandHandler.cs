using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IApplicationDbContext context)
    {
        _userRepository = userRepository;
        _context = context;
    }

    public async Task<Guid> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(
                request.Email,
                cancellationToken))
        {
            throw new ConflictException(
                "Bu e-posta adresi başka bir kullanıcı tarafından kullanılmaktadır.");
        }

        var user = new Domain.Entities.User(
            request.FirstName,
            request.LastName,
            request.Email);

        _userRepository.Add(user);

        await _context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}