using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using TaskManagerApi.Data;
using TaskManagerApi.Middlewares;
using TaskManagerApi.Models.Settings;
using TaskManagerApi.Services.Implementations;
using TaskManagerApi.Services.Interfaces;
using System.Linq;
using static TaskManagerApi.Models.Constants.ServerSettingsConstants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var serverSettings = new ServerSettings();
builder.Configuration.GetSection("serverSettings").Bind(serverSettings);
builder.Services.AddOpenApi();
builder.Services.AddControllers()
       .AddNewtonsoftJson(opts =>
           opts.SerializerSettings.ReferenceLoopHandling 
             = ReferenceLoopHandling.Ignore);
builder.Services.AddSwaggerGen();

var taskHistoryUrl = serverSettings.ApiServices["TaskHistory"];
builder.Services.AddHttpClient("taskHistory", client =>
{
    client.BaseAddress = new Uri(taskHistoryUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});
builder.Services.AddDbContext<TicketManagerAPIDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<ITicketHistoryService, TicketHistoryService>();
builder.Services.AddScoped<IAccountVerification, AccountVerification>();
builder.Services.AddScoped<IAiService, AiService>();
builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5220);
        options.ListenAnyIP(7099, listenOptions =>
        {
            listenOptions.UseHttps(); 
        });
    });
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = serverSettings.Client.Authority;
        options.Audience = serverSettings.Client.Audience;
        options.TokenValidationParameters.ValidateAudience = true;
        options.TokenValidationParameters.ValidateLifetime = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactClient",
        builder => builder
            .WithOrigins("https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactClient");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseHsts();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();

