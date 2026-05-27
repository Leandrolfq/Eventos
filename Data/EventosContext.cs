using Eventos.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Data
{
	public class EventosContext : IdentityDbContext<IdentityUser>
	{
		public EventosContext(DbContextOptions<EventosContext> options)
			: base(options)
		{
		}

		public DbSet<Categoria> Categoria { get; set; } = default!;
		public DbSet<Contato> Contato { get; set; } = default!;
		public DbSet<Evento> Evento { get; set; } = default!;
		public DbSet<Palestrante> Palestrante { get; set; } = default!;
		public DbSet<EventoPalestrante> EventoPalestrantes { get; set; } = default!;
		public DbSet<EventoCategoria> EventoCategorias { get; set; } = default!;
		public DbSet<Inscricao> Inscricoes { get; set; } = default!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<EventoPalestrante>()
				.HasKey(ep => new { ep.EventoId, ep.PalestranteId });

			modelBuilder.Entity<EventoCategoria>()
				.HasKey(ec => new { ec.EventoId, ec.CategoriaId });
		}
	}
}