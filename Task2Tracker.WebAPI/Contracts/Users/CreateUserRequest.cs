namespace Task2Tracker.WebAPI.Contracts.Users;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email);