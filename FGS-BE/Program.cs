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
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProjectDtoValidator>();
// Custom extensions (these handle additional registrations like auto-scanning)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWebServices(builder.Configuration);
// Database Context (overriding the commented MySQL in extensions for SQL Server)
string defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection")
?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(defaultConnection)
.EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
.EnableDetailedErrors(builder.Environment.IsDevelopment())
.UseProjectables());
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
.AddScoped<IUserRepository, UserRepository>();
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
.AddScoped<IProjectInvitationService, ProjectInvitationService>();
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
    return new UnitOfWork(context, semesterRepo, rewardItemRepo, termKeywordRepo,
    projectRepo, milestoneRepo, taskRepo, redeemRequestRepo, submissionRepo,
    projectMemberRepo, performanceScoreRepo, userRepo, projectInvitationRepo,
    notificationRepo, notificationTemplateRepo);
});
// Background Services (e.g., for expiring invitations)
builder.Services.AddHostedService<InvitationExpiryService>();
// Authorization (enables [Authorize] attributes on controllers)
builder.Services.AddAuthorization();
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
app.MapControllers();
app.Run();