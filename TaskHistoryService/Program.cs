using TaskHistoryService.Services.Interfaces;
using TaskHistoryService.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using TaskHistoryService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<IHistoryService, HistoryService>();
builder.Services.AddDbContext<TaskHistoryAPIDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// builder.WebHost.ConfigureKestrel(options =>
// {
//     options.ListenAnyIP(5178);
//     options.ListenAnyIP(7299, listenOptions =>
//     {
//         listenOptions.UseHttps(); 
//     });
// });
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowReactClient",
//         builder => builder
//             .WithOrigins("https://localhost:5173", "https://localhost:7270")
//             .AllowAnyHeader()
//             .AllowAnyMethod()
//             .AllowCredentials());
// });

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();
app.MapControllers();
app.Run();
