using FGS_BE.Repo.Data;
using FGS_BE.Repo.Repositories.Implements;
using FGS_BE.Repo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<FGSDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

string defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(defaultConnection) 
           .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
           .EnableDetailedErrors(builder.Environment.IsDevelopment()));

builder.Services
    .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>))
    .AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(x => x
   .AllowCredentials()
   .SetIsOriginAllowed(origin => true)
   .AllowAnyMethod()
   .AllowAnyHeader());
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
