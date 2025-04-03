using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.Dtos
{
    public class CategoriaDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Elnombre es obligatorio")]
        [MaxLength(100, ErrorMessage = "El nombre no puede tener m�s de 100 caracteres")]
        public string Nombre { get; set; }

        public DateTime FechaCreacion { get; set; }
    } 
}
