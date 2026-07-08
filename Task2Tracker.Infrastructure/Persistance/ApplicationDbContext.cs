using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Domain.Entities;

namespace Task2Tracker.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    // Program.cs üzerinden gelecek veri tabanı bağlantı ayarlarını (ConnectionString vb.) karşılayan kurucu metot
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Veri tabanındaki tablolarımızın C# tarafındaki temsilleri
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<User> Users => Set<User>();

    // Application katmanından çağırdığımız atomik kaydetme sözleşmesinin içi
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // İleride buraya AuditLog (Veriyi kim değiştirdi, ne zaman değiştirdi) mekanizması ekleyebiliriz.
        return await base.SaveChangesAsync(cancellationToken);
    }

    // Tablo şemalarını, ilişkilerini (One-to-Many vb.) ve kısıtlamaları (Fluent API) ayağa kaldıran sihirli metot
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Bu satır sayesinde, bu projede (Assembly) yazacağımız tüm tablo konfigürasyon dosyalarını (IEntityTypeConfiguration) tek tek elle kaydetmek yerine otomatik bulup uygular.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}