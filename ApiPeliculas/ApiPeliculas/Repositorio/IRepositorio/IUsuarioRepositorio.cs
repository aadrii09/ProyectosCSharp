using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;

namespace ApiPeliculas.Repositorio.IRepositorio
{
    public interface IUsuarioRepositorio
    {
        ICollection<AppUsuario> GetUsuarios();
        AppUsuario GetUsuario(string UsuarioId);
        bool IsUniqueUsuario(string nombreUsuario);
        Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto);
        Task<UsuarioDatosDto> Registro(UsuarioRegistroDto usuarioRegistroDto);
        void Eliminar(AppUsuario appUsuario); 
    
    }
}
