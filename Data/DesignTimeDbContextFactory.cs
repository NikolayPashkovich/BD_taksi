using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BD_taksi.Data
{
	public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
	{
		public AppDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
			// LocalDB по умолчанию; строку перепишем в App.xaml.cs при запуске
			optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=BD_Taksi;Trusted_Connection=True;TrustServerCertificate=True");
			return new AppDbContext(optionsBuilder.Options);
		}
	}
} 