using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using XSystem.Security.Cryptography;

namespace ApiPeliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _bd;
        private string claveSecreta;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; //para el manejo de roles
        private readonly IMapper _mapper;

        //constructor
        public UsuarioRepositorio(ApplicationDbContext bd, IConfiguration config, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _bd = bd;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
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
            // Cambio de AppUsuario a Usuario
            var usuarioBd = _bd.Usuario.FirstOrDefault(u => u.NombreUsuario.ToLower() == nombreUsuario.ToLower());
            if (usuarioBd == null)
            {
                return true;
            }
            return false;
        }

        public async Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto)
        {
            var passwordEnciptada = obtenermd5(usuarioLoginDto.Password);
            // Cambio de AppUsuario a Usuario y de UserName a NombreUsuario
            var usuario = _bd.Usuario.FirstOrDefault(u => u.NombreUsuario.ToLower() == usuarioLoginDto.NombreUsuario.ToLower());

            // Verificar si usuario es null antes de usar CheckPasswordAsync
            if (usuario == null)
            {
                return new UsuarioLoginRespuestaDto()
                {
                    Token = "",
                    Usuario = null
                };
            }

            bool isValid = await _userManager.CheckPasswordAsync(usuario, usuarioLoginDto.Password);

            //Validaamos si el usuario no existe con la combinadion de nombre y contraseña correcta
            if (!isValid)
            {
                return new UsuarioLoginRespuestaDto()
                {
                    Token = "",
                    Usuario = null
                };
            }

            //Aqui existe el usuario entonces podemosprocesar el login
            var roles = await _userManager.GetRolesAsync(usuario);
            var manejadoorToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(claveSecreta);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    // Cambio de UserName a NombreUsuario
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? usuario.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = manejadoorToken.CreateToken(tokenDescriptor);

            UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto()
            {
                Token = manejadoorToken.WriteToken(token),
                Usuario = _mapper.Map<UsuarioDatosDto>(usuario)
            };

            return usuarioLoginRespuestaDto;
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

        public void Eliminar(Usuario itemUsuario)
        {
            _bd.Usuario.Remove(itemUsuario);
            _bd.SaveChanges();
        }
    }
}
