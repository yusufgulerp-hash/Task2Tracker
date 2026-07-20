using FluentValidation;

namespace Task2Tracker.Application.Features.Tasks.Queries.SearchTasks;

public sealed class SearchTasksQueryValidator
    : AbstractValidator<SearchTasksQuery>
{
    public SearchTasksQueryValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Arama metni giriniz.")
            .MinimumLength(2)
            .WithMessage("Arama metni en az 2 karakter olmalıdır.")
            .MaximumLength(100)
            .WithMessage("Arama metni en fazla 100 karakter olmalıdır.");
    }
}