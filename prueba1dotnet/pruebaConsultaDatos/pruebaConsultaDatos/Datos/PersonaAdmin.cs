using Foundation.ObjectHydrator;
using pruebaConsultaDatos.Models;

namespace pruebaConsultaDatos.Datos
{
    public class PersonaAdmin
    {
        public IEnumerable<Persona> Consultar()
        {
            var listado = new Hydrator<Persona>()
                .WithFirstName(n => n.Nombre)
                .WithLastName(a => a.Apellido)
                .WithInteger(s => s.Edad, 18, 60); 
            return listado.GetList(10).ToList(); 
        }
    }
}

    