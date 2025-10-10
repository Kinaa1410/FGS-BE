using FGS_BE.Repo.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<FGSDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

string defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<FGSDbContext>(options =>
    options.UseMySql(defaultConnection, ServerVersion.AutoDetect(defaultConnection),
               builder =>
               {
                   builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                   builder.MigrationsAssembly(typeof(FGSDbContext).Assembly.FullName);
               }).EnableSensitiveDataLogging()
                 .EnableDetailedErrors());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
