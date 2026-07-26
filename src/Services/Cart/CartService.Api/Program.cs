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

builder.Services.AddDbContext<CartService.Infrastructure.Persistence.CartDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CartDatabase")));

builder.Services.AddScoped<CartService.Application.Interfaces.ICartRepository, CartService.Infrastructure.Repositories.CartRepository>();
builder.Services.AddScoped<CartService.Application.Interfaces.ICartService, CartService.Application.Services.CartService>();

var internalApiKey = builder.Configuration["InternalApiKey"] ?? throw new ArgumentNullException("InternalApiKey is missing");

// 1. Add Memory Cache to the container
builder.Services.AddMemoryCache();

// 2. Register the raw HTTP client using just the concrete class
// This keeps exact Polly resilience pipeline attached to the real network calls
builder.Services.AddHttpClient<CartService.Infrastructure.Clients.CatalogServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:CatalogService"] ?? throw new InvalidOperationException("CatalogService URL is not configured."));
})
.AddStandardResilienceHandler(options =>
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

// 3. Register the Interface to use the Cache Decorator, passing the raw client inside it
builder.Services.AddScoped<CartService.Application.Interfaces.ICatalogServiceClient>(provider =>
{
    // Get the raw HTTP client (with Polly attached)
    var rawClient = provider.GetRequiredService<CartService.Infrastructure.Clients.CatalogServiceClient>();
    // Get the Memory Cache
    var cache = provider.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();

    // Return the decorator
    return new CartService.Infrastructure.Clients.CachedCatalogServiceClient(rawClient, cache);
});

builder.Services.AddHttpClient<CartService.Application.Interfaces.IInventoryServiceClient, CartService.Infrastructure.Clients.InventoryServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:InventoryService"] ?? throw new InvalidOperationException("InventoryService URL is not configured."));
    client.DefaultRequestHeaders.Add("X-Internal-Api-Key", internalApiKey);
})
.AddStandardResilienceHandler(options =>
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

builder.Services.AddHttpClient<CartService.Application.Interfaces.IUserServiceClient, CartService.Infrastructure.Clients.UserServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:UserService"] ?? throw new InvalidOperationException("UserService URL is not configured."));
    client.DefaultRequestHeaders.Add("X-Internal-Api-Key", internalApiKey);
})
.AddStandardResilienceHandler(options =>
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

builder.Services.AddValidatorsFromAssemblyContaining<CartService.Application.Validators.CreateCartItemDtoValidator>(ServiceLifetime.Transient);

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
