using Microsoft.EntityFrameworkCore;

namespace net.jancerveny.weatherstation.Common.Helpers
{
	public static class Database
	{
		public static DbContextOptions<TContext> GetDbContextOptions<TContext>(string connectionString) where TContext: DbContext
		{
			return NpgsqlDbContextOptionsExtensions.UseNpgsql(new DbContextOptionsBuilder<TContext>(), connectionString).Options;
		}
	}
}
