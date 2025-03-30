using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Domain.Interfaces;
using Intrastructure.Data;
using Intrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using NeonNovaApp.Middleware;
using Scalar.AspNetCore;
using System.Reflection;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Register Services
builder.Services.AddScoped<IProductService, ProductService>();

//Register Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Configurar automapper en nuestra aplicacio
builder.Services.AddAutoMapper(
    Assembly.GetExecutingAssembly(),
    typeof(Application.Mappings.MappingProduct).Assembly
//typeof(OtherNamespace.OtherMapping).Assembly
);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{

});

// Agregar health checks y métricas de Prometheus
builder.Services.AddHealthChecks()
    .ForwardToPrometheus();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options
        .WithTitle("NeonNovaApi")
        .WithTheme(ScalarTheme.Mars)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
        .WithPreferredScheme("Api Scheme");
    });
}

// Middlewares
app.UseGlobalExceptionHandler();

// Agregar métricas de Prometheus
app.UseRouting();
app.UseHttpMetrics(); // Añadir esta línea
app.UseMetricServer(); // Añadir esta línea

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics(); 
    endpoints.MapHealthChecks("/health");
});

app.MapGet("/", () => Results.Ok(new { status = "API en funcionamiento", version = "1.0" }));
app.Run();