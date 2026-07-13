using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Task2Tracker.Domain.Common;

namespace Task2Tracker.Domain.Entities;

public class User : BaseEntity
{
    public const int MaxFirstNameLength = 100;
    public const int MaxLastNameLength = 100;
    public const int MaxEmailLength = 255;
    private readonly List<Project> _projects = new();
    private readonly List<TaskItem> _tasks = new();

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;

    public IReadOnlyCollection<Project> Projects => _projects.AsReadOnly();
    public IReadOnlyCollection<TaskItem> Tasks => _tasks.AsReadOnly();

    protected User() { }

    public User(string firstName, string lastName, string email)
    {
        ValidateAndSetName(firstName, lastName);
        ValidateAndSetEmail(email);

        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string firstName, string lastName, string email)
    {
        ValidateAndSetName(firstName, lastName);
        ValidateAndSetEmail(email);

        UpdatedAt = DateTime.UtcNow;
    }

    private void ValidateAndSetName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("İsim alanı boş bırakılamaz.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Soyisim alanı boş bırakılamaz.", nameof(lastName));

        if (ContainsInvalidCharacters(firstName) || ContainsInvalidCharacters(lastName))
            throw new ArgumentException("İsim veya soyisim alanı sayı veya özel karakter içeremez.");
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
            throw new ArgumentException("E-posta adresi boş bırakılamaz.", nameof(email));
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
            throw new ArgumentException("Geçerli bir e-posta adresi giriniz.", nameof(email));
        }

        Email = email.Trim();
    }

    private static bool ContainsInvalidCharacters(string input)
    {
        return input.Any(c => !char.IsLetter(c) && !char.IsWhiteSpace(c));
    }
}