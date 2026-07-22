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

builder.Services.AddDbContext<PaymentService.Infrastructure.Persistence.PaymentDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("PaymentDatabase")));

builder.Services.AddScoped<PaymentService.Application.Interfaces.IPaymentRepository, PaymentService.Infrastructure.Repositories.PaymentRepository>();
builder.Services.AddScoped<PaymentService.Application.Interfaces.IPaymentService, PaymentService.Application.Services.PaymentService>();

builder.Services.AddHttpClient<PaymentService.Application.Interfaces.IMockGatewayClient, PaymentService.Infrastructure.Clients.MockGatewayClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:MockPaymentGateway"] ?? throw new Exception("MockPaymentGateway URL is not configured."));
});

var internalApiKey = builder.Configuration["InternalApiKey"] ?? throw new ArgumentNullException("InternalApiKey is missing");

builder.Services.AddHttpClient<PaymentService.Application.Interfaces.IOrderServiceClient, PaymentService.Infrastructure.Clients.OrderServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:OrderService"] ?? throw new Exception("OrderService URL is not configured."));
    client.DefaultRequestHeaders.Add("X-Internal-Api-Key", internalApiKey);
});

builder.Services.AddValidatorsFromAssemblyContaining<PaymentService.Application.Validators.InitiatePaymentDtoValidator>(ServiceLifetime.Transient);

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
