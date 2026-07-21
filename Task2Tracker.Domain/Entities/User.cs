using System.Net.Mail;
using Task2Tracker.Domain.Common;
using Task2Tracker.Domain.Enums;

namespace Task2Tracker.Domain.Entities;

public class User : BaseEntity
{
    public const int MaxFirstNameLength = 100;
    public const int MaxLastNameLength = 100;
    public const int MaxEmailLength = 255;

    private readonly List<TaskItem> _tasks = new();
    private readonly List<RefreshToken> _refreshTokens = new();

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }

    public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    protected User() { }

    public User(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        UserRole role = UserRole.User)
    {
        ValidateAndSetName(firstName, lastName);
        ValidateAndSetEmail(email);

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException(
                "Şifre hash boş olamaz.",
                nameof(passwordHash));

        Id = Guid.NewGuid();
        PasswordHash = passwordHash;
        Role = role;
        Status = UserStatus.PendingApproval;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string firstName,
        string lastName,
        string email)
    {
        ValidateAndSetName(firstName, lastName);
        ValidateAndSetEmail(email);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException(
                "Şifre hash boş olamaz.",
                nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRole(UserRole role)
    {
        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve()
    {
        if (Status != UserStatus.PendingApproval)
            throw new InvalidOperationException(
                "Sadece onay bekleyen kullanıcılar approve edilebilir.");

        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        if (Status != UserStatus.PendingApproval)
            throw new InvalidOperationException(
                "Sadece onay bekleyen kullanıcılar reject edilebilir.");

        Status = UserStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Block()
    {
        if (Status != UserStatus.Active)
            throw new InvalidOperationException(
                "Sadece aktif kullanıcılar block edilebilir.");

        Status = UserStatus.Blocked;
        UpdatedAt = DateTime.UtcNow;
    }

    private void ValidateAndSetName(
        string firstName,
        string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException(
                "İsim alanı boş bırakılamaz.",
                nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException(
                "Soyisim alanı boş bırakılamaz.",
                nameof(lastName));

        if (ContainsInvalidCharacters(firstName) ||
            ContainsInvalidCharacters(lastName))
            throw new ArgumentException(
                "İsim veya soyisim alanı sayı veya özel karakter içeremez.");

        if (firstName.Length > MaxFirstNameLength)
            throw new ArgumentException(
                $"İsim en fazla {MaxFirstNameLength} karakter olabilir.",
                nameof(firstName));

        if (lastName.Length > MaxLastNameLength)
            throw new ArgumentException(
                $"Soyisim en fazla {MaxLastNameLength} karakter olabilir.",
                nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    private void ValidateAndSetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException(
                "E-posta adresi boş bırakılamaz.",
                nameof(email));

        if (email.Length > MaxEmailLength)
            throw new ArgumentException(
                $"E-posta en fazla {MaxEmailLength} karakter olabilir.",
                nameof(email));

        try
        {
            _ = new MailAddress(email.Trim());
        }
        catch
        {
            throw new ArgumentException(
                "Geçerli bir e-posta adresi giriniz.",
                nameof(email));
        }

        Email = email.Trim();
    }

    private static bool ContainsInvalidCharacters(string input)
        => input.Any(c =>
            !char.IsLetter(c) &&
            !char.IsWhiteSpace(c));
}