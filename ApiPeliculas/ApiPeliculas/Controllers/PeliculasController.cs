using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [Route("api/v{version:ApiVersion}/peliculas")]
    [ApiController]
    [ApiVersion("1.0")]
    public class PeliculasController : ControllerBase
    {
        private readonly IPeliculaRepositorio _peliculaRepositorio;
        private readonly IMapper _mapper;

        public PeliculasController(IPeliculaRepositorio peliculaRepo, IMapper mapper)
        {
            _peliculaRepositorio = peliculaRepo;
            _mapper = mapper;
        }

        [HttpGet("mostrarTodo")]
        [ResponseCache(CacheProfileName = "Default20segs")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[EnableCors("PoliticaCors")] //Aplica la politica CORS a este endpoint
        public IActionResult GetPelicula()
        {
            var listaPeliculas = _peliculaRepositorio.GetPeliculas();
            var listaPeliculasDto = new List<PeliculaDto>();
            foreach (var lista in listaPeliculas)
            {
                listaPeliculasDto.Add(_mapper.Map<PeliculaDto>(lista));
            }
            return Ok(listaPeliculasDto);
        }

        [HttpGet("mostrar/{peliculaId:int}", Name = "GetPelicula")]
        [ResponseCache(CacheProfileName = "Default30segs")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPelicula(int peliculaId)
        {
            var itemPelicula = _peliculaRepositorio.GetPelicula(peliculaId);

            if (itemPelicula == null)
            {
                return NotFound();
            }

            var itemPeliculaDto = _mapper.Map<PeliculaDto>(itemPelicula);
            return Ok(itemPeliculaDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PeliculaDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CrearPelicula([FromBody] CrearPeliculaDto crearPeliculaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (crearPeliculaDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_peliculaRepositorio.ExistePelicula(crearPeliculaDto.Titulo))
            {
                ModelState.AddModelError("", "La película ya existe");
                return StatusCode(404, ModelState);
            }

            var pelicula = _mapper.Map<Pelicula>(crearPeliculaDto);

            if (!_peliculaRepositorio.CrearPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Error al guardar el registro {pelicula.Titulo}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetPelicula", new { peliculaId = pelicula.Id }, pelicula);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("actualizar/{peliculaId:int}", Name = "PatchPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public IActionResult ActualizarPatchPelicula(int peliculaId, [FromBody] PeliculaDto peliculaDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (peliculaDto == null || peliculaId != peliculaDto.Id)
            {
                return BadRequest(ModelState);
            }
            var peliculaExistente = _peliculaRepositorio.GetPelicula(peliculaId);
            if (peliculaExistente == null)
            {
                return NotFound($"No se encontro la pelicula con ID {peliculaId}");
            }

            var pelicula = _mapper.Map<Pelicula>(peliculaDto);

            if (!_peliculaRepositorio.ActualizarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal al actualizar el registro {pelicula.Titulo}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("borrar/{peliculaId:int}", Name = "DeletePelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult EliminarPelicula(int peliculaId)
        {
            if (!_peliculaRepositorio.ExistePelicula(peliculaId))
            {
                return NotFound($"No se encontró la película con ID {peliculaId}");
            }

            var pelicula = _peliculaRepositorio.GetPelicula(peliculaId);

            if (!_peliculaRepositorio.EliminarPelicula(pelicula))
            {
                ModelState.AddModelError("", $"Algo salió mal al eliminar el registro {pelicula.Titulo}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpGet("MostrarPeliculaXIdCategoria/{categoriaId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult GetPeliculaCategoria(int categoriaId)
        {
            var listaPeliculas = _peliculaRepositorio.GetPeliculasCategoria(categoriaId);
            var listaPeliculasDto = new List<PeliculaDto>();
            foreach (var lista in listaPeliculas)
            {
                listaPeliculasDto.Add(_mapper.Map<PeliculaDto>(lista));
            }
            return Ok(listaPeliculasDto);
        }

        [HttpGet("buscar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult Buscar(string titulo)
        {

            try
            {
                var resultado = _peliculaRepositorio.BuscarPelicula(titulo);
                if (resultado.Any())
                {
                    return Ok(resultado);
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Error en el servidor");
            }
            return NotFound("No se encontraron resultados");
        }

    }
}
