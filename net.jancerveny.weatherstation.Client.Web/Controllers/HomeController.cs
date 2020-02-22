using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using net.jancerveny.weatherstation.DataLayer;
using net.jancerveny.weatherstation.Models;
using System.Diagnostics;

namespace net.jancerveny.weatherstation.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly DbContextOptions<WeatherDbContext> _dbOptions;

		public HomeController(ILogger<HomeController> logger, DbContextOptions<WeatherDbContext> dbOptions)
		{
			_logger = logger;
			_dbOptions = dbOptions;
		}

		public IActionResult Index()
		{
			return View();
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
