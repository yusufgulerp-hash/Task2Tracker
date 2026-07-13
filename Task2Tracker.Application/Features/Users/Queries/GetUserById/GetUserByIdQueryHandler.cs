using MediatR;
using Task2Tracker.Application.Features.Users.DTOs;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler
    : IRequestHandler<GetUserByIdQuery, UserDetailDto>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDetailDto> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(
    request.Id,
    cancellationToken);

        if (user is null)
        {
            throw new KeyNotFoundException(
                $"User with id '{request.Id}' is not found.");
        }

        return new UserDetailDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email);
    }
}