using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }



        // Relación con Pelicula, para hacerla bidireccional
        //public virtual ICollection<Pelicula> Peliculas { get; set; }

    }
}
