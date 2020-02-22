using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using net.jancerveny.weatherstation.BusinessLayer;
using net.jancerveny.weatherstation.Common.Helpers;
using net.jancerveny.weatherstation.Common.Models;
using net.jancerveny.weatherstation.DataLayer;

namespace net.jancerveny.weatherstation.WorkerService
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseSystemd() // Linux service lifetime management
				.ConfigureAppConfiguration((context, builder) =>
				{
					builder
						.AddJsonFile("appsettings.json")
						.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json")
						.AddEnvironmentVariables()
						.Build();
				})
				.ConfigureServices((hostContext, services) =>
				{
					var philipsHueConfig = new PhilipsHueConfiguration();
					hostContext.Configuration.GetSection("PhilipsHue").Bind(philipsHueConfig);
					services.AddSingleton(philipsHueConfig);
					services.AddSingleton(Database.GetDbContextOptions<WeatherDbContext>(hostContext.Configuration.GetConnectionString("Db")));
					services.AddSingleton<DataCollection>();
					services.AddHostedService<Worker>();
				});
	}
}
