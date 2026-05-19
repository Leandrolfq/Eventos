using System.ComponentModel.DataAnnotations;

namespace Eventos.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }

		
	}
}