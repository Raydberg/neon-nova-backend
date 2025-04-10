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
using System.Text;
using Domain.Entities;
using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NeonNovaApp.Services;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Cargar variables de entorno desde el archivo .env
Env.Load();

// Add services to the container.
var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

var originsPermits = builder.Configuration.GetSection("origenesPermitidos").Get<string[]>()!;

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(optCors => { optCors.WithOrigins(originsPermits).AllowAnyMethod().AllowAnyHeader(); });
});

//Configuracion de Identity
var KeyJwt = Environment.GetEnvironmentVariable("key_jwt");
builder.Services.AddIdentityCore<Users>(opt =>
    {
        opt.SignIn.RequireConfirmedEmail = true;
        opt.Password.RequiredLength = 6;
        opt.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<Users>>();
//Autenticar Usuarios
builder.Services.AddScoped<SignInManager<Users>>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication().AddJwtBearer(opt =>
{
    opt.MapInboundClaims = false;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        //Llave JWT
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KeyJwt!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("isAdmin", politica => politica.RequireClaim("isAdmin"));
    opt.AddPolicy("isUser", politica => politica.RequireClaim("isUser"));
    opt.AddPolicy("isInvited", politica => politica.RequireClaim("isInvited"));
});

// Register Services
builder.Services.AddScoped<IProductService, ProductService>();
/**
 * Transient ya que no necesitamos compartir el estado
 */
builder.Services.AddTransient<ICurrentUserService, CurrentUserService>();

// Register Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Configurar automapper en nuestra aplicacion
builder.Services.AddAutoMapper(
    Assembly.GetExecutingAssembly(),
    typeof(Application.Mappings.MappingProduct).Assembly,
    typeof(Application.Mappings.MappingUser).Assembly
);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// Documentacion Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    // Configuración de OpenAPI
});
builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

// Agregar health checks y métricas de Prometheus
builder.Services.AddHealthChecks()
    .ForwardToPrometheus();

var app = builder.Build();

app.UseSwagger(opt => { opt.RouteTemplate = "openapi/{documentName}.json"; });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "NeonNovaAPI v1");
        c.RoutePrefix = "swagger";
    });
// Documentacion con Scalar
    app.MapScalarApiReference(opt =>
    {
        opt
            .WithTitle("NeonNovaApi")
            .WithTheme(ScalarTheme.Mars)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
            .WithPreferredScheme("Api Scheme")
            .WithDefaultHttpClient(ScalarTarget.Http, ScalarClient.Http11)
            .WithTheme(ScalarTheme.BluePlanet)
            // .WithTheme(ScalarTheme.DeepSpace)
            ;
    });
}

builder.Services.AddHttpContextAccessor();
// Middlewares
app.UseGlobalExceptionHandler();
app.UseCors();
// Agregar métricas de Prometheus
app.UseRouting();
app.UseHttpMetrics();
app.UseMetricServer();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics();
    endpoints.MapHealthChecks("/health");
});
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/scalar"));
app.Run();