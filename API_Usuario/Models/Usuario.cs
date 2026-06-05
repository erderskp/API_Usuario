using System.ComponentModel.DataAnnotations;

namespace API_Usuario.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [MinLength(3)]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Correo { get; set; }

        [Required]
        [MinLength(6)]
        public string Contrasena { get; set; }

        public DateTime FechaDeNacimiento { get; set; }
    }
}
