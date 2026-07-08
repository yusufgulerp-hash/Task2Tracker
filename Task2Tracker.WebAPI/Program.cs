using Task2Tracker.Application;
using Task2Tracker.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// === Katmanlarımızın Servis Kayıtları (IoC Container) ===
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
// =========================================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// === HTTP İstek Hattı Yapılandırması (Middleware Pipeline) ===
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();