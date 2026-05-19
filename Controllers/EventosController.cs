using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eventos.Data;
using Eventos.Models;

namespace Eventos.Controllers
{
    public class EventosController : Controller
    {
        private readonly EventosContext _context;

        public EventosController(EventosContext context)
        {
            _context = context;
        }

        // GET: Eventos
        public async Task<IActionResult> Index()
        {
			return View(await _context.Evento
	            .Include(e => e.EventoCategorias)
		        .ThenInclude(ec => ec.Categoria)
	            .Include(e => e.EventoPalestrantes)
		        .ThenInclude(ep => ep.Palestrante)
	            .ToListAsync());
		}

        // GET: Eventos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Evento
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

		public IActionResult Create()
		{
			ViewBag.Categorias = _context.Categoria.ToList();
			ViewBag.Palestrantes = _context.Palestrante.ToList();
			return View();
		}

		// POST: Eventos/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,Data,Horario,Local,Banner")] Evento evento,
	IFormFile bannerFile,
	int[] categoriasIds,      // IDs das categorias selecionadas
	int[] palestrantesIds)    // IDs dos palestrantes selecionados
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

				// Salva as categorias selecionadas
				foreach (var categoriaId in categoriasIds)
				{
					_context.EventoCategorias.Add(new EventoCategoria
					{
						EventoId = evento.Id,
						CategoriaId = categoriaId
					});
				}

				// Salva os palestrantes selecionados
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
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Evento.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }
            return View(evento);
        }

        // POST: Eventos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,Data,Horario,Local,Banner")] Evento evento)
        {
            if (id != evento.Id)
            {
                return NotFound();
            }

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
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(evento);
        }

        // GET: Eventos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Evento
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        // POST: Eventos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evento = await _context.Evento.FindAsync(id);
            if (evento != null)
            {
                _context.Evento.Remove(evento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventoExists(int id)
        {
            return _context.Evento.Any(e => e.Id == id);
        }
    }
}
