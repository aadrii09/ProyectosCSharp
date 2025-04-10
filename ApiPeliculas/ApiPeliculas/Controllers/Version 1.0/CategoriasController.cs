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

namespace ApiPeliculas.Controllers.Version_1._0
{
    //[Authorize(Roles = "Admin")]  //Autorizacion global para todos los endopoints del controller
    //[ResponseCache(Duration= 20)] // Se puede poner aqui para que afecte de manera global a todos los endpoints
    [Route("api/v{version:ApiVersion}/categoria")]
    [ApiController]
    [ApiVersion("1.0")] //Version de la API
    //[ApiVersion("2.0")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepositorio _categoriaRepositorio;
        private readonly IMapper _mapper;

        public CategoriasController(ICategoriaRepositorio categoriaRepo, IMapper mapper)
        {
            _categoriaRepositorio = categoriaRepo;
            _mapper = mapper;
        }

        //[AllowAnonymous]  /eso sirve para poner publico el endpoint en el caso de que este todo el controller en privado
       
        [HttpGet("mostrarTodo")]
        //[ResponseCache(Duration = 20)]
        //[MapToApiVersion("1.0")] //Version de la API a escogen en el endpoint
        [ResponseCache(CacheProfileName= "Default20segs")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategorias()
        {
            var listaCategorias = _categoriaRepositorio.GetCategorias();
            var listaCategoriasDto = new List<CategoriaDto>();
            foreach (var lista in listaCategorias)
            {
                listaCategoriasDto.Add(_mapper.Map<CategoriaDto>(lista));
            }
            return Ok(listaCategoriasDto);
        }

        ////PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  
        [HttpGet("GetString")]
        [Obsolete("Este endpoint esta obsoleto, favor use la version 2.0")]
        //[MapToApiVersion("2.0")] //Version de la API a escogen en el endpoint
        public IEnumerable<string> Get()
        {
            return new string[] { "valor1", "valor2", "valor3" };
        }
        ////PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  PRUEBA  

        [HttpGet("mostrar/{categoriaId:int}", Name = "GetCategoria")]
        //[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [ResponseCache(CacheProfileName = "Default30segs")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult GetCategoria(int categoriaId) 
        {
                
            var itemCategoria = _categoriaRepositorio.GetCategoria(categoriaId);

            if(itemCategoria == null)
            {
                return NotFound();
            }

            var itemCategoriaDto = _mapper.Map<CategoriaDto>(itemCategoria);
            return Ok(itemCategoriaDto);  
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public IActionResult CrearCategoria([FromBody] CrearCategoriaDto crearCategoriaDto)
        {

          if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

          if(crearCategoriaDto== null)
            {
                return BadRequest(ModelState);
            }

          if(_categoriaRepositorio.ExisteCategoria(crearCategoriaDto.Nombre))
            {
                ModelState.AddModelError("", "La categoria ya existe");
                return StatusCode(404, ModelState);
            }

          var categoria = _mapper.Map<Categoria>(crearCategoriaDto);

            if(!_categoriaRepositorio.CrearCategoria(categoria))
            {
                ModelState.AddModelError("", $"Error al guardar el registro {categoria.Nombre}");
                return StatusCode(404, ModelState);
            }
            return CreatedAtRoute("GetCategoria", new { categoriaId = categoria.Id }, categoria);
        }


        [Authorize(Roles = "Admin")]
        [HttpPatch("actualizar/{categoriaId:int}", Name = "PatchCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public IActionResult ActualizarPatchCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (categoriaDto == null || categoriaId != categoriaDto.Id)
            {
                return BadRequest(ModelState);
            }
            var categoriaExistente = _categoriaRepositorio.GetCategoria(categoriaId);
            if (categoriaExistente == null)
            {
                return NotFound($"No se encontro la categoria con ID{categoriaId}");
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            if (!_categoriaRepositorio.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal al actualizar el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("actualizar/{categoriaId:int}", Name = "PutCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult ActualizarPutCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (categoriaDto == null || categoriaId != categoriaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var categoriaExistente = _categoriaRepositorio.GetCategoria(categoriaId);
            if (categoriaExistente == null)
            {
                return NotFound($"No se encontro la categoria con ID{categoriaId}");
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            if (!_categoriaRepositorio.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal al actualizar el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("borrar/{categoriaId:int}", Name = "DeleteCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult EliminarCategoria(int categoriaId)
        {
          
            if (!_categoriaRepositorio.ExisteCategoria(categoriaId))
            {
                return NotFound($"No se encontro la categoria con ID{categoriaId}");
            }
            

            var categoria = _categoriaRepositorio.GetCategoria(categoriaId);

            if(!_categoriaRepositorio.EliminarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal al eliminar el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
