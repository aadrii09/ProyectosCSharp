using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using AutoMapper;

namespace ApiPeliculas.PeliculasMapper
{
    public class PeliculasMapper :Profile
    {
        public PeliculasMapper()
        {
            CreateMap<Categoria, CategoriaDto>().ReverseMap();
            CreateMap<Categoria, CrearCategoriaDto>().ReverseMap();

            CreateMap<Pelicula, PeliculaDto>().ReverseMap();
            CreateMap<Pelicula, CrearPeliculaDto>().ReverseMap();

            CreateMap<Usuario, UsuarioDto>().ReverseMap();
            CreateMap<Usuario, UsuarioRegistroDto>().ReverseMap();
            CreateMap<Usuario, UsuarioLoginDto>().ReverseMap();
            CreateMap<AppUsuario, UsuarioDatosDto>().ReverseMap();



        }
    }
}
