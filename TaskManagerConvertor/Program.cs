using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using Polly;
using Serilog;
using TaskManagerConvertor.Handlers;
using TaskManagerConvertor.Middlewares;
using TaskManagerConvertor.Models;
using TaskManagerConvertor.Models.Settings;
using TaskManagerConvertor.Providers.Implementations;
using TaskManagerConvertor.Providers.Interfaces;
using TaskManagerConvertor.Services.Implementation;
using TaskManagerConvertor.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var serverSettings = new ServerSettings();
builder.Configuration.GetSection("serverSettings").Bind(serverSettings);
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<IHelperService, HelperService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
// builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountHelperService, AccountHelperService>();
builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddScoped<IHeaderProvider, HeaderProvider>();
builder.Services.AddTransient<HeaderPropagationHandler>();
builder.Services.AddControllers()
       .AddNewtonsoftJson(opts =>
           opts.SerializerSettings.ReferenceLoopHandling 
             = ReferenceLoopHandling.Ignore);
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = serverSettings.Client.Authority;
        options.Audience = serverSettings.Client.Audience;
        options.TokenValidationParameters.ValidateAudience = true;
        options.TokenValidationParameters.ValidateLifetime = true;
    });
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5271);
    options.ListenAnyIP(7188, listenOptions =>
    {
        listenOptions.UseHttps(); 
    });
});
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactClient",
        builder => builder
            .WithOrigins("https://localhost:5173", "https://localhost:7270")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);
});

var projectsBulkhead = Policy.BulkheadAsync<HttpResponseMessage>(
    maxParallelization: 5,
    maxQueuingActions: 20
);
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient(Constants.Settings.HttpClientNaming.TASK_MANAGER_CLIENT, client =>
{
    client.BaseAddress = new Uri(serverSettings.ApiServices["TaskManagerApi"]);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
})
.AddPolicyHandler(projectsBulkhead)
.AddHttpMessageHandler<HeaderPropagationHandler>();
// builder.Services.AddHttpClient<ITicketService, TicketService>(client =>
// {
//     client.BaseAddress = new Uri(serverSettings.ApiServices["TaskManagerApi"]);
//     client.DefaultRequestHeaders.Accept.Add(
//         new MediaTypeWithQualityHeaderValue("application/json"));
// })
// .AddPolicyHandler(projectsBulkhead)
// .AddHttpMessageHandler<HeaderPropagationHandler>();

builder.Services.AddHttpClient<IAccountService, AccountService>(client =>
{
    client.BaseAddress = new Uri(serverSettings.ApiServices["IdentityServer"]);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
})
.AddPolicyHandler(projectsBulkhead)
.AddHttpMessageHandler<HeaderPropagationHandler>();

var app = builder.Build();

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
