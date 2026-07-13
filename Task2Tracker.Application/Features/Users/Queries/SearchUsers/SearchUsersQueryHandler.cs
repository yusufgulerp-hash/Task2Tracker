using MediatR;
using Task2Tracker.Application.Features.Users.DTOs;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Users.Queries.SearchUsers;

public class SearchUsersQueryHandler
    : IRequestHandler<SearchUsersQuery, List<UserListItemDto>>
{
    private readonly IUserRepository _userRepository;

    public SearchUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserListItemDto>> Handle(
        SearchUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _userRepository.SearchAsync(
            request.Text,
            cancellationToken);

        return users
            .Select(user => new UserListItemDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email))
            .ToList();
    }
}