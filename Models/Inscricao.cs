using Microsoft.AspNetCore.Identity;

namespace Eventos.Models
{
	public class Inscricao
	{
		public int Id { get; set; }
		public int EventoId { get; set; }
		public Evento Evento { get; set; }

		// Relacionamento com o Usuário do Identity
		public string IdentityUserId { get; set; }
		public IdentityUser IdentityUser { get; set; }

		public DateTime DataInscricao { get; set; } = DateTime.Now;
	}
}
