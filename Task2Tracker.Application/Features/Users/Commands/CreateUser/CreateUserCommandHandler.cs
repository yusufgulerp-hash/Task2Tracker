using MediatR;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;


    public async Task<Guid> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = new Domain.Entities.User(
            request.FirstName,
            request.LastName,
            request.Email);

        _userRepository.Add(user);

        await _context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}