using System.ComponentModel.DataAnnotations;

namespace API_Usuario.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        public ICollection<Producto> Productos { get; set; }
            = new List<Producto>();
    }
}
