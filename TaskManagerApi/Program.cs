using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManagerApi.Data;
using TaskManagerApi.Models.Settings;
using TaskManagerApi.Services.Implementations;
using TaskManagerApi.Services.Interfaces;
using static TaskManagerApi.Models.Constants.ServerSettingsConstants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
var serverSettings = new ServerSettings();
builder.Configuration.GetSection("IdentityServerSettings").Bind(serverSettings);
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient(
    "taskHistory",
    client =>
    {
        client.BaseAddress = new Uri("http://localhost:5178");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    }
);
builder.Services.AddDbContext<TaskManagerAPIDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    // options.UseSqlServer(builder.Configuration.GetConnectionString(serverSettings.ConnectionStrings.Connections.First(c => c == "DefaultConnection")))
);
builder.Services.AddScoped<ITaskItemService, TaskItemService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IProjectStatusesService, ProjectStatusesService>();
builder.Services.AddScoped<ITaskHistoryService, TaskHistoryService>();
builder.Services.AddScoped<IAccountVerification, AccountVerification>();
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
        options.Authority = "https://localhost:7270";
        options.Audience = "taskManagerApi";
        // options.Authority = serverSettings.Client.Authority;
        // options.Audience = serverSettings.Client.Audience;
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
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
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
app.UseAuthentication();
app.UseAuthorization();
app.UseHsts();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();

