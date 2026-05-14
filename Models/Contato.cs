using System.ComponentModel.DataAnnotations;

namespace Eventos.Models
{
    public class Contato
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Assunto { get; set; }
        [Required]
        public string Mensagem { get; set; }
        public DateTime DataEnvio { get; set; }
    }
}