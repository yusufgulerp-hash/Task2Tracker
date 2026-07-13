using MediatR;
using Task2Tracker.Application.Features.Users.DTOs;

namespace Task2Tracker.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDetailDto>;