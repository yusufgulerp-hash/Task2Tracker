using MediatR;
using Task2Tracker.Application.Features.Users.DTOs;
using Task2Tracker.Application.Interfaces.Repositories;
using Task2Tracker.Domain.Enums;

namespace Task2Tracker.Application.Features.Users.Queries.GetPendingUsers;

public sealed class GetPendingUsersQueryHandler
    : IRequestHandler<GetPendingUsersQuery, List<PendingUserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetPendingUsersQueryHandler(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<PendingUserDto>> Handle(
        GetPendingUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetByStatusAsync(
            UserStatus.PendingApproval,
            cancellationToken);

        return users
            .Select(user => new PendingUserDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.CreatedAt))
            .ToList();
    }
}