using System.Text;
using ApiPeliculas.Data;
using ApiPeliculas.PeliculasMapper;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

builder.Services.AddControllers();
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

// Repositorios
builder.Services.AddScoped<ICategoriaRepositorio, CategotiaRepositorio>();
builder.Services.AddScoped<IPeliculaRepositorio, PeliculaRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(PeliculasMapper));

var app = builder.Build();

// Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Importante: UseCors debe ir antes de UseAuthentication y UseAuthorization
app.UseCors("PoliticaCors");

// Soporte de la autenticación JWT
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
