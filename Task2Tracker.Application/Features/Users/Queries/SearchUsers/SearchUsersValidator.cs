using FluentValidation;

namespace Task2Tracker.Application.Features.Users.Queries.SearchUsers;

public class SearchUsersValidator : AbstractValidator<SearchUsersQuery>
{
    public SearchUsersValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Arama metini giriniz.")
            .MinimumLength(2)
            .WithMessage("Arama metini en az 2 karakter uzunluğunda olamlıdır.")
            .MaximumLength(100)
            .WithMessage("Arama metini en fazla 100 karakter uzunluğunda olmalıdır.");
    }
}