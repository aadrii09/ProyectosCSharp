using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;

namespace ApiPeliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _bd;

        //constructor
        public UsuarioRepositorio(ApplicationDbContext bd)
        {
            _bd = bd;
        }

        public Usuario GetUsuario(int UsuarioId)
        {
            return _bd.Usuario.FirstOrDefault(u => u.Id == UsuarioId);
        }

        public ICollection<Usuario> GetUsuarios()
        {
            return _bd.Usuario.OrderBy(u => u.NombreUsuario).ToList();
        }

        public bool IsUniqueUsuario(string nombreUsuario)
        {
            var usuarioBd = _bd.Usuario.FirstOrDefault(u => u.NombreUsuario.ToLower() == nombreUsuario.ToLower());
            if (usuarioBd == null)
            {
                return true; 
            }
            return false;
        }

        public Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto)
        {
            throw new NotImplementedException();
        }

        public Task<UsuarioDatosDto> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
            throw new NotImplementedException();
        }
    }
}
