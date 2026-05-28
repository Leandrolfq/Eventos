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
	[Authorize]
	public class ContatosController : Controller
	{
		private readonly EventosContext _context;

		public ContatosController(EventosContext context)
		{
			_context = context;
		}

		// GET: Contatos — somente Admin
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Index()
		{
			var contatos = await _context.Contato
				.OrderByDescending(c => c.DataEnvio) // ← ordenação por data
				.ToListAsync();
			return View(contatos);
		}

		// GET: Contatos/Details/5 — somente Admin
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null) return NotFound();

			var contato = await _context.Contato
				.FirstOrDefaultAsync(m => m.Id == id);
			if (contato == null) return NotFound();

			return View(contato);
		}

		// GET: Contatos/Create — público para visitantes
		[AllowAnonymous] // ← qualquer um pode acessar o formulário
		public IActionResult Create()
		{
			return View();
		}

		// POST: Contatos/Create — público para visitantes
		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous] // ← qualquer um pode enviar
		public async Task<IActionResult> Create([Bind("Id,Nome,Email,Assunto,Mensagem")] Contato contato)
		{
			if (ModelState.IsValid)
			{
				contato.DataEnvio = DateTime.Now; // ← data gerada no servidor
				_context.Add(contato);
				await _context.SaveChangesAsync();
				TempData["MensagemSucesso"] = "Mensagem enviada com sucesso!";
				return RedirectToAction(nameof(Create));
			}
			return View(contato);
		}

		// GET: Contatos/Edit/5
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null) return NotFound();

			var contato = await _context.Contato.FindAsync(id);
			if (contato == null) return NotFound();
			return View(contato);
		}

		// POST: Contatos/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email,Assunto,Mensagem,DataEnvio")] Contato contato)
		{
			if (id != contato.Id) return NotFound();

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(contato);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ContatoExists(contato.Id)) return NotFound();
					else throw;
				}
				return RedirectToAction(nameof(Index));
			}
			return View(contato);
		}

		// GET: Contatos/Delete/5
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null) return NotFound();

			var contato = await _context.Contato
				.FirstOrDefaultAsync(m => m.Id == id);
			if (contato == null) return NotFound();

			return View(contato);
		}

		// POST: Contatos/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var contato = await _context.Contato.FindAsync(id);
			if (contato != null) _context.Contato.Remove(contato);

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ContatoExists(int id)
		{
			return _context.Contato.Any(e => e.Id == id);
		}
	}
}