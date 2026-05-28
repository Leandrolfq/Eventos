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

namespace Eventos.Controllers
{
	public class PalestrantesController : Controller
	{
		private readonly EventosContext _context;

		public PalestrantesController(EventosContext context)
		{
			_context = context;
		}

		// GET: Palestrantes - público para todos
		[AllowAnonymous]
		public async Task<IActionResult> Index(string? nome, string? empresa, string? especialidade)
		{
			var palestrantes = _context.Palestrante.AsQueryable();

			if (!string.IsNullOrEmpty(nome))
				palestrantes = palestrantes.Where(p => p.NomeCompleto.Contains(nome));

			if (!string.IsNullOrEmpty(empresa))
				palestrantes = palestrantes.Where(p => p.Empresa.Contains(empresa));

			if (!string.IsNullOrEmpty(especialidade))
				palestrantes = palestrantes.Where(p => p.Especialidade.Contains(especialidade));

			ViewBag.NomeBusca = nome;
			ViewBag.EmpresaBusca = empresa;
			ViewBag.EspecialidadeBusca = especialidade;

			return View(await palestrantes.ToListAsync());
		}

		// GET: Palestrantes/Details/5
		[AllowAnonymous]
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
				return NotFound();

			var palestrante = await _context.Palestrante
				.Include(p => p.EventoPalestrantes)
					.ThenInclude(ep => ep.Evento)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (palestrante == null)
				return NotFound();

			return View(palestrante);
		}

		// GET: Palestrantes/Create
		[Authorize(Roles = "Admin")]
		public IActionResult Create()
		{
			return View();
		}

		// POST: Palestrantes/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Create([Bind("Id,NomeCompleto,Empresa,Cargo,Especialidade,MiniBiografia")] Palestrante palestrante, IFormFile Foto)
		{
			ModelState.Remove("Foto");
			if (ModelState.IsValid)
			{
				if (Foto != null && Foto.Length > 0)
				{
					var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/palestrantes");
					Directory.CreateDirectory(pasta);
					var nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(Foto.FileName);
					var caminho = Path.Combine(pasta, nomeArquivo);
					using (var stream = new FileStream(caminho, FileMode.Create))
					{
						await Foto.CopyToAsync(stream);
					}
					palestrante.Foto = "/images/palestrantes/" + nomeArquivo;
				}

				_context.Add(palestrante);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(palestrante);
		}

		// GET: Palestrantes/Edit/5
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null) return NotFound();
			var palestrante = await _context.Palestrante.FindAsync(id);
			if (palestrante == null) return NotFound();
			return View(palestrante);
		}

		// POST: Palestrantes/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(int id, [Bind("Id,NomeCompleto,Empresa,Cargo,Especialidade,MiniBiografia,Foto")] Palestrante palestrante, IFormFile? FotoFile)
		{
			if (id != palestrante.Id) return NotFound();

			ModelState.Remove("Foto");

			if (ModelState.IsValid)
			{
				if (FotoFile != null && FotoFile.Length > 0)
				{
					var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/palestrantes");
					Directory.CreateDirectory(pasta);
					var nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(FotoFile.FileName);
					var caminho = Path.Combine(pasta, nomeArquivo);
					using (var stream = new FileStream(caminho, FileMode.Create))
					{
						await FotoFile.CopyToAsync(stream);
					}
					palestrante.Foto = "/images/palestrantes/" + nomeArquivo;
				}

				try
				{
					_context.Update(palestrante);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!PalestranteExists(palestrante.Id)) return NotFound();
					else throw;
				}
				return RedirectToAction(nameof(Index));
			}
			return View(palestrante);
		}

		// GET: Palestrantes/Delete/5
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
				return NotFound();

			var palestrante = await _context.Palestrante
				.FirstOrDefaultAsync(m => m.Id == id);
			if (palestrante == null)
				return NotFound();

			return View(palestrante);
		}

		// POST: Palestrantes/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var palestrante = await _context.Palestrante.FindAsync(id);
			if (palestrante != null)
				_context.Palestrante.Remove(palestrante);

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool PalestranteExists(int id)
		{
			return _context.Palestrante.Any(e => e.Id == id);
		}
	}
}