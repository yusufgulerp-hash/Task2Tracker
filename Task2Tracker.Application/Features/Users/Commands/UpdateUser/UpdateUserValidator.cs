using FluentValidation;
using Task2Tracker.Domain.Entities;
namespace Task2Tracker.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(User.MaxFirstNameLength);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(User.MaxLastNameLength);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(User.MaxEmailLength);
    }
}