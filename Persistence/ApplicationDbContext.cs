using Microsoft.AspNetCore.Identity.EntityFrameworkCore;	
namespace SurveyBasket.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
	IdentityDbContext<ApplicationUser>(options)
{
	public DbSet<Poll> Polls { get; set; }
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		base.OnModelCreating(modelBuilder);
	}
}
