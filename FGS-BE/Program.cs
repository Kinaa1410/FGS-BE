using FGS_BE;
using FGS_BE.Repo.Data;
using FGS_BE.Repo.Repositories.Implements;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Repo.Settings;
using FGS_BE.Service.Implements;
using FGS_BE.Service.Interfaces;
using FGS_BE.Service.Services;
using FGS_BE.Services.Implements;
using FGS_BE.Services.Interfaces;
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

// Cloudinary configuration
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services

    //repositories
    .AddScoped<ISemesterRepository, SemesterRepository>()
    .AddScoped<IRewardItemRepository, RewardItemRepository>()
    .AddScoped<ITermKeywordRepository, TermKeywordRepository>()
    .AddScoped<IProjectRepository, ProjectRepository>()
    .AddScoped<IMilestoneRepository, MilestoneRepository>()
    .AddScoped<ITaskRepository, TaskRepository>()
    .AddScoped<IRedeemRequestRepository, RedeemRequestRepository>()
    .AddScoped<ISubmissionRepository, SubmissionRepository>()
    .AddScoped<IProjectMemberRepository, ProjectMemberRepository>()
    .AddScoped<ILevelRepository, LevelRepository>()
    .AddScoped<IPerformanceScoreRepository, PerformanceScoreRepository>()

    //services
    .AddScoped<IRedeemRequestService, RedeemRequestService>()
    .AddScoped<ISemesterService, SemesterService>()
    .AddScoped<IRewardItemService, RewardItemService>()
    .AddScoped<ITermKeywordService, TermKeywordService>()
    .AddScoped<IProjectService, ProjectService>()
    .AddScoped<IMilestoneService, MilestoneService>()
    .AddScoped<ITaskService, TaskService>()
    .AddScoped<ISubmissionService, SubmissionService>()
    .AddScoped<ICloudinaryService, CloudinaryService>()
    .AddScoped<IProjectMemberService, ProjectMemberService>()
    .AddScoped<ILevelService, LevelService>()
    .AddScoped<INotificationService, NotificationService>()
    .AddScoped<IPerformanceScoreService, PerformanceScoreService>()

    .AddScoped<IUnitOfWork>(provider =>
    {
        var context = provider.GetRequiredService<ApplicationDbContext>();
        var semesterRepo = provider.GetRequiredService<ISemesterRepository>();
        var rewardItemRepo = provider.GetRequiredService<IRewardItemRepository>();
        var termKeywordRepo = provider.GetRequiredService<ITermKeywordRepository>();
        var projectRepo = provider.GetRequiredService<IProjectRepository>();
        var milestoneRepo = provider.GetRequiredService<IMilestoneRepository>();
        var taskRepo = provider.GetRequiredService<ITaskRepository>();
        var redeemRequestRepo = provider.GetRequiredService<IRedeemRequestRepository>();
        var submissionRepo = provider.GetRequiredService<ISubmissionRepository>();
        var projectMemberRepo = provider.GetRequiredService<IProjectMemberRepository>();
        var performanceScoreRepo = provider.GetRequiredService<IPerformanceScoreRepository>();
        return new UnitOfWork(context, semesterRepo, rewardItemRepo, termKeywordRepo, 
            projectRepo, milestoneRepo, taskRepo, redeemRequestRepo, submissionRepo,
            projectMemberRepo, performanceScoreRepo);
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();




var app = builder.Build();
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.MapControllers();
app.Run();
