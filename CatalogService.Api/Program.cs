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
