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

builder.Services.AddDbContext<CatalogService.Infrastructure.Persistence.CatalogDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogDatabase")));

builder.Services.AddScoped<CatalogService.Application.Interfaces.ICategoryRepository, CatalogService.Infrastructure.Repositories.CategoryRepository>();
builder.Services.AddScoped<CatalogService.Application.Interfaces.ICategoryService, CatalogService.Application.Services.CategoryService>();

builder.Services.AddScoped<CatalogService.Application.Interfaces.IProductRepository, CatalogService.Infrastructure.Repositories.ProductRepository>();
builder.Services.AddScoped<CatalogService.Application.Interfaces.IProductService, CatalogService.Application.Services.ProductService>();

builder.Services.AddScoped<CatalogService.Application.Interfaces.IImageService, CatalogService.Infrastructure.Services.ImageService>();

builder.Services.AddHttpClient<CatalogService.Application.Interfaces.IInventoryServiceClient, CatalogService.Infrastructure.Clients.InventoryServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:InventoryService"] ?? throw new Exception("InventoryService URL is not configured."));
});

builder.Services.AddValidatorsFromAssemblyContaining<CatalogService.Application.Validators.CreateCategoryDtoValidator>(ServiceLifetime.Transient);
//builder.Services.AddValidatorsFromAssemblyContaining<CatalogService.Application.Validators.UpdateCategoryDtoValidator>(ServiceLifetime.Transient);

var app = builder.Build();

app.UseStaticFiles();

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
