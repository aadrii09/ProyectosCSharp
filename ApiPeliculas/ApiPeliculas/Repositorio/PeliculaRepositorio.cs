using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Repositorio
{
    public class PeliculaRepositorio : IPeliculaRepositorio
    {
        private readonly ApplicationDbContext _bd;

        //constructor
        public PeliculaRepositorio(ApplicationDbContext bd)
        {
            _bd = bd;
        }

        public bool ActualizarPelicula(Pelicula pelicula)
        {
            pelicula.FechaEstreno = DateTime.Now;
            _bd.Pelicula.Update(pelicula); 
            return Guardar(); 
        }

        public IEnumerable<Pelicula> BuscarPelicula(string titulo)
        {
            IQueryable<Pelicula> query = _bd.Pelicula;
            if (!string.IsNullOrEmpty(titulo))
            {
                query = query.Where(e => e.Titulo.ToLower().Contains(titulo.ToLower()) || e.Descripcion.ToLower().Contains(titulo.ToLower()));
            }
            return query.ToList();
        }

        public bool CrearPelicula(Pelicula pelicula)
        {
            pelicula.FechaEstreno = DateTime.Now;
            _bd.Pelicula.Add(pelicula);
            return Guardar();
        }

        public bool EliminarPelicula(Pelicula pelicula)
        {
            _bd.Pelicula.Remove(pelicula);
            return Guardar();
        }

        public bool ExistePelicula(int Id)
        {
            return _bd.Pelicula.Any(c => c.Id == Id);
        }

        public bool ExistePelicula(string titulo)
        {
            bool valor = _bd.Pelicula.Any(c => c.Titulo.ToLower().Trim() == titulo.ToLower().Trim());
            return valor;
        }

        public Pelicula GetPelicula(int PeliculaId)
        {
            return _bd.Pelicula.FirstOrDefault(c => c.Id == PeliculaId);
        }

        public ICollection<Pelicula> GetPelicula()
        {
            return _bd.Pelicula.OrderBy(c => c.Titulo).ToList();
        }

        public ICollection<Pelicula> GetPeliculas()
        {
            return _bd.Pelicula.Include(ca => ca.Categoria).OrderBy(c => c.Titulo).ToList();
        }

        public ICollection<Pelicula> GetPeliculasCategoria(int categotiaId)
        {
            return _bd.Pelicula.Include(ca => ca.Categoria).Where(ca => ca.CategoriaId == categotiaId).ToList();
        }

        public bool Guardar()
        {
            return _bd.SaveChanges() >= 0 ? true : false;
        }   
    }
}
