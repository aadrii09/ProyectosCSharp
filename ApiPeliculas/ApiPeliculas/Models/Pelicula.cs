using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace ApiPeliculas.Models
{
    public class Pelicula
    {
        [Key]
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }  
        public int Duracion { get; set; }

        public string? RutaImagen { get; set; } 

        public string? RutaLocalImagen { get; set; } 

        public enum TipoClasificacion { tres, siete, trece, diecisiete, dieciocho }
        public TipoClasificacion Clasificacion {get; set; }    
        public DateTime FechaEstreno { get; set; }


        //Relaccion con Categoria
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public virtual Categoria Categoria { get; set; }
    }
}
