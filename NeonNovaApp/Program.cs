using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Domain.Interfaces;
using Intrastructure.Data;
using Intrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



//Register Services

builder.Services.AddScoped<IProductService,ProductService>();

//Register Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Configurar automapper en nuestra aplicacio
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
