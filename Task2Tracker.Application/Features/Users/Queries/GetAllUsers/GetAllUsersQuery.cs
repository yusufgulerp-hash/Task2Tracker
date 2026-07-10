using MediatR;
using Task2Tracker.Domain.Entities;

namespace Task2Tracker.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery : IRequest<IReadOnlyList<User>>;