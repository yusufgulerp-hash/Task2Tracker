namespace Task2Tracker.Application.Common.Exceptions;

public sealed class BusinessRuleException : AppException
{
    public BusinessRuleException(string message)
        : base(message)
    {
    }
}