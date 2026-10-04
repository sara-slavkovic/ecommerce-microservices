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

builder.Services.AddDbContext<OrderService.Infrastructure.Persistence.OrderDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("OrderDatabase")));

builder.Services.AddScoped<OrderService.Application.Interfaces.IOrderRepository, OrderService.Infrastructure.Repositories.OrderRepository>();
builder.Services.AddScoped<OrderService.Application.Interfaces.IOrderService, OrderService.Application.Services.OrderService>();

var internalApiKey = builder.Configuration["InternalApiKey"] ?? throw new ArgumentNullException("InternalApiKey is missing");

builder.Services.AddHttpClient<OrderService.Application.Interfaces.ICartServiceClient, OrderService.Infrastructure.Clients.CartServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:CartService"] ?? throw new InvalidOperationException("CartService URL is not configured."));
    client.DefaultRequestHeaders.Add("X-Internal-Api-Key", internalApiKey);
})
.AddCustomResilienceHandler(builder.Configuration, "ResilienceSettings:Standard");

builder.Services.AddHttpClient<OrderService.Application.Interfaces.IInventoryServiceClient, OrderService.Infrastructure.Clients.InventoryServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:InventoryService"] ?? throw new InvalidOperationException("InventoryService URL is not configured."));
    client.DefaultRequestHeaders.Add("X-Internal-Api-Key", internalApiKey);
})
.AddCustomResilienceHandler(builder.Configuration, "ResilienceSettings:Standard");

builder.Services.AddHttpClient<OrderService.Application.Interfaces.ICatalogServiceClient, OrderService.Infrastructure.Clients.CatalogServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:CatalogService"] ?? throw new InvalidOperationException("CatalogService URL is not configured."));
})
.AddCustomResilienceHandler(builder.Configuration, "ResilienceSettings:Standard");

builder.Services.AddHttpClient<OrderService.Application.Interfaces.IUserServiceClient, OrderService.Infrastructure.Clients.UserServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:UserService"] ?? throw new InvalidOperationException("UserService URL is not configured."));
    client.DefaultRequestHeaders.Add("X-Internal-Api-Key", internalApiKey);
})
.AddCustomResilienceHandler(builder.Configuration, "ResilienceSettings:Standard");

builder.Services.AddValidatorsFromAssemblyContaining<OrderService.Application.Validators.CreateOrderDtoValidator>(ServiceLifetime.Transient);

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
