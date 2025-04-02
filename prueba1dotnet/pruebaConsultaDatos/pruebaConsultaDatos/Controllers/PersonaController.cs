using Microsoft.AspNetCore.Mvc;
using pruebaConsultaDatos.Datos;

namespace pruebaConsultaDatos.Controllers
{
    public class PersonaController : Controller
    {
        public IActionResult Index()
        {
            return View(new PersonaAdmin().Consultar() );
        }
    }
}
