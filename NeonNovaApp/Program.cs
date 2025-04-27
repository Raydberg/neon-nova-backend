using System.Text.Json.Serialization;
using DotNetEnv;
using Intrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NeonNovaApp.Extensions;
using NeonNovaApp.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Cargar variables de entorno
Env.Load();

// Configuración de base de datos
var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// CORS
var originsPermits = builder.Configuration.GetSection("origenesPermitidos").Get<string[]>()!;
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(optCors =>
    {
        optCors.WithOrigins(originsPermits).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});

// Autenticación y autorización
builder.Services.AddAuthenticationConfiguration();

// Servicios de aplicación
builder.Services.AddHttpServices();
builder.Services.AddApplicationServices();
builder.Services.AddMappingConfiguration();

// Documentación
builder.Services.AddDocumentationServices();

// Monitoreo
builder.Services.AddMonitoringServices();

builder.Services.AddControllers()
    //Manehar referencias circulares => ignorar cilcos
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    })
    ;
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

await DataSeeder.SeedUsers(app.Services);
// Middlewares

app.UseCors();

// Configuración de documentación
app.UseDocumentationMiddleware();

// Monitoreo
app.UseGlobalExceptionHandler();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMonitoringMiddleware();
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/scalar"));
app.Run();