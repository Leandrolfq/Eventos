using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Eventos.Models;

namespace Eventos.Data
{
    public class EventosContext : DbContext
    {
        public EventosContext (DbContextOptions<EventosContext> options)
            : base(options)
        {
        }

        public DbSet<Eventos.Models.Categoria> Categoria { get; set; } = default!;
        public DbSet<Eventos.Models.Contato> Contato { get; set; } = default!;
        public DbSet<Eventos.Models.Evento> Evento { get; set; } = default!;
        public DbSet<Eventos.Models.Palestrante> Palestrante { get; set; } = default!;
        public DbSet<Eventos.Models.EventoPalestrante> EventoPalestrantes { get; set; } = default!;
		public DbSet<Eventos.Models.EventoCategoria> EventoCategorias { get; set; } = default!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<EventoPalestrante>()
				.HasKey(ep => new { ep.EventoId, ep.PalestranteId });

			modelBuilder.Entity<EventoCategoria>()
				.HasKey(ec => new { ec.EventoId, ec.CategoriaId });
		}
	}

}

