using System.ComponentModel.DataAnnotations;

namespace API_Usuario.Models
{
    public class Proveedor
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } 

        [Required]
        [MaxLength(150)]
        public string Contacto { get; set; }

        public ICollection<Producto> Productos { get; set; }
            = new List<Producto>();
    }
}
