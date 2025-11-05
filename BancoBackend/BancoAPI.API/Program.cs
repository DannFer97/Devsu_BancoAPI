using BancoAPI.Application.Interfaces;
using BancoAPI.Application.Mappings;
using BancoAPI.Application.Services;
using BancoAPI.Application.Configuration;
using BancoAPI.Domain.Interfaces;
using BancoAPI.Infrastructure.Data;
using BancoAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using BancoAPI.API.Middleware;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);


// 1. CONFIGURACION DE SERVICIOS


// DbContext con SQL Server
builder.Services.AddDbContext<BancoDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)));

// Dependency Injection - Patron Repository y Unit of Work 
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Configuración de PDF (Options Pattern)
builder.Services.Configure<PdfConfiguration>(
    builder.Configuration.GetSection("PdfConfiguration"));

// Dependency Injection - Servicios de Application
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<ICuentaService, CuentaService>();
builder.Services.AddScoped<IMovimientoService, MovimientoService>();
builder.Services.AddScoped<IPdfReportService, PdfReportService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configuración para manejar referencias circulares
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

        // Configuración para aceptar enums como strings (ej: "Retiro" en lugar de 1)
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = new Dictionary<string, string[]>();
            
            foreach (var modelError in context.ModelState)
            {
                var key = modelError.Key;
                var value = modelError.Value;
                
                if (value.Errors.Count > 0)
                {
                    errors[key] = value.Errors.Select(error => error.ErrorMessage).ToArray();
                }
            }
            
            var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Please refer to the errors property for additional details.",
                Instance = context.HttpContext.Request.Path
            };

            var result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(problemDetails)
            {
                ContentTypes = { "application/json" }
            };

            result.Value = new
            {
                problemDetails.Type,
                problemDetails.Title,
                problemDetails.Status,
                problemDetails.Detail,
                problemDetails.Instance,
                Errors = errors
            };

            return result;
        };
    });

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "BancoAPI",
        Version = "v1",
        Description = "API REST para sistema bancario - Prueba Tcnica para Devsu",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Banco API",
            Email = "contacto@banco.com"
        }
    });
});

// CORS 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();


// 2. CONFIGURACION DEL PIPELINE


// Middleware de excepciones global 
app.UseExceptionHandlerMiddleware();

// Swagger en todos los ambientes (para facilitar pruebas)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BancoAPI v1");
    c.RoutePrefix = string.Empty; 
});

// CORS
app.UseCors("AllowAll");

// Redireccion HTTPS 
app.UseHttpsRedirection();

// Autorizacion 
app.UseAuthorization();

// Controllers
app.MapControllers();


// 3. MIGRACIONES

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<BancoDbContext>();
        context.Database.Migrate(); // Aplica migraciones pendientes
        Console.WriteLine("Migraciones aplicadas exitosamente");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al aplicar migraciones");
    }
}

Console.WriteLine("BancoAPI iniciada en: " + app.Urls.FirstOrDefault());
app.Run();