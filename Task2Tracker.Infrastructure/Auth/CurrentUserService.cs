using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Domain.Enums;

namespace Task2Tracker.Infrastructure.Auth;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;

    public Guid UserId
    {
        get
        {
            var value =
                User?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(value) || !Guid.TryParse(value, out var userId))
            {
                throw new InvalidOperationException(
                    "Kimliği doğrulanmış bir kullanıcı bulunamadı. " +
                    "Bu servis yalnızca [Authorize] altındaki endpoint'lerde kullanılmalıdır.");
            }

            return userId;
        }
    }

    public UserRole Role
    {
        get
        {
            var value = User?.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrWhiteSpace(value) || !Enum.TryParse<UserRole>(value, out var role))
            {
                throw new InvalidOperationException(
                    "Kullanıcı rolü token içinde bulunamadı veya geçersiz.");
            }

            return role;
        }
    }
}
