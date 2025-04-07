using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [Route("api/peliculas")]
    [ApiController]
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
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategorias()
        {
            var listaPeliculas = _peliculaRepositorio.GetPeliculas();
            var listaPeliculasDto = new List<PeliculaDto>();
            foreach (var lista in listaPeliculas)
            {
                listaPeliculasDto.Add(_mapper.Map<PeliculaDto>(lista));
            }
            return Ok(listaPeliculasDto);
        }
    }
}
