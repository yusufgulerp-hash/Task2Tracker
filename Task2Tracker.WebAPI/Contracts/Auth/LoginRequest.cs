namespace Task2Tracker.WebAPI.Contracts.Auth
{
    public sealed record LoginRequest(
        string Email,
        string Password)
    {
    }
}
