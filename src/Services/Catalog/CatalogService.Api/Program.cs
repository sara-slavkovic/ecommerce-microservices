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

var internalApiKey = builder.Configuration["InternalApiKey"] ?? throw new ArgumentNullException("InternalApiKey is missing");

builder.Services.AddHttpClient<CatalogService.Application.Interfaces.IInventoryServiceClient, CatalogService.Infrastructure.Clients.InventoryServiceClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:InventoryService"] ?? throw new InvalidOperationException("InventoryService URL is not configured."));
    client.DefaultRequestHeaders.Add("X-Internal-Api-Key", internalApiKey);
});

builder.Services.AddValidatorsFromAssemblyContaining<CatalogService.Application.Validators.CreateCategoryDtoValidator>(ServiceLifetime.Transient);
//builder.Services.AddValidatorsFromAssemblyContaining<CatalogService.Application.Validators.UpdateCategoryDtoValidator>(ServiceLifetime.Transient);

builder.Services.AddExceptionHandler<SharedKernel.Web.ExceptionHandlers.ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<SharedKernel.Web.ExceptionHandlers.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseStaticFiles();

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
