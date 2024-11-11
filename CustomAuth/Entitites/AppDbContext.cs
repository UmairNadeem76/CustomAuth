//AppDbContext.cs
using CustomAuth.Entitites;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	public DbSet<UserAccount> UserAccounts { get; set; }
	public DbSet<TaskList> TaskList { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// Configure the primary key for TaskList
		modelBuilder.Entity<TaskList>()
			.HasKey(t => t.TaskID);

		// Configure the relationship between TaskList and UserAccount
		modelBuilder.Entity<TaskList>()
			.HasOne(t => t.User)
			.WithMany()
			.HasForeignKey(t => t.UserID)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
