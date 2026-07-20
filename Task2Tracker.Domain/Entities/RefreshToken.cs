namespace Task2Tracker.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    protected RefreshToken() { }

    public RefreshToken(string token, Guid userId, int expiryDays = 7)
    {
        Id = Guid.NewGuid();
        Token = token;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.AddDays(expiryDays);
        IsRevoked = false;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke()
    {
        IsRevoked = true;
    }
}