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
mockGatewayClientBuilder.AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 3;
    options.Retry.BackoffType = Polly.DelayBackoffType.Exponential;
    options.Retry.Delay = TimeSpan.FromSeconds(1);
    options.Retry.UseJitter = true;

    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(3);
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);

    options.CircuitBreaker.FailureRatio = 0.5;
    options.CircuitBreaker.MinimumThroughput = 4;
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(60);
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);
});
// 3. Add Tracking Handler to the builder (Inner layer: executes on every single retry attempt)
mockGatewayClientBuilder.AddHttpMessageHandler<PaymentService.Infrastructure.Handlers.PaymentAttemptTrackingHandler>();

var internalApiKey = builder.Configuration["InternalApiKey"] ?? throw new ArgumentNullException("InternalApiKey is missing");

builder.Services.AddHttpClient<PaymentService.Application.Interfaces.IOrderServiceClient, PaymentService.Infrastructure.Clients.OrderServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:OrderService"] ?? throw new Exception("OrderService URL is not configured."));
    client.DefaultRequestHeaders.Add("X-Internal-Api-Key", internalApiKey);
}).AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 3;
    options.Retry.BackoffType = Polly.DelayBackoffType.Exponential;
    options.Retry.Delay = TimeSpan.FromSeconds(1);
    options.Retry.UseJitter = true;

    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(30);
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);

    options.CircuitBreaker.FailureRatio = 0.5;
    options.CircuitBreaker.MinimumThroughput = 4;
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(60);
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);
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

app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
