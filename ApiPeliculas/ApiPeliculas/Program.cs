using ApiPeliculas.Data;
using ApiPeliculas.PeliculasMapper;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración CORS (permite todo, incluyendo 'null')
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", builder =>
    {
        builder.SetIsOriginAllowed(_ => true)  // Permite cualquier origen (incluso 'null')
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();  // Solo si usas autenticación/cookies
    });
});

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
app.UseCors("PermitirTodo");  // Aplica la política CORS aquí
app.UseAuthorization();
app.MapControllers();

app.Run();