using Task2Tracker.Domain.Enums;

namespace Task2Tracker.Application.Common.Interfaces;

public interface ICurrentUserService
{
    bool IsAuthenticated { get; }

    Guid UserId { get; }

    UserRole Role { get; }

    bool IsAdmin => Role == UserRole.Admin;
}
