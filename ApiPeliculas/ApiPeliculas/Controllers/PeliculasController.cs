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
    }
}
