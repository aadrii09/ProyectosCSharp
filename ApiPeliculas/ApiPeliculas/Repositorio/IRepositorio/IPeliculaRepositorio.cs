using ApiPeliculas.Models;

namespace ApiPeliculas.Repositorio.IRepositorio
{
    public interface IPeliculaRepositorio
    {
        ICollection<Pelicula> GetPeliculas();
        ICollection<Pelicula> GetPeliculasCategoria(int categotiaId);
        IEnumerable<Pelicula> BuscarPelicula(string titulo);
        Pelicula GetPelicula(int PeliculaId);
        bool ExistePelicula(int Id);
        bool ExistePelicula(string titulo);
        bool CrearPelicula(Pelicula pelicula);
        bool ActualizarPelicula(Pelicula pelicula);
        bool EliminarPelicula(Pelicula pelicula);
        bool Guardar();
    }
} 
