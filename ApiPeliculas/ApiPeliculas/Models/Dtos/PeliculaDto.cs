using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPeliculas.Models.Dtos
{
    public class PeliculaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Duracion { get; set; }

        public string RutaImagen { get; set; }

        public enum TipoClasificacion { tres, siete, trece, diecisiete, dieciocho }
        public TipoClasificacion Clasificacion { get; set; }
        public DateTime FechaEstreno { get; set; }


       
        public int CategoriaId { get; set; }
     
    }
}
