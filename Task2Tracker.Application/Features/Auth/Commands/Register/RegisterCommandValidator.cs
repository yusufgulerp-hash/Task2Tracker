using FluentValidation;

namespace Task2Tracker.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Şifre en az 8 karakter olmalıdır.")
            .Matches("[A-Z]")
            .WithMessage("Şifre en az bir büyük harf içermelidir.")
            .Matches("[a-z]")
            .WithMessage("Şifre en az bir küçük harf içermelidir.")
            .Matches("[0-9]")
            .WithMessage("Şifre en az bir rakam içermelidir.");
    }
}