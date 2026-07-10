using System;
using System.Collections.Generic;
using System.Linq;
using Task2Tracker.Domain.Common;

namespace Task2Tracker.Domain.Entities;

public class User : BaseEntity
{
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

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-posta adresi boş bırakılamaz.", nameof(email));

        Id = Guid.NewGuid(); // BaseEntity'den gelen ID
        Email = email;
        CreatedAt = DateTime.UtcNow; // BaseEntity'den gelen oluşturulma tarihi
    }

    public void UpdateDetails(string firstName, string lastName, string email)
    {
        ValidateAndSetName(firstName, lastName);

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Güncellenen e-posta adresi boş olamaz.", nameof(email));

        Email = email;
        UpdatedAt = DateTime.UtcNow; // BaseEntity'den gelen güncellenme tarihi
    }

    // 🌟 Domain Invariant: Sayı ve özel karakter kısıtlamasını merkezi olarak yöneten metot
    private void ValidateAndSetName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("İsim alanı boş bırakılamaz.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Soyisim alanı boş bırakılamaz.", nameof(lastName));

        if (ContainsInvalidCharacters(firstName) || ContainsInvalidCharacters(lastName))
            throw new ArgumentException("İsim veya soyisim alanı sayı veya özel karakter içeremez.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    private static bool ContainsInvalidCharacters(string input)
    {
        return input.Any(c => !char.IsLetter(c) && !char.IsWhiteSpace(c));
    }
}