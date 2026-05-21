using System.Diagnostics;
using Eventos.Data;
using Eventos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eventos.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly EventosContext _context;

		public HomeController(ILogger<HomeController> logger, EventosContext context)
		{
			_logger = logger;
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			var ultimosEventos = await _context.Evento
				.Include(e => e.EventoCategorias).ThenInclude(ec => ec.Categoria)
				.OrderByDescending(e => e.Data)
				.Take(10)
				.ToListAsync();

			return View(ultimosEventos);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}