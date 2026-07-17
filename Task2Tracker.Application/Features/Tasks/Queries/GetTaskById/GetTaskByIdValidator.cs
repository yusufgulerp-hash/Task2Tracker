using FluentValidation;

namespace Task2Tracker.Application.Features.Tasks.Queries.GetTaskById;

public sealed class GetTaskByIdValidator : AbstractValidator<GetTaskByIdQuery>
{
    public GetTaskByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}