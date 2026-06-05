using System.ComponentModel.DataAnnotations;

namespace API_Usuario.DTOs
{
    public class LoginDTO
    {
        
        [Required]
        public string Correo { get; set; }

        [Required]
        public string Contrasena { get; set; }
    }
}
