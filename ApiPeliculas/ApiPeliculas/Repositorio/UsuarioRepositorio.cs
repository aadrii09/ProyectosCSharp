using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using XSystem.Security.Cryptography;

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

        public async Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
            var passwordEncriptada = obtenermd5(usuarioRegistroDto.Password);

            Usuario usuario = new Usuario()
            {
                NombreUsuario = usuarioRegistroDto.NombreUsuario,
                Password = passwordEncriptada,
                Nombre = usuarioRegistroDto.Nombre,
                Role = usuarioRegistroDto.Role,
            };

            _bd.Usuario.Add(usuario);
            await _bd.SaveChangesAsync();
            usuario.Password = passwordEncriptada;
            return usuario;
        }

        //Metodo para encriptar la contraseña con MD5 se usa tanto en el Login como en el Registro
        public static string obtenermd5(string valor)
        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
            data = x.ComputeHash(data);
            string resp = "";
            for (int i = 0; i < data.Length; i++)
            {
                resp += data[i].ToString("x2").ToLower();
            }
            return resp;
        }

    }
}
