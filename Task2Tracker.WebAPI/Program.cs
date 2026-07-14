using Microsoft.EntityFrameworkCore;
using Serilog;
using Task2Tracker.Application;
using Task2Tracker.Infrastructure;
using Task2Tracker.Infrastructure.Logging;
using Task2Tracker.Infrastructure.Persistence;
using Task2Tracker.WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// Serilog
// ==========================
builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    SerilogConfiguration.Configure(
        context.Configuration,
        loggerConfiguration);
});

// ==========================
// Katman Servisleri
// ==========================
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// ==========================
// ASP.NET Core Servisleri
// ==========================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ==========================
// EF Core Migration
// ==========================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// ==========================
// Development
// ==========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ==========================
// Middleware Pipeline
// ==========================
app.UseGlobalExceptionMiddleware();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();