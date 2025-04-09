using System.Net;
using Microsoft.OpenApi.MicrosoftExtensions;

namespace ApiPeliculas.Models
{
    public class RespuestaApi
    {
        public RespuestaApi()
        {
            ErrorMenssages = new List<string>();
        }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> ErrorMenssages { get; set; }
        public object Result { get; set; }
    }
}
