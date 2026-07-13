namespace Task2Tracker.WebAPI.Contracts.Users;

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string Email);