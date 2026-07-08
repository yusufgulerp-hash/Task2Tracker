using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Task2Tracker.Domain.Entities;

namespace Task2Tracker.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Project> Projects { get; }
    DbSet<TaskItem> Tasks { get; }
    DbSet<User> Users { get; }

    // Yapılan değişiklikleri veri tabanına atomik olarak kaydeder
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}