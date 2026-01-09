using FGS_BE;
using FGS_BE.Repo.Data;
using FGS_BE.Repo.DTOs.Projects.Validators;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Implements;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Repo.Settings;
using FGS_BE.Service.Implements;
using FGS_BE.Service.Interfaces;
using FGS_BE.Service.Services;
using FGS_BE.Services.Implements;
using FGS_BE.Services.Interfaces;
using FGS_BE.Services.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProjectDtoValidator>();
// Custom extensions (these handle additional registrations like auto-scanning)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebServices(builder.Configuration);
// Database Context (SQL Server)
string defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(defaultConnection)
        .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
        .EnableDetailedErrors(builder.Environment.IsDevelopment())
        .UseProjectables());
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
// Repositories (all Scoped for per-request lifetime with DbContext)
builder.Services
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
    .AddScoped<IProjectInvitationRepository, ProjectInvitationRepository>()
    .AddScoped<INotificationRepository, NotificationRepository>()
    .AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>()
    .AddScoped<IUserRepository, UserRepository>()
    .AddScoped<IUserProjectStatsRepository, UserProjectStatsRepository>();  // New: For escalation threshold
// Services (all Scoped to match repositories/DbContext)
builder.Services
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
    .AddScoped<IUserService, UserService>()
    .AddScoped<INotificationTemplateService, NotificationTemplateService>()
    .AddScoped<IProjectInvitationService, ProjectInvitationService>()
    .AddScoped<IEmailService, EmailService>()
    // Jobs as Scoped services for Hangfire
    .AddScoped<InvitationExpiryService>()
    .AddScoped<ProjectClosureService>()
    .AddScoped<SemesterStatusSyncService>();
// UnitOfWork (Scoped factory to inject all repositories)
builder.Services.AddScoped<IUnitOfWork>(provider =>
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
    var userRepo = provider.GetRequiredService<IUserRepository>();
    var notificationRepo = provider.GetRequiredService<INotificationRepository>();
    var notificationTemplateRepo = provider.GetRequiredService<INotificationTemplateRepository>();
    var projectInvitationRepo = provider.GetRequiredService<IProjectInvitationRepository>();
    var userProjectStatsRepo = provider.GetRequiredService<IUserProjectStatsRepository>();  // New: Inject for escalation
    return new UnitOfWork(context, semesterRepo, rewardItemRepo, termKeywordRepo,
        projectRepo, milestoneRepo, taskRepo, redeemRequestRepo, submissionRepo,
        projectMemberRepo, performanceScoreRepo, userRepo, projectInvitationRepo,
        notificationRepo, notificationTemplateRepo, userProjectStatsRepo);  // Updated: Include new repo
});
// Authorization (enables [Authorize] attributes on controllers)
builder.Services.AddAuthorization();
// Hangfire (for all 3 jobs)
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();
var app = builder.Build();
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FGS API v1");
        options.RoutePrefix = string.Empty; // Optional: Serve Swagger at app root
    });
}
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
// Essential Middleware Pipeline (using extension for consistency)
await app.UseWebApplication();
app.UseAuthentication();
app.UseAuthorization();
// Hangfire Dashboard (secure in prod with auth)
app.UseHangfireDashboard("/hangfire");
// Schedule 3 Jobs
RecurringJob.AddOrUpdate<InvitationExpiryService>("invitation-expiry",
    service => service.ProcessExpiredInvitationsAsync(),
    Cron.Minutely); // Every minute
RecurringJob.AddOrUpdate<ProjectClosureService>("project-closure",
    service => service.AutoCloseProjectsAsync(),
    Cron.Daily(0, 0)); // Daily at 00:00 UTC
RecurringJob.AddOrUpdate<SemesterStatusSyncService>("semester-status-sync",
    service => service.SyncAllSemesterStatusesAsync(),
    Cron.Daily(0, 0)); // Daily at 00:00 UTC
app.MapControllers();
app.Run();