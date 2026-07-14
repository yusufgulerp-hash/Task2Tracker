using MediatR;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;
using Task2Tracker.Domain.Entities;

namespace Task2Tracker.Application.Features.Projects.Commands.CreateProject;

public sealed class CreateProjectCommandHandler
    : IRequestHandler<CreateProjectCommand, Guid>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IApplicationDbContext _context;

    public CreateProjectCommandHandler(
        IProjectRepository projectRepository,
        IApplicationDbContext context)
    {
        _projectRepository = projectRepository;
        _context = context;
    }

    public async Task<Guid> Handle(
        CreateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var exists = await _projectRepository.ExistsByNameAsync(
            request.Name,
            cancellationToken);

        if (exists)
        {
            throw new Exception("Project name already exists.");
        }

        var project = new Project(request.Name);

        _projectRepository.Add(project);

        await _context.SaveChangesAsync(cancellationToken);

        return project.Id;
    }
}