using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using net.jancerveny.weatherstation.DataLayer;
using net.jancerveny.weatherstation.DataLayer.Models;
using net.jancerveny.weatherstation.Models;

namespace net.jancerveny.weatherstation.Controllers
{
	public class HomeController : Controller
	{
		//private readonly WeatherDbContext _db;
		private readonly ILogger<HomeController> _logger;
		private readonly DbContextOptions<WeatherDbContext> _dbOptions;

		public HomeController(ILogger<HomeController> logger/*, WeatherDbContext db*/, DbContextOptions<WeatherDbContext> dbOptions)
		{
			_logger = logger;
			//_db = db;
			_dbOptions = dbOptions;
		}

		public IActionResult Index()
		{
			using (var db = new WeatherDbContext(_dbOptions))
			{
				var temperature = new Temperatures
				{
					SensorId = 1,
					Temperature = 3000,
					Timestamp = DateTime.Now
				};
				db.Temperatures.Add(temperature);
				db.SaveChanges();
			}

			//var temperature = new Temperatures
			//{
			//	SensorId = 1,
			//	Temperature = 3000,
			//	Timestamp = DateTime.Now
			//};
			//_db.Temperatures.Add(temperature);
			//_db.SaveChanges();


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
