using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPeliculas.Models.Dtos
{
    public class CrearPeliculaDto
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Duracion { get; set; }

        public string RutaImagen { get; set; }

        public enum CrearTipoClasificacion { tres, siete, trece, diecisiete, dieciocho }
        public CrearTipoClasificacion Clasificacion { get; set; }


       
        public int CategoriaId { get; set; }
     
    }
}
