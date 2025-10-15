using FGS_BE.Repo.Data;
using FGS_BE.Repo.Data.SeedData;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Exceptions;
using FGS_BE.Repo.Extensions;
using FGS_BE.Repo.Repositories.Implements;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Repo.Settings;
using FGS_BE.Services.Interfaces;
using FGS_BE.Services.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Net.Mime;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Task = System.Threading.Tasks.Task;

namespace FGS_BE;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddServices();
        services.AddDbContext(configuration);
        services.AddRepositories();
        services.AddInitialiseDatabase();
        services.AddDefaultIdentity();
        services.AddConfigureSettingServices(configuration);

    }

    private static void AddConfigureSettingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));
    }

    private static void AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<IJwtService, JwtService>()
            .AddScoped<IAchievementService, AchievementService>()
            .AddScoped<IUserService, UserService>();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services
            .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>))
            .AddScoped<IUnitOfWork, UnitOfWork>();
    }

    private static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        //string defaultConnection = configuration.GetConnectionString("DefaultConnection")!;
        //services.AddDbContext<ApplicationDbContext>((sp, options) =>
        //   options.UseMySql(defaultConnection, ServerVersion.AutoDetect(defaultConnection),
        //       builder =>
        //       {
        //           builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        //           builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
        //       })
        //          //.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>())
        //          .EnableSensitiveDataLogging()
        //          //.UseLazyLoadingProxies()
        //          .EnableDetailedErrors()
        //          .UseProjectables());

    }

    private static void AddDefaultIdentity(this IServiceCollection services)
    {

        services.AddIdentity<User, Role>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 1;
            options.Password.RequiredUniqueChars = 0;
            options.User.RequireUniqueEmail = true;

        }).AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders();
    }

    private static void AddInitialiseDatabase(this IServiceCollection services)
    {
        services
            .AddScoped<ApplicationDbContextInitialiser>();
    }

    public static void AddApplication(this IServiceCollection services)
    {
        services.AddValidators();

    }

    private static void AddValidators(this IServiceCollection services)
    {
        services.AddFluentValidationRulesToSwagger();
        services.AddValidatorsFromAssembly(typeof(Repo.AssemblyReference).Assembly);
        services.AddFluentValidationAutoValidation();
    }

    public static void AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddEndpointsApiExplorer();
        services.AddControllerServices();
        services.AddSwaggerServices();
        services.AddDistributedMemoryCache();
        services.AddAuthenticationServices(configuration);

    }

    private static void AddControllerServices(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
        }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }

    private static void AddSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            xmlFilename = $"{typeof(Repo.AssemblyReference).Assembly.GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

            c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new()
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });
            c.OperationFilter<SecurityRequirementsOperationFilter>(JwtBearerDefaults.AuthenticationScheme);
            c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
            c.EnableAnnotations();
        });
    }

    private static void AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                     configuration.GetSection("Authentication:Schemes:Bearer:SerectKey").Value!)),
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = false,
                ValidateAudience = false,
                NameClaimType = ClaimTypes.NameIdentifier
            };
            options.RequireHttpsMetadata = false;
            options.HandleEvents();
        });
    }

    public static async Task UseWebApplication(this WebApplication app)
    {

        //app.UseSwagger();
        //app.UseSwaggerUI(c =>
        //{
        //    c.EnableDeepLinking();
        //    c.EnablePersistAuthorization();
        //    c.EnableTryItOutByDefault();
        //    c.DisplayRequestDuration();
        //});

        app.UseExceptionApplication();

        await app.UseInitialiseDatabaseAsync();

        app.UseCors(x => x
           .AllowCredentials()
           .SetIsOriginAllowed(origin => true)
           .AllowAnyMethod()
           .AllowAnyHeader());

        //app.UseHttpsRedirection();
        //app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();
        //app.MapControllers();

    }

    public static async Task UseInitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        if (app.Environment.IsDevelopment())
        {
            //await initialiser.DeletedDatabaseAsync();
            await initialiser.MigrateAsync();
            await initialiser.SeedAsync();
        }

        //if (app.Environment.IsProduction())
        //{
        //    await initialiser.MigrateAsync();
        //    await initialiser.SeedAsync();
        //}
    }

    private static void UseExceptionApplication(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                var _factory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = exceptionHandlerFeature?.Error;

                context.Response.ContentType = MediaTypeNames.Application.Json;
                context.Response.StatusCode = exception switch
                {
                    BadRequestException e => StatusCodes.Status400BadRequest,
                    ConflictException e => StatusCodes.Status409Conflict,
                    ForbiddenAccessException e => StatusCodes.Status403Forbidden,
                    NotFoundException e => StatusCodes.Status404NotFound,
                    UnauthorizedAccessException e => StatusCodes.Status401Unauthorized,
                    _ => StatusCodes.Status500InternalServerError,
                };

                var problemDetails = _factory.CreateProblemDetails(
                             httpContext: context,
                             statusCode: context.Response.StatusCode,
                             detail: exception?.Message);

                var result = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, });

                await context.Response.WriteAsync(result);

            });
        });
    }

}
