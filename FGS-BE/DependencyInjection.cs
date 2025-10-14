using FGS_BE.Repo.Data;
using FGS_BE.Repo.Data.SeedData;
using FGS_BE.Repo.Entities;
using FGS_BE.Repo.Repositories.Implements;
using FGS_BE.Repo.Repositories.Interfaces;
using FGS_BE.Services.Interfaces;
using FGS_BE.Services.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Routing;
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

    private static void AddServices(this IServiceCollection services)
    {
        services
            //.AddScoped<ICurrentUserService, CurrentUserService>()
            //.AddScoped<IMomoPaymentService, MomoPaymentService>()
            //.AddScoped<IVnPayPaymentService, VnPayPaymentService>()
            //.AddTransient<IEmailSender, EmailSender>()
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

    private static void AddConfigureSettingServices(this IServiceCollection services, IConfiguration configuration)
    {

        //services.Configure<VnPaySettings>(configuration.GetSection(VnPaySettings.Section));
        //services.Configure<MomoSettings>(configuration.GetSection(MomoSettings.Section));
        //services.Configure<MailSettings>(configuration.GetSection(MailSettings.Section));
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
        //services.AddFluentValidationRulesToSwagger();
        //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        //ValidatorOptions.Global.PropertyNameResolver = CamelCasePropertyNameResolver.ResolvePropertyName;
        //services.AddFluentValidationAutoValidation();
    }

    public static void AddWebServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddEndpointsApiExplorer();
        services.AddControllerServices();
        services.AddSwaggerServices();
        services.AddUrlHelperServices();
        services.AddDistributedMemoryCache();

    }

    private static void AddUrlHelperServices(this IServiceCollection services)
    {

        //services.AddSingleton<IActionContextAccessor, ActionContextAccessor>()
        // .AddScoped((IServiceProvider it) =>
        //     it.GetRequiredService<IUrlHelperFactory>()
        //       .GetUrlHelper(it.GetRequiredService<IActionContextAccessor>().ActionContext!));

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
        //services.AddSwaggerGen(c =>
        //{
        //    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        //    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

        //    xmlFilename = $"{typeof(AssemblyReference).Assembly.GetName().Name}.xml";
        //    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

        //    c.EnableAnnotations();
        //});
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
        //app.UseAuthentication();
        //app.UseAuthorization();
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
        //app.UseExceptionHandler(exceptionHandlerApp =>
        //{
        //    exceptionHandlerApp.Run(async context =>
        //    {
        //        var _factory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();
        //        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        //        var exception = exceptionHandlerFeature?.Error;

        //        context.Response.ContentType = MediaTypeNames.Application.Json;
        //        context.Response.StatusCode = exception switch
        //        {
        //            BadRequestException e => StatusCodes.Status400BadRequest,
        //            ValidationBadRequestException e => StatusCodes.Status400BadRequest,
        //            ConflictException e => StatusCodes.Status409Conflict,

        //            NotFoundException e => StatusCodes.Status404NotFound,
        //            UnauthorizedAccessException e => StatusCodes.Status401Unauthorized,
        //            _ => StatusCodes.Status500InternalServerError,
        //        };

        //        var problemDetails = _factory.CreateProblemDetails(
        //                     httpContext: context,
        //                     statusCode: context.Response.StatusCode,
        //                     detail: exception?.Message);

        //        var options = JsonSerializerUtils.GetGlobalJsonSerializerOptions();

        //        var result = JsonSerializer.Serialize(problemDetails, options);

        //        if (exception is ValidationBadRequestException badRequestException)
        //        {
        //            if (badRequestException.ModelState != null)
        //            {
        //                problemDetails = _factory.CreateValidationProblemDetails(
        //                      httpContext: context,
        //                      modelStateDictionary: badRequestException.ModelState,
        //                      statusCode: context.Response.StatusCode,
        //                      detail: exception?.Message);
        //                result = JsonSerializer.Serialize((ValidationProblemDetails)problemDetails, options);
        //            }
        //        }

        //        await context.Response.WriteAsync(result);

        //    });
        //});
    }

}
