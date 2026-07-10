using MediatR;
using Task2Tracker.Application.Interfaces.Repositories;
using Task2Tracker.Domain.Entities;

namespace Task2Tracker.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IReadOnlyList<User>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<User>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        return await _userRepository.GetAllAsync(cancellationToken);
    }
}