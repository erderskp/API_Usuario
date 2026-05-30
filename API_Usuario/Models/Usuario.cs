using System.ComponentModel.DataAnnotations;

namespace API_Usuario.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        public DateTime FechaDeNacimiento { get; set; }
    }
}
