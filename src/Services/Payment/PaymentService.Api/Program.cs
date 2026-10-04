using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Infrastructure.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddDbContext<PaymentService.Infrastructure.Persistence.PaymentDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("PaymentDatabase")));

builder.Services.Configure<PaymentService.Application.Settings.PaymentSimulationSettings>(builder.Configuration.GetSection("PaymentSimulation"));

builder.Services.AddScoped<PaymentService.Application.Interfaces.IPaymentRepository, PaymentService.Infrastructure.Repositories.PaymentRepository>();
builder.Services.AddScoped<PaymentService.Application.Interfaces.IPaymentService, PaymentService.Application.Services.PaymentService>();

builder.Services.AddTransient<PaymentService.Infrastructure.Handlers.PaymentAttemptTrackingHandler>();

// 1. Assign the HttpClientBuilder to a variable
var mockGatewayClientBuilder = builder.Services.AddHttpClient<PaymentService.Application.Interfaces.IMockGatewayClient, PaymentService.Infrastructure.Clients.MockGatewayClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:MockPaymentGateway"] ?? throw new Exception("MockPaymentGateway URL is not configured."));
});
// 2. Add Resilience to the builder (Outer layer: handles the retry loop)
mockGatewayClientBuilder.AddCustomResilienceHandler(builder.Configuration, "ResilienceSettings:MockGateway");
// 3. Add Tracking Handler to the builder (Inner layer: executes on every single retry attempt)
mockGatewayClientBuilder.AddHttpMessageHandler<PaymentService.Infrastructure.Handlers.PaymentAttemptTrackingHandler>();

var internalApiKey = builder.Configuration["InternalApiKey"] ?? throw new ArgumentNullException("InternalApiKey is missing");

builder.Services.AddHttpClient<PaymentService.Application.Interfaces.IOrderServiceClient, PaymentService.Infrastructure.Clients.OrderServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:OrderService"] ?? throw new Exception("OrderService URL is not configured."));
    client.DefaultRequestHeaders.Add("X-Internal-Api-Key", internalApiKey);
}).AddCustomResilienceHandler(builder.Configuration, "ResilienceSettings:Standard");

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

app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
