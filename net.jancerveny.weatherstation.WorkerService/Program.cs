using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using net.jancerveny.weatherstation.BusinessLayer;

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
					services.AddDbContext<DataLayer.DbContext>(options =>
						options.UseNpgsql(hostContext.Configuration.GetConnectionString("Db")));
					services.AddSingleton<DataCollection>();
					services.AddHostedService<Worker>();
				});
	}
}
