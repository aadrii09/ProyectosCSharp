using ApiPeliculas.Modelos.Dtos;
using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XAct.Diagnostics;

namespace ApiPeliculas.Controllers.Version_2._0
{
    //[Authorize(Roles = "Admin")]  //Autorizacion global para todos los endopoints del controller
    //[ResponseCache(Duration= 20)] // Se puede poner aqui para que afecte de manera global a todos los endpoints
    [Route("api/v{version:ApiVersion}/categoria")]
    [ApiController]
    [ApiVersion("2.0")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepositorio _categoriaRepositorio;
        private readonly IMapper _mapper;

        public CategoriasController(ICategoriaRepositorio categoriaRepo, IMapper mapper)
        {
            _categoriaRepositorio = categoriaRepo;
            _mapper = mapper;
        }

        

//PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA
        [HttpGet("GetString")]
         //Version de la API a escogen en el endpoint
        public IEnumerable<string> Get()
        {
            return new string[] { "valor1", "valor2", "valor3" };
        }
//PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA

        
    }
}
