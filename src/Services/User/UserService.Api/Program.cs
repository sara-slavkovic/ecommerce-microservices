using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

builder.Services.AddDbContext<UserService.Infrastructure.Persistence.UserDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("UserDatabase")));

builder.Services.AddScoped<UserService.Application.Interfaces.IUserRepository, UserService.Infrastructure.Repositories.UserRepository>();
builder.Services.AddScoped<UserService.Application.Interfaces.IUserService, UserService.Application.Services.UserService>();
builder.Services.AddScoped<UserService.Application.Interfaces.IPasswordHasherService, UserService.Infrastructure.Services.PasswordHasherService>();

builder.Services.AddValidatorsFromAssemblyContaining<UserService.Application.Validators.RegisterUserDtoValidator>(ServiceLifetime.Transient);

builder.Services.AddExceptionHandler<SharedKernel.Web.ExceptionHandlers.ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<SharedKernel.Web.ExceptionHandlers.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
