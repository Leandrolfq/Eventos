using System.ComponentModel.DataAnnotations;

namespace Eventos.Models
{
    public class Evento
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public string Descricao { get; set; }
        [Required]
        public DateTime Data { get; set; }
        [Required]
        public string Horario { get; set; }
        [Required]
        public string Local { get; set; }
        [Required]
        public string Banner { get; set; }

    }
}