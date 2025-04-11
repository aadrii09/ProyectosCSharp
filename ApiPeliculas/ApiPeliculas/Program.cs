using System.Text;
using ApiPeliculas.Data;
using ApiPeliculas.Middleware; // Añadir esta línea para importar el namespace
using ApiPeliculas.Models;
using ApiPeliculas.PeliculasMapper;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");

//// Verifica que la clave tenga al menos 32 caracteres para HMAC-SHA256
//if (string.IsNullOrEmpty(key) || Encoding.ASCII.GetBytes(key).Length < 32)
//{
//    throw new ArgumentException("La clave secreta en ApiSettings:Secreta debe tener al menos 32 caracteres");
//}

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));

builder.Services.AddControllers(opcion =>
{
    //Cache profile. Cache global para los endpoints
    opcion.CacheProfiles.Add("Default20segs", new CacheProfile() { Duration = 20 });
    opcion.CacheProfiles.Add("Default30segs", new CacheProfile() { Duration = 30 });
});
builder.Services.AddEndpointsApiExplorer();
// Configuración de Swagger para integrar la autenticación JWT
// Esto añade un botón de autorización en la interfaz de Swagger UI que permite 
// introducir tokens JWT para probar endpoints protegidos con [Authorize]
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
        "JWT Authorization header usando el esquema 'Bearer'. \n\r\n\r" +
        "Ingrese 'Bearer' [espacio] y luego su token en el campo de texto a continuación.\n\r\n\r" +
        "Ejemplo: \"Bearer tutoken\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ApiPeliculas V1",
        Version = "v1.0",
        Description = "API de Peliculas",
        TermsOfService = new Uri("https://nombrekquiera.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Soporte API",
            Url = new Uri("https://nombrekquiera.com/support"),
        },
        License = new OpenApiLicense
        {
            Name = "Licencia de API",
            Url = new Uri("https://nombrekquiera.com/license"),
        }
    }
    );
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "ApiPeliculas V2",
        Version = "v2.0",
        Description = "API de Peliculas v2",
        TermsOfService = new Uri("https://nombrekquiera.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Soporte API",
            Url = new Uri("https://nombrekquiera.com/support"),
        },
        License = new OpenApiLicense
        {
            Name = "Licencia de API",
            Url = new Uri("https://nombrekquiera.com/license"),
        }
    });
}
);

//Configuración de la autenticación JWT
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

//Politicas de CORS para permitir el acceso a la API desde el cliente
builder.Services.AddCors(p => p.AddPolicy("PoliticaCors", build =>
{
    build.WithOrigins("http://localhost:5500", "http://127.0.0.1:5500")
         .AllowAnyMethod()
         .AllowAnyHeader();
}));

//Soporte para autenticacion con .NET Identity
builder.Services.AddIdentity<AppUsuario, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

//Soporte para cache
var apiVersioningBuilder = builder.Services.AddApiVersioning(opcion =>
{
    opcion.AssumeDefaultVersionWhenUnspecified = true; //al ponerlo en false, no se asume la version por defecto y es necesario especidicar la version
    opcion.DefaultApiVersion = new ApiVersion(1, 0);
    opcion.ReportApiVersions = true;
    //opcion.ApiVersionReader =  ApiVersionReader.Combine(
    //    new QueryStringApiVersionReader("api-version")
    //);
});

apiVersioningBuilder.AddApiExplorer(opcion =>
{
    opcion.GroupNameFormat = "'v'VVV";
    opcion.SubstituteApiVersionInUrl = true; //Esto permite que la version se vea en la URL
});

// Repositorios
builder.Services.AddScoped<ICategoriaRepositorio, CategotiaRepositorio>();
builder.Services.AddScoped<IPeliculaRepositorio, PeliculaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

//Soporte para el versionamiento
builder.Services.AddApiVersioning();

// AutoMapper
builder.Services.AddAutoMapper(typeof(PeliculasMapper));

var app = builder.Build();

// Registrar el middleware de excepciones antes que otros middlewares para saber si la bbdd esta bien
app.UseExceptionHandlerMiddleware(); // Añadir esta línea para usar el middleware personalizado

// Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opciones =>
    {
        opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiPeliculasV1");
        opciones.SwaggerEndpoint("/swagger/v2/swagger.json", "ApiPeliculasV2");
        //opciones.RoutePrefix = string.Empty; // Esto hace que Swagger UI esté disponible en la raíz de la aplicación
    });
}

app.UseHttpsRedirection();

// Importante: UseCors debe ir antes de UseAuthentication y UseAuthorization
app.UseCors("PoliticaCors");

// Soporte de la autenticación JWT
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
