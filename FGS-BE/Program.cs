using FGS_BE;
using FGS_BE.Repo.Data;
using FGS_BE.Repo.Repositories.Implements;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Service.Implements;
using FGS_BE.Service.Interfaces;
using FGS_BE.Services.Interfaces;
using FGS_BE.Services.Services;
using Microsoft.EntityFrameworkCore;

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
    .AddScoped<ITermKeywordRepository, TermKeywordRepository>()
    .AddScoped<IProjectRepository, ProjectRepository>()
    .AddScoped<IMilestoneRepository, MilestoneRepository>()
    .AddScoped<ITaskRepository, TaskRepository>()


    .AddScoped<ISemesterService, SemesterService>()
    .AddScoped<IRewardItemService, RewardItemService>()
    .AddScoped<ITermKeywordService, TermKeywordService>()
    .AddScoped<IProjectService, ProjectService>()
    .AddScoped<IMilestoneService, MilestoneService>()
    .AddScoped<ITaskService, TaskService>()

    .AddScoped<IUnitOfWork>(provider =>
    {
        var context = provider.GetRequiredService<ApplicationDbContext>();
        var semesterRepo = provider.GetRequiredService<ISemesterRepository>();
        var rewardItemRepo = provider.GetRequiredService<IRewardItemRepository>();
        var termKeywordRepo = provider.GetRequiredService<ITermKeywordRepository>();
        var projectRepo = provider.GetRequiredService<IProjectRepository>();
        var milestoneRepo = provider.GetRequiredService<IMilestoneRepository>();
        var taskRepo = provider.GetRequiredService<ITaskRepository>();
        return new UnitOfWork(context, semesterRepo, rewardItemRepo, termKeywordRepo, projectRepo, milestoneRepo, taskRepo);
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
await app.UseWebApplication();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
