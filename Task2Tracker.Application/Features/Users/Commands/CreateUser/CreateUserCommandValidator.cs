using FluentValidation;

namespace Task2Tracker.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(v => v.FirstName)
            .NotEmpty().WithMessage("İsim alanı boş geçilemez.")
            .MaximumLength(20).WithMessage("İsim en fazla 20 karakter olabilir.");

        RuleFor(v => v.LastName)
            .NotEmpty().WithMessage("Soyisim alanı boş geçilemez.")
            .MaximumLength(20).WithMessage("Soyisim en fazla 20 karakter olabilir.");

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage("E-posta adresi boş geçilemez.")
            .EmailAddress().WithMessage("Lütfen geçerli bir e-posta adresi giriniz.")
            .MaximumLength(50).WithMessage("E-posta en fazla 50 karakter olabilir.");
    }
}