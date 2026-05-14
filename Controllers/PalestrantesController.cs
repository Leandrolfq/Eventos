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
    public class PalestrantesController : Controller
    {
        private readonly EventosContext _context;

        public PalestrantesController(EventosContext context)
        {
            _context = context;
        }

        // GET: Palestrantes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Palestrante.ToListAsync());
        }

        // GET: Palestrantes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var palestrante = await _context.Palestrante
                .FirstOrDefaultAsync(m => m.Id == id);
            if (palestrante == null)
            {
                return NotFound();
            }

            return View(palestrante);
        }

        // GET: Palestrantes/Create
        public IActionResult Create()
        {
            return View();
        }

		// POST: Palestrantes/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,NomeCompleto,Empresa,Especialidade,MiniBiografia,Foto")] Palestrante palestrante, IFormFile fotoFile)
		{
			if (fotoFile != null && fotoFile.Length > 0)
			{
				var nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(fotoFile.FileName);
				var caminho = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", nomeArquivo);

				using (var stream = new FileStream(caminho, FileMode.Create))
				{
					await fotoFile.CopyToAsync(stream);
				}

				palestrante.Foto = "/images/" + nomeArquivo;
			}
			ModelState.Remove("Foto");
			if (ModelState.IsValid)
			{
				_context.Add(palestrante);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(palestrante);
		}

		// POST: Palestrantes/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomeCompleto,Empresa,Especialidade,MiniBiografia,Foto")] Palestrante palestrante)
        {
            if (id != palestrante.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(palestrante);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PalestranteExists(palestrante.Id))
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
            return View(palestrante);
        }

        // GET: Palestrantes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var palestrante = await _context.Palestrante
                .FirstOrDefaultAsync(m => m.Id == id);
            if (palestrante == null)
            {
                return NotFound();
            }

            return View(palestrante);
        }

        // POST: Palestrantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var palestrante = await _context.Palestrante.FindAsync(id);
            if (palestrante != null)
            {
                _context.Palestrante.Remove(palestrante);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PalestranteExists(int id)
        {
            return _context.Palestrante.Any(e => e.Id == id);
        }
    }
}
