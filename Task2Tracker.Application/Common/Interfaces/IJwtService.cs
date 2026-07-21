using Task2Tracker.Domain.Entities;

namespace Task2Tracker.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}