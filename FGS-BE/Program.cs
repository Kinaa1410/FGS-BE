using FGS_BE;
using FGS_BE.Repo.Data;
using FGS_BE.Repo.Repositories.Implements;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Services.Interfaces;
using FGS_BE.Services.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebServices(builder.Configuration);

string defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(defaultConnection)
           .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
           .EnableDetailedErrors(builder.Environment.IsDevelopment())
           .UseProjectables());

builder.Services

    .AddScoped<ISemesterRepository, SemesterRepository>()
    .AddScoped<IRewardItemRepository, RewardItemRepository>()

    .AddScoped<ISemesterService, SemesterService>()
    .AddScoped<IRewardItemService, RewardItemService>()

    .AddScoped<IUnitOfWork>(provider =>
    {
        var context = provider.GetRequiredService<ApplicationDbContext>();
        var semesterRepo = provider.GetRequiredService<ISemesterRepository>();
        var rewardItemRepo = provider.GetRequiredService<IRewardItemRepository>();
        return new UnitOfWork(context, semesterRepo, rewardItemRepo);
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();
await app.UseWebApplication();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
