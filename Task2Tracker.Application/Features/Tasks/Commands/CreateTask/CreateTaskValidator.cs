using FluentValidation;

namespace Task2Tracker.Application.Features.Tasks.Commands.CreateTask;

public sealed class CreateTaskCommandValidator
    : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.Priority)
            .IsInEnum();
    }
}