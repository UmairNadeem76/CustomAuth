// AppDbContext.cs

// Importing required namespaces
using CustomAuth.Entitites; // Namespace containing entity definitions
using Microsoft.EntityFrameworkCore; // For Entity Framework Core functionality

// Defining the AppDbContext class, which serves as the database context for the application
public class AppDbContext : DbContext
{
	// Constructor to initialize the DbContext with the provided options
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	// Defining DbSet for the UserAccount entity, representing the UserAccounts table in the database
	public DbSet<UserAccount> UserAccounts { get; set; }

	// Defining DbSet for the TaskList entity, representing the TaskList table in the database
	public DbSet<TaskList> TaskList { get; set; }

	// Configures the database model using Fluent API
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// Configuring TaskList entity's primary key
		modelBuilder.Entity<TaskList>()
			.HasKey(t => t.TaskID); // Specifies that TaskID is the primary key for the TaskList table

		// Configuring the relationship between TaskList and UserAccount
		modelBuilder.Entity<TaskList>()
			.HasOne(t => t.User) // Specifies that each TaskList is associated with one UserAccount
			.WithMany() // Specifies that a UserAccount can have many TaskLists (default navigation)
			.HasForeignKey(t => t.UserID) // Specifies that UserID in TaskList is a foreign key referencing UserAccount
			.OnDelete(DeleteBehavior.Cascade); // Ensures that deleting a UserAccount deletes all related TaskLists
	}
}
