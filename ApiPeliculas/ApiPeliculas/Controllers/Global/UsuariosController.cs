using System.Net;
using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers.Global
{
    [Route("api/v{version:ApiVersion}/usuarios")]
    [ApiController]
    //[ApiVersion("1.0")]
    [ApiVersionNeutral] //Version neutral, no se especifica version en el endpoint
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        protected RespuestaApi _respuestaApi;
        private readonly IMapper _mapper;

        public UsuariosController(IUsuarioRepositorio usuarioRepo, IMapper mapper)
        {
            _usuarioRepositorio = usuarioRepo;
            _mapper = mapper;
            _respuestaApi = new();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("mostrarTodo")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsuarios()
        {
            var listaUsuarios = _usuarioRepositorio.GetUsuarios();
            var listaUsuariosDto = new List<UsuarioDto>();
            foreach (var lista in listaUsuarios)
            {
                listaUsuariosDto.Add(_mapper.Map<UsuarioDto>(lista));
            }
            return Ok(listaUsuariosDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("mostrar/{usuarioId}", Name = "GetUsuario")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult GetUsuario(string usuarioId)
        {

            var itemUsuario = _usuarioRepositorio.GetUsuario(usuarioId);

            if (itemUsuario == null)
            {
                return NotFound();
            }

            var itemUsuarioDto = _mapper.Map<UsuarioDto>(itemUsuario);
            return Ok(itemUsuarioDto);
        }

        [HttpPost("registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDto usuarioRegistroDto)
        {
            bool validarNombreUsuarioUnico = _usuarioRepositorio.IsUniqueUsuario(usuarioRegistroDto.NombreUsuario);
            if (!validarNombreUsuarioUnico)
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMenssages.Add("El nombre de usuario ya existe");
                return BadRequest(_respuestaApi);
            }

            var usuario = await _usuarioRepositorio.Registro(usuarioRegistroDto);
            if (usuario == null)
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMenssages.Add("Error al registrar el usuario");
                return BadRequest(_respuestaApi);
            }

            // Añadir un return exitoso
            _respuestaApi.StatusCode = HttpStatusCode.OK;
            _respuestaApi.IsSuccess = true;
            return Ok(_respuestaApi);   
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto usuarioLoginDto)
        {
            var respuestaLogin = await _usuarioRepositorio.Login(usuarioLoginDto);
            if (respuestaLogin.Usuario == null || string.IsNullOrEmpty(respuestaLogin.Token))
            {
                _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                _respuestaApi.IsSuccess = false;
                _respuestaApi.ErrorMenssages.Add("El nombre de usuario o contraseña spn incorrectos");
                return BadRequest(_respuestaApi);
            }
                _respuestaApi.StatusCode = HttpStatusCode.OK;
                _respuestaApi.IsSuccess = true;
                _respuestaApi.Result = respuestaLogin;
                return Ok(_respuestaApi);           
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("eliminar/{usuarioId}", Name = "EliminarUsuario")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult EliminarUsuario(string usuarioId)
        {
            var appUsuario = _usuarioRepositorio.GetUsuario(usuarioId);
            if (appUsuario == null)
            {
                return NotFound();
            }

            _usuarioRepositorio.Eliminar(appUsuario);
            _respuestaApi.StatusCode = HttpStatusCode.NoContent;
            _respuestaApi.IsSuccess = true;
            return Ok(_respuestaApi);
        }

    }
}