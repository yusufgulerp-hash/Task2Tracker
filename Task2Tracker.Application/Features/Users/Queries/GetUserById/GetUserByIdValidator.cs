using FluentValidation;

namespace Task2Tracker.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("User id is required.");
    }
}