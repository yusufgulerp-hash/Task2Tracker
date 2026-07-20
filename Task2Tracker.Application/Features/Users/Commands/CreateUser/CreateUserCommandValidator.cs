using FluentValidation;
using Task2Tracker.Domain.Entities;
namespace Task2Tracker.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(v => v.FirstName)
            .NotEmpty().WithMessage("İsim alanı boş bırakılamaz.")
            .MaximumLength(User.MaxFirstNameLength).WithMessage("İsim en fazla 20 karakter olabilir.");

        RuleFor(v => v.LastName)
            .NotEmpty().WithMessage("Soyisim alanı boş geçilemez.")
            .MaximumLength(User.MaxLastNameLength).WithMessage("Soyisim en fazla 20 karakter olabilir.");

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage("E-posta adresi boş geçilemez.")
            .EmailAddress().WithMessage("Lütfen geçerli bir e-posta adresi giriniz.")
            .MaximumLength(User.MaxEmailLength).WithMessage("E-posta en fazla 50 karakter olabilir.");
    }
}