namespace Task2Tracker.Application.Common.Exceptions;

public sealed class ConflictException : AppException
{
    public ConflictException(string message)
        : base(message)
    {
    }
}