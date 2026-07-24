using FluentValidation;

namespace Task2Tracker.Application.Features.Tasks.Commands.UpdateTask;

public sealed class UpdateTaskCommandValidator
    : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Priority)
            .IsInEnum();

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.BlockerNote)
            .MaximumLength(1000);
    }
}