using CartService.Infrastructure.Persistence;
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

builder.Services.AddDbContext<CartService.Infrastructure.Persistence.CartDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CartDatabase")));

builder.Services.AddScoped<CartService.Application.Interfaces.ICartRepository, CartService.Infrastructure.Repositories.CartRepository>();
builder.Services.AddScoped<CartService.Application.Interfaces.ICartService, CartService.Application.Services.CartService>();

builder.Services.AddHttpClient<CartService.Application.Interfaces.ICatalogServiceClient, CartService.Infrastructure.Clients.CatalogServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:CatalogService"] ?? throw new Exception("CatalogService URL is not configured."));
});

builder.Services.AddHttpClient<CartService.Application.Interfaces.IInventoryServiceClient, CartService.Infrastructure.Clients.InventoryServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:InventoryService"] ?? throw new Exception("InventoryService URL is not configured."));
});

builder.Services.AddValidatorsFromAssemblyContaining<CartService.Application.Validators.CreateCartItemDtoValidator>(ServiceLifetime.Transient);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
