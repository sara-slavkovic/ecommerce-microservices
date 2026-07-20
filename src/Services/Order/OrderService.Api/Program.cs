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

builder.Services.AddDbContext<OrderService.Infrastructure.Persistence.OrderDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("OrderDatabase")));

builder.Services.AddScoped<OrderService.Application.Interfaces.IOrderRepository, OrderService.Infrastructure.Repositories.OrderRepository>();
builder.Services.AddScoped<OrderService.Application.Interfaces.IOrderService, OrderService.Application.Services.OrderService>();

var internalApiKey = builder.Configuration["InternalApiKey"] ?? throw new ArgumentNullException("InternalApiKey is missing");

builder.Services.AddHttpClient<OrderService.Application.Interfaces.ICartServiceClient, OrderService.Infrastructure.Clients.CartServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:CartService"] ?? throw new InvalidOperationException("CartService URL is not configured."));
    client.DefaultRequestHeaders.Add("X-Internal-Api-Key", internalApiKey);
});

builder.Services.AddHttpClient<OrderService.Application.Interfaces.IInventoryServiceClient, OrderService.Infrastructure.Clients.InventoryServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:InventoryService"] ?? throw new InvalidOperationException("InventoryService URL is not configured."));
    client.DefaultRequestHeaders.Add("X-Internal-Api-Key", internalApiKey);
});

builder.Services.AddHttpClient<OrderService.Application.Interfaces.ICatalogServiceClient, OrderService.Infrastructure.Clients.CatalogServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:CatalogService"] ?? throw new Exception("CatalogService URL is not configured."));
});

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
