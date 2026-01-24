using FGS_BE.Repo.Data;
using FGS_BE.Repo.Data.SeedData;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Exceptions;
using FGS_BE.Repo.Extensions;
using FGS_BE.Repo.Repositories.Implements;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Repo.Settings;
using FGS_BE.Service.Services;
using FGS_BE.Services.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
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
        // options.UseMySql(defaultConnection, ServerVersion.AutoDetect(defaultConnection),
        // builder =>
        // {
        // builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        // builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
        // })
        // //.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>())
        // .EnableSensitiveDataLogging()
        // //.UseLazyLoadingProxies()
        // .EnableDetailedErrors()
        // .UseProjectables());
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
        services.AddDataProtection(configuration);
        services.AddCors(configuration);
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
        // Fetch and validate the Bearer configuration section
        var bearerSection = configuration.GetSection("Authentication:Schemes:Bearer");
        if (!bearerSection.Exists())
        {
            throw new InvalidOperationException("Bearer authentication configuration section 'Authentication:Schemes:Bearer' not found in appsettings.json.");
        }
        var secretKey = bearerSection.GetValue<string>("SecretKey");
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("JWT SecretKey is null, empty, or whitespace. Check 'Authentication:Schemes:Bearer:SecretKey' in configuration.");
        }
        var validIssuer = bearerSection.GetValue<string>("ValidIssuer");
        var validAudiencesSection = bearerSection.GetSection("ValidAudiences");
        var validAudiences = validAudiencesSection.Get<string[]>() ?? Array.Empty<string>();
        // Log for debugging (remove in production)
        Console.WriteLine($"Loaded JWT Config - SecretKey Length: {secretKey.Length}, Issuer: '{validIssuer}', Audiences Count: {validAudiences.Length}");
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(jwtOptions =>
        {
            jwtOptions.TokenValidationParameters = new TokenValidationParameters
            {
                // Securely create signing key from config
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateIssuerSigningKey = true,
                // Strict lifetime validation (enforces 'exp' claim)
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero, // No leeway for clock differences
                // Conditional issuer validation
                ValidateIssuer = !string.IsNullOrWhiteSpace(validIssuer),
                ValidIssuer = validIssuer,
                // Conditional audience validation
                ValidateAudience = validAudiences.Length > 0,
                ValidAudiences = validAudiences,
                // Standard claims
                NameClaimType = ClaimTypes.NameIdentifier,
                RoleClaimType = ClaimTypes.Role // If using roles
            };
            // Disable HTTPS requirement for local dev (enable in prod)
            jwtOptions.RequireHttpsMetadata = false;
            // Event handlers for debugging/logging (customize as needed)
            jwtOptions.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    // Log failure reason
                    Console.WriteLine($"JWT Auth Failed: {context.Exception?.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    // Optional: Inspect/add claims post-validation
                    var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    Console.WriteLine($"JWT Validated for User: {userId}");
                    return Task.CompletedTask;
                },
                OnForbidden = context =>
                {
                    Console.WriteLine("JWT Access Forbidden (e.g., invalid role/scope)");
                    return Task.CompletedTask;
                }
            };
        });
    }

    private static void AddDataProtection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "keys"))) // Dev: local folder; Prod: Use Azure KeyVault
            .SetApplicationName("FGSApp") // Scopes keys to your application
            .SetDefaultKeyLifetime(TimeSpan.FromDays(90)); // Rotate keys every 90 days
    }

    private static void AddCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>()
                             ?? new[] { "http://localhost:5173", "https://localhost:5173" };
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
        });
    }

    public class RefreshTokenSettings
    {
        public string SecretRefreshKey { get; set; } = string.Empty;
        public int RefreshTokenExpire { get; set; } // Seconds, as in your config
    }

    public static async Task UseWebApplication(this WebApplication app)
    {
        //app.UseSwagger();
        //app.UseSwaggerUI(c =>
        //{
        // c.EnableDeepLinking();
        // c.EnablePersistAuthorization();
        // c.EnableTryItOutByDefault();
        // c.DisplayRequestDuration();
        //});
        app.UseExceptionApplication();
        await app.UseInitialiseDatabaseAsync();
        app.UseCors("AllowAll");
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
        // await initialiser.MigrateAsync();
        // await initialiser.SeedAsync();
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