using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eventos.Data;
using Eventos.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims; // Adicionado para buscar o ID do usuário

namespace Eventos.Controllers
{
	public class EventosController : Controller
	{
		private readonly EventosContext _context;

		public EventosController(EventosContext context)
		{
			_context = context;
		}

		// GET: Eventos - público para todos
		[AllowAnonymous]
		public async Task<IActionResult> Index(string? nome, int? categoriaId, int? palestranteId)
		{
			var eventos = _context.Evento
				.Include(e => e.EventoCategorias).ThenInclude(ec => ec.Categoria)
				.Include(e => e.EventoPalestrantes).ThenInclude(ep => ep.Palestrante)
				.AsQueryable();

			if (!string.IsNullOrEmpty(nome))
				eventos = eventos.Where(e => e.Nome.Contains(nome));

			if (categoriaId.HasValue)
				eventos = eventos.Where(e => e.EventoCategorias.Any(ec => ec.CategoriaId == categoriaId));

			if (palestranteId.HasValue)
				eventos = eventos.Where(e => e.EventoPalestrantes.Any(ep => ep.PalestranteId == palestranteId));

			// --- LÓGICA DE INSCRIÇÃO DIRETA NO CARD ---
			var eventosInscritosIds = new List<int>();
			if (User.Identity.IsAuthenticated)
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				// Busca a lista de IDs dos eventos que este usuário já se inscreveu
				eventosInscritosIds = await _context.Inscricoes
					.Where(i => i.IdentityUserId == userId)
					.Select(i => i.EventoId)
					.ToListAsync();
			}
			ViewBag.EventosInscritosIds = eventosInscritosIds;
			// ------------------------------------------

			ViewBag.Categorias = await _context.Categoria.ToListAsync();
			ViewBag.Palestrantes = await _context.Palestrante.ToListAsync();
			ViewBag.NomeBusca = nome;
			ViewBag.CategoriaId = categoriaId;
			ViewBag.PalestranteId = palestranteId;

			return View(await eventos.ToListAsync());
		}

		// GET: Eventos/Details/5 - público para todos
		[AllowAnonymous]
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
				return NotFound();

			var evento = await _context.Evento
				.Include(e => e.EventoCategorias).ThenInclude(ec => ec.Categoria)
				.Include(e => e.EventoPalestrantes).ThenInclude(ep => ep.Palestrante)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (evento == null)
				return NotFound();

			// Lógica para verificar se o usuário logado já está inscrito
			bool jaInscrito = false;
			if (User.Identity.IsAuthenticated)
			{
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				jaInscrito = await _context.Inscricoes
					.AnyAsync(i => i.EventoId == id && i.IdentityUserId == userId);
			}
			ViewBag.JaInscrito = jaInscrito;

			return View(evento);
		}

		// GET: Eventos/Create
		[Authorize(Roles = "Admin")]
		public IActionResult Create()
		{
			ViewBag.Categorias = _context.Categoria.ToList();
			ViewBag.Palestrantes = _context.Palestrante.ToList();
			return View();
		}

		// POST: Eventos/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,Data,Horario,Local,Banner")] Evento evento,
			IFormFile bannerFile,
			int[] categoriasIds,
			int[] palestrantesIds)
		{
			if (bannerFile != null && bannerFile.Length > 0)
			{
				var nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(bannerFile.FileName);
				var caminho = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", nomeArquivo);
				using (var stream = new FileStream(caminho, FileMode.Create))
				{
					await bannerFile.CopyToAsync(stream);
				}
				evento.Banner = "/images/" + nomeArquivo;
			}

			ModelState.Remove("Banner");
			if (ModelState.IsValid)
			{
				_context.Add(evento);
				await _context.SaveChangesAsync();

				foreach (var categoriaId in categoriasIds)
				{
					_context.EventoCategorias.Add(new EventoCategoria
					{
						EventoId = evento.Id,
						CategoriaId = categoriaId
					});
				}

				foreach (var palestranteId in palestrantesIds)
				{
					_context.EventoPalestrantes.Add(new EventoPalestrante
					{
						EventoId = evento.Id,
						PalestranteId = palestranteId
					});
				}

				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(evento);
		}

		// GET: Eventos/Edit/5
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
				return NotFound();

			var evento = await _context.Evento.FindAsync(id);
			if (evento == null)
				return NotFound();

			return View(evento);
		}

		// POST: Eventos/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,Data,Horario,Local,Banner")] Evento evento)
		{
			if (id != evento.Id)
				return NotFound();

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(evento);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!EventoExists(evento.Id))
						return NotFound();
					else
						throw;
				}
				return RedirectToAction(nameof(Index));
			}
			return View(evento);
		}

		// GET: Eventos/Delete/5
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
				return NotFound();

			var evento = await _context.Evento
				.FirstOrDefaultAsync(m => m.Id == id);
			if (evento == null)
				return NotFound();

			return View(evento);
		}

		// POST: Eventos/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var evento = await _context.Evento.FindAsync(id);
			if (evento != null)
				_context.Evento.Remove(evento);

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		// POST: Eventos/Inscrever
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Inscrever(int eventoId)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (string.IsNullOrEmpty(userId))
			{
				return Challenge();
			}

			var eventoExists = await _context.Evento.AnyAsync(e => e.Id == eventoId);
			if (!eventoExists)
			{
				return NotFound();
			}

			var jaInscrito = await _context.Inscricoes
				.AnyAsync(i => i.EventoId == eventoId && i.IdentityUserId == userId);

			if (jaInscrito)
			{
				TempData["MensagemErro"] = "Você já está inscrito neste evento!";
				return RedirectToAction(nameof(Index)); // Modificado para voltar para a listagem
			}

			var inscricao = new Inscricao
			{
				EventoId = eventoId,
				IdentityUserId = userId,
				DataInscricao = DateTime.Now
			};

			_context.Inscricoes.Add(inscricao);
			await _context.SaveChangesAsync();

			TempData["MensagemSucesso"] = "Inscrição realizada com sucesso!";
			return RedirectToAction(nameof(Index)); // Modificado para voltar para a listagem
		}

		private bool EventoExists(int id)
		{
			return _context.Evento.Any(e => e.Id == id);
		}
	}
}