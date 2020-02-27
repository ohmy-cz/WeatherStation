using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;

namespace net.jancerveny.weatherstation
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((context, builder) =>
				{
					builder
						.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)) // Required for Linux service
						.AddJsonFile("appsettings.json")
						.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json")
						.AddEnvironmentVariables()
						.Build();
				})
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseUrls("http://localhost:5050");
					webBuilder.UseStartup<Startup>();
				});
	}
}
